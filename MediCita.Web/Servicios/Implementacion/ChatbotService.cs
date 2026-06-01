using ClinPiura.Web.Data;
using ClinPiura.Web.Servicios.Contrato;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Data;
using System.Text;
using System.Text.Json;

namespace ClinPiura.Web.Servicios.Implementacion
{
    public class ChatbotService : IChatbotService
    {
        private readonly ClinicaContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        // =========================================================
        // MEMORIA DE SESIONES (EN RAMA)
        // cada usuario tiene su sessionId fijo
        // =========================================================
        private static readonly ConcurrentDictionary<string, string> _sesiones = new();

        public ChatbotService(
            ClinicaContext context,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        // =========================================================
        // RESPONDER CONSULTA PRINCIPAL (CHATBOT)
        // =========================================================
        public async Task<string> ResponderConsulta(
            int? idUsuario,
            string mensaje,
            bool estaLogueado,
            string nombreUsuario)
        {
            try
            {
                // 1. Validar API Key de Groq
                string apiKey = _configuration["ChatbotSettings:GroqKey"] ?? "";

                if (string.IsNullOrWhiteSpace(apiKey))
                    return "API Key de Groq no configurada.";

                // 2. Obtener contexto desde BD (médicos, medicamentos, horarios)
                string contexto = await ObtenerContextoBaseDatos();

                // 3. Obtener memoria del chat (últimos mensajes)
                string memoriaChat = await ObtenerUltimosMensajes(idUsuario);

                // 4. Construir prompt final
                string prompt = GenerarPrompt(contexto + "\n\nHISTORIAL CHAT:\n" + memoriaChat);

                // 5. Obtener sessionId estable por usuario
                string sessionId = ObtenerSessionId(idUsuario);

                // 6. Armar request a la IA (Groq)
                var requestBody = new
                {
                    model = "llama-3.3-70b-versatile",
                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = prompt
                        },
                        new
                        {
                            role = "user",
                            content = mensaje
                        }
                    },
                    temperature = 0.5,
                    max_tokens = 800
                };

                // 7. Configurar headers HTTP
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                // 8. Llamar API de Groq
                var response = await _httpClient.PostAsync(
                    "https://api.groq.com/openai/v1/chat/completions",
                    new StringContent(
                        JsonSerializer.Serialize(requestBody),
                        Encoding.UTF8,
                        "application/json")
                );

                string responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "Error IA: " + responseString;

                // 9. Extraer respuesta IA
                using var doc = JsonDocument.Parse(responseString);

                string respuestaIA =
                    doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString()
                    ?? "No se pudo generar respuesta.";

                // 10. Guardar historial en BD usando SP
                await GuardarEnHistorial(idUsuario, mensaje, respuestaIA, sessionId);

                return respuestaIA;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        // =========================================================
        // PROMPT DEL SISTEMA (REGLAS DEL BOT)
        // =========================================================
        private string GenerarPrompt(string contexto)
        {
            return $@"
Eres el asistente virtual de MediCita Piura.

REGLAS:
- Responde solo temas médicos.
- Usa HTML con Bootstrap 5.
- No inventes información.
- No muestres datos privados.
- Solo usa el contexto proporcionado.

FUNCIONES:
- Médicos => cards Bootstrap
- Medicamentos => listas Bootstrap
- Horarios => tablas Bootstrap

SI EL USUARIO SALUDA:
- Mostrar botones: Médicos, Farmacia, Horarios

CONTEXTO:
{contexto}
";
        }

        // =========================================================
        // OBTENER CONTEXTO DESDE SQL SERVER
        // (usa SP sp_Chatbot_ObtenerContexto)
        // =========================================================
        private async Task<string> ObtenerContextoBaseDatos()
        {
            StringBuilder sb = new();

            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_Chatbot_ObtenerContexto";
                command.CommandType = CommandType.StoredProcedure;

                if (_context.Database.GetDbConnection().State != ConnectionState.Open)
                    await _context.Database.OpenConnectionAsync();

                using var reader = await command.ExecuteReaderAsync();

                // ================= MEDICOS =================
                sb.AppendLine("MEDICOS:");

                while (await reader.ReadAsync())
                {
                    sb.AppendLine(
                        $"{reader["NombreCompleto"]} - " +
                        $"{reader["NombreEspec"]} - " +
                        $"S/{reader["PrecioConsulta"]}");
                }

                // ================= MEDICAMENTOS =================
                if (await reader.NextResultAsync())
                {
                    sb.AppendLine("MEDICAMENTOS:");

                    while (await reader.ReadAsync())
                    {
                        sb.AppendLine(
                            $"{reader["Nombre"]} - " +
                            $"S/{reader["Precio"]} - " +
                            $"Stock:{reader["Stock"]}");
                    }
                }

                // ================= HORARIOS =================
                if (await reader.NextResultAsync())
                {
                    sb.AppendLine("HORARIOS:");

                    while (await reader.ReadAsync())
                    {
                        sb.AppendLine(
                            $"{reader["Medico"]} - " +
                            $"{Convert.ToDateTime(reader["Fecha"]):dd/MM/yyyy} - " +
                            $"{reader["HoraInicio"]} - {reader["HoraFin"]}");
                    }
                }
            }
            catch
            {
                return "No se pudo obtener información.";
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }

            return sb.ToString();
        }

        // =========================================================
        // OBTENER MEMORIA DEL CHAT (ULTIMOS MENSAJES)
        // =========================================================
        private async Task<string> ObtenerUltimosMensajes(int? idUsuario)
        {
            StringBuilder sb = new();

            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();

                command.CommandText = @"
                    SELECT TOP 6 MensajeUsuario, RespuestaBot
                    FROM tb_ChatHistorial
                    WHERE (@id IS NULL OR IdUsuario = @id)
                    ORDER BY FechaRegistro DESC";

                var param = command.CreateParameter();
                param.ParameterName = "@id";
                param.Value = idUsuario ?? (object)DBNull.Value;

                command.Parameters.Add(param);
                command.CommandType = CommandType.Text;

                if (_context.Database.GetDbConnection().State != ConnectionState.Open)
                    await _context.Database.OpenConnectionAsync();

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    sb.AppendLine("Usuario: " + reader["MensajeUsuario"]);
                    sb.AppendLine("Bot: " + reader["RespuestaBot"]);
                    sb.AppendLine("---");
                }
            }
            catch
            {
                return "";
            }

            return sb.ToString();
        }

        // =========================================================
        // OBTENER SESSION ID FIJO POR USUARIO
        // =========================================================
        private string ObtenerSessionId(int? idUsuario)
        {
            string key = idUsuario?.ToString() ?? "anonimo";

            if (_sesiones.TryGetValue(key, out string sessionId))
                return sessionId;

            sessionId = Guid.NewGuid().ToString();
            _sesiones[key] = sessionId;

            return sessionId;
        }

        // =========================================================
        // GUARDAR HISTORIAL (USANDO STORED PROCEDURE)
        // =========================================================
        private async Task GuardarEnHistorial(int? idUsuario, string mensajeUsuario, string respuestaBot, string sessionId)
        {
            try
            {
                // Usamos parámetros nominales de SQL Server para mayor seguridad
                var idParam = new Microsoft.Data.SqlClient.SqlParameter("@IdUsuario", (object)idUsuario ?? DBNull.Value);
                var mensajeParam = new Microsoft.Data.SqlClient.SqlParameter("@MensajeUsuario", mensajeUsuario);
                var respuestaParam = new Microsoft.Data.SqlClient.SqlParameter("@RespuestaBot", respuestaBot);
                var sessionParam = new Microsoft.Data.SqlClient.SqlParameter("@SessionId", sessionId);

                // ExecuteSqlRawAsync maneja internamente la conexión, el comando y el cierre de forma óptima
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Chatbot_GuardarHistorial @IdUsuario, @MensajeUsuario, @RespuestaBot, @SessionId",
                    idParam, mensajeParam, respuestaParam, sessionParam
                );

                System.Diagnostics.Debug.WriteLine("DEBUG: Datos guardados con éxito vía ExecuteSqlRawAsync.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR CRÍTICO AL GUARDAR EN BD: {ex.Message}");
                throw;
            }
        }
    }
}