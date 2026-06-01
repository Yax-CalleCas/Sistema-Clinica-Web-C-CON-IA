using MediCita.Web.Entidades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediCita.Web.Servicios.Contrato
{
    public interface IReporte
    {
        //  Reporte de citas (con filtro por estado opcional)
        Task<List<ReporteCita>> ReporteCitas(
            DateTime fechaInicio,
            DateTime fechaFin,
            string estado = null   // "P", "A", "C" o null
        );

        // 💰 Reporte de ventas
        Task<List<ReporteVenta>> ReporteVentas(
            DateTime fechaInicio,
            DateTime fechaFin
        );
    }
}
