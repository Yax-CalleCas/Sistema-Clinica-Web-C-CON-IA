using MediCita.Web.Entidades;
using MediCita.Web.Servicios.Contrato;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MediCita.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReporteController : Controller
    {
        private readonly IReporte _reporteService;
        private const int PageSize = 4;

        public ReporteController(IReporte reporteService)
        {
            _reporteService = reporteService;
        }   

        // VISTA INICIAL
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ===============================
        // REPORTE DE CITAS MÉDICAS
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReporteCitas(DateTime fechaInicio, DateTime fechaFin, string estado, int page = 1)
        {
            if (!RangoValido(fechaInicio, fechaFin))
            {
                TempData["Error"] = "El rango de fechas no es válido.";
                return RedirectToAction(nameof(Index));
            }

            // Obtenemos los datos del servicio
            var rawData = await _reporteService.ReporteCitas(fechaInicio, fechaFin, estado);

            // 🔥 SOLUCIÓN AL ERROR: Aseguramos que la lista exista aunque esté vacía
            var data = rawData ?? new List<MediCita.Web.Entidades.ReporteCita>();

            // Realizamos la paginación de forma segura
            var resultado = data
                .OrderByDescending(x => x.FechaCita)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // Cargamos los datos de control
            CargarViewBagPaginacion(data.Count, page, fechaInicio, fechaFin);
            ViewBag.Estado = string.IsNullOrEmpty(estado) ? "Todos" : estado;

            // Enviamos el total recaudado para evitar cálculos pesados en el Razor
            ViewBag.TotalRecaudado = data.Sum(x => x.MontoPagar ?? 0);

            return View(resultado);
        }


        // ===============================
        // REPORTE DE VENTAS
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReporteVentas(DateTime fechaInicio, DateTime fechaFin, int page = 1)
        {
            if (!RangoValido(fechaInicio, fechaFin))
            {
                TempData["Error"] = "El rango de fechas no es válido.";
                return RedirectToAction(nameof(Index));
            }

            var data = await _reporteService.ReporteVentas(fechaInicio, fechaFin);

            var resultado = data
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            CargarViewBagPaginacion(data.Count, page, fechaInicio, fechaFin);

            return View(resultado);
        }

        // ===============================
        // EXPORTAR CITAS A PDF
        // ===============================
        [HttpGet]
        public async Task<IActionResult> ExportarCitasPdf(string fechaInicio, string fechaFin, string estado)
        {
            // Convertir strings a DateTime de forma segura
            if (!DateTime.TryParse(fechaInicio, out DateTime fInicio)) fInicio = DateTime.Now.AddDays(-30);
            if (!DateTime.TryParse(fechaFin, out DateTime fFin)) fFin = DateTime.Now;

            var data = await _reporteService.ReporteCitas(fInicio, fFin, estado);

            ViewBag.FechaInicio = fInicio.ToString("dd/MM/yyyy");
            ViewBag.FechaFin = fFin.ToString("dd/MM/yyyy");
            ViewBag.Estado = string.IsNullOrEmpty(estado) || estado == "Todos" ? "Todos" : estado;

            return View("ExportarCitasPdf", data ?? new List<ReporteCita>());
        }

        // ===============================
        // MÉTODOS PRIVADOS
        // ===============================
        private static bool RangoValido(DateTime inicio, DateTime fin)
        {
            return inicio.Date <= fin.Date;
        }

        private void CargarViewBagPaginacion(int totalRegistros, int page, DateTime inicio, DateTime fin)
        {
            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalRegistros / (double)PageSize);
            ViewBag.FechaInicio = inicio.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fin.ToString("yyyy-MM-dd");
        }
    }
}
