using Microsoft.AspNetCore.Mvc;
using MediCita.Web.Entidades;
using MediCita.Web.Servicios.Contrato;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MediCita.Web.Controllers
{
    public class CitasController : Controller
    {
        private readonly ICitaService _citaService;

        public CitasController(ICitaService citaService)
        {
            _citaService = citaService;
        }

        // --- FLUJO DE RESERVA ---

        public async Task<IActionResult> Crear(int? idEspecialidad, DateTime? fecha)
        {
            fecha ??= DateTime.Today;
            var medicos = await _citaService.ListarMedicosDisponibles(idEspecialidad, fecha);

            var listaUnica = medicos.GroupBy(m => m.IdMedico)
                                    .Select(g => g.First())
                                    .ToList();
            return View(listaUnica);
        }
        public async Task<IActionResult> DetalleCita(int idCita)
        {
            if (idCita <= 0)
                return BadRequest();

            int idPaciente = Convert.ToInt32(
                User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"
            );

            if (idPaciente == 0)
                return Unauthorized();

            var detalle = await _citaService.VerDetalleCitaPaciente(idCita, idPaciente);

            if (detalle == null)
                return NotFound();

            return View(detalle);
        }

        public async Task<IActionResult> Horarios(int idMedico, DateTime fecha)
    {
        //  Seguridad básica
        if (idMedico <= 0)
            return BadRequest();

        // Si la fecha es inválida, usamos hoy
        if (fecha < DateTime.Today)
            fecha = DateTime.Today;

        // 1️ Buscar horarios para la fecha solicitada
        var horarios = await _citaService.ListarHorariosDisponibles(idMedico, fecha);

        // 2️ Si HOY no hay horarios, buscar automáticamente el siguiente día con disponibilidad
        int intentos = 0;
        while (!horarios.Any() && intentos < 7) // máximo 7 días adelante
        {
            fecha = fecha.AddDays(1);
            horarios = await _citaService.ListarHorariosDisponibles(idMedico, fecha);
            intentos++;
        }

        // 3️ Datos del usuario
        ViewBag.IdPaciente = Convert.ToInt32(
            User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"
        );

        ViewBag.IdMedico = idMedico;
        ViewBag.Fecha = fecha;

        // 4️ Obtener info del médico
        var medico = (await _citaService.ListarMedicosDisponibles(null, fecha))
                        .FirstOrDefault(m => m.IdMedico == idMedico);

        ViewBag.Precio = medico?.PrecioConsulta ?? 0;
        ViewBag.NombreMedico = medico?.NombreCompleto ?? "Médico";

        // 5️⃣ Mensaje friendly si no hay horarios ni en próximos días
        if (!horarios.Any())
        {
            TempData["Error"] = "No hay horarios disponibles para los próximos días.";
        }

        return View(horarios);
    }


    // NUEVO: Acción para procesar el pago y crear la cita
    [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarCitaConPago(int idPaciente, int idMedico, DateTime fecha,
                                                            string horaInicioStr, string horaFinStr,
                                                            decimal monto, string idTransaccion)
        {
            try
            {
                // 1. Validaciones básicas de tiempo
                TimeSpan.TryParse(horaInicioStr, out TimeSpan horaInicio);
                TimeSpan.TryParse(horaFinStr, out TimeSpan horaFin);

                // 2. Si el idTransaccion viene vacío, simulamos uno (En producción esto lo da la pasarela)
                if (string.IsNullOrEmpty(idTransaccion))
                {
                    idTransaccion = "PAY-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
                }

                // 3. Llamada al nuevo método del servicio que creamos antes
                int resultado = await _citaService.CrearCitaConPago(
                    idPaciente,
                    idMedico,
                    fecha.Date,
                    horaInicio,
                    horaFin,
                    monto,
                    idTransaccion
                );

                if (resultado > 0)
                {
                    TempData["Success"] = $"¡Cita reservada y pagada con éxito! Transacción: {idTransaccion}";
                    return RedirectToAction(nameof(MisCitasPaciente));
                }
                else
                {
                    TempData["Error"] = "El horario ya no está disponible. Por favor, elige otro.";
                    return RedirectToAction("Horarios", new { idMedico = idMedico, fecha = fecha });
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hubo un error al procesar la cita: " + ex.Message;
                return RedirectToAction("Crear");
            }
        }

        // --- SEGUIMIENTO ---

        public async Task<IActionResult> MisCitasPaciente()
        {
            int idPaciente = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var citas = await _citaService.ListarCitas(idPaciente, null);
            return View(citas ?? new List<Cita>());
        }

        //panel del medico ver detalles
        public async Task<IActionResult> PanelMedico(DateTime? fecha, string estado)
        {
            int idMedico = Convert.ToInt32(User.FindFirstValue("IdMedico") ?? "0");
            if (idMedico == 0) return Unauthorized();

            var citas = await _citaService.ListarCitasDetalleMedico(idMedico);

            // 🔹 Filtro por fecha
            if (fecha.HasValue)
            {
                citas = citas
                    .Where(c => c.FechaCita.Date == fecha.Value.Date)
                    .ToList();
            }

            // 🔹 Filtro por estado
            if (!string.IsNullOrEmpty(estado))
            {
                citas = citas
                    .Where(c => c.Estado == estado)
                    .ToList();
            }

            return View(citas);
        }

        public async Task<IActionResult> MisCitasMedico()
        {
            int idMedico = Convert.ToInt32(User.FindFirstValue("IdMedico") ?? "0");
            var citas = await _citaService.ListarCitas(null, idMedico);
            return View(citas ?? new List<Cita>());
        }

        // --- GESTIÓN MÉDICA ---

        public IActionResult Atender(int idCita)
        {
            return View(new Cita { IdCita = idCita });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Atender(Cita modelo)
        {
            if (modelo.IdCita <= 0) return BadRequest();
            await _citaService.AtenderCita(modelo.IdCita, modelo.Estado ?? "A", modelo.Nota);
            TempData["Success"] = "Consulta finalizada con éxito.";
            return RedirectToAction(nameof(MisCitasMedico));
        }
    }
}