using MediCita.Web.Entidades;
using MediCita.Web.Servicios.Contrato;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MediCita.Web.Servicios.Implementacion
{
    public class ReporteService : IReporte
    {
        private readonly string cadena;

        public ReporteService(IConfiguration config)
        {
            cadena = config.GetConnectionString("CadenaSQL")
                     ?? throw new Exception("CadenaSQL no encontrada en appsettings.json");
        }
               
        // =======================
        // REPORTE DE CITAS
        // =======================
        // =======================
        // REPORTE DE CITAS
        // =======================
        public async Task<List<ReporteCita>> ReporteCitas(
    DateTime fechaInicio,
    DateTime fechaFin,
    string estado = null   // OPCIONAL: "P", "A", "C"
)
        {
            var lista = new List<ReporteCita>();

            using var cn = new SqlConnection(cadena);
            using var cmd = new SqlCommand("sp_ReporteCitasPorFecha", cn)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = fechaInicio.Date;
            cmd.Parameters.Add("@FechaFin", SqlDbType.Date).Value = fechaFin.Date;

            // parámetro opcional
            cmd.Parameters.Add("@Estado", SqlDbType.Char, 1)
                          .Value = string.IsNullOrWhiteSpace(estado)
                                    ? DBNull.Value
                                    : estado;

            await cn.OpenAsync();

            using var dr = await cmd.ExecuteReaderAsync();
            while (await dr.ReadAsync())
            {
                // Dentro del while (await dr.ReadAsync())
                lista.Add(new ReporteCita
                {
                    IdCita = Convert.ToInt32(dr["IdCita"]),
                    Paciente = dr["Paciente"]?.ToString() ?? "",
                    Medico = dr["Medico"]?.ToString() ?? "",
                    CMP = dr["CMP"]?.ToString() ?? "",
                    Especialidad = dr["Especialidad"]?.ToString() ?? "",
                    FechaCita = Convert.ToDateTime(dr["FechaCita"]),
                    HoraInicio = dr["HoraInicio"] != DBNull.Value ? (TimeSpan?)dr["HoraInicio"] : null,
                    MontoPagar = dr["MontoPagar"] != DBNull.Value ? Convert.ToDecimal(dr["MontoPagar"]) : null,

                    // 🔥 Asegúrate de mapear la propiedad Estado
                    Estado = dr["Estado"]?.ToString() ?? "",
                    EstadoDescripcion = dr["EstadoDescripcion"]?.ToString() ?? ""
                });
            }

            return lista;
        }


        // REPORTE DE VENTAS
        public async Task<List<ReporteVenta>> ReporteVentas(DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = new List<ReporteVenta>();

            using (SqlConnection cn = new SqlConnection(cadena))
            using (SqlCommand cmd = new SqlCommand("sp_ReporteVentasPorFecha", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio.Date);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin.Date);

                await cn.OpenAsync();
                using var dr = await cmd.ExecuteReaderAsync();

                while (await dr.ReadAsync())
                {
                    lista.Add(new ReporteVenta
                    {
                        IdVenta = Convert.ToInt32(dr["IdVenta"]),
                        Paciente = dr["Paciente"].ToString(),
                        FechaVenta = Convert.ToDateTime(dr["FechaVenta"]),
                        NombreMedicamento = dr["NombreMedicamento"].ToString(),
                        Cantidad = Convert.ToInt32(dr["Cantidad"]),
                        PrecioUnitario = Convert.ToDecimal(dr["PrecioUnitario"]),
                        SubTotal = Convert.ToDecimal(dr["SubTotal"]),
                        VentaSubTotal = Convert.ToDecimal(dr["VentaSubTotal"]),
                        IGV = Convert.ToDecimal(dr["IGV"]),
                        TotalFinal = Convert.ToDecimal(dr["TotalFinal"]),
                        MetodoPago = dr["MetodoPago"].ToString(),
                        PorcentajeDescuento = Convert.ToInt32(dr["PorcentajeDescuento"])
                    });
                }
            }

            return lista;
        }

    }
}
