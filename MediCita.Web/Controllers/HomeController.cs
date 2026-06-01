using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediCita.Web.Controllers
{
    public class HomeController : Controller
    {
        // Página principal
        public IActionResult Index()
        {
            return View();
        }

        // Servicios
        public IActionResult Servicios()
        {
            return View();
        }

        // Especialistas
        public IActionResult Especialistas()
        {
            return View();
        }

        // Clínicas
        public IActionResult Clinicas()
        {
            return View();
        }

        // Sobre Nosotros
        public IActionResult Nosotros()
        {
            return View();
        }

        // Vista de Acceso Denegado
        [AllowAnonymous]
        public IActionResult Denegado(string? returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? Url.Content("~/");
            return View();
        }

        // Cerrar Sesión
        public async Task<IActionResult> Salir()
        {
            // Eliminar autenticación
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Limpiar sesión
            HttpContext.Session.Clear();

            TempData["MensajeLogout"] = "Has cerrado sesión correctamente.";

            return RedirectToAction("Index");
        }
    }
}