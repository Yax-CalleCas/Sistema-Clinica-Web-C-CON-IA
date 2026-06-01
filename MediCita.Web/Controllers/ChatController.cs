using ClinPiura.Web.Servicios.Contrato;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace ClinPiura.Web.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatbotService _chatbotService;

        public ChatController(IChatbotService chatbotService)
        {
            _chatbotService = chatbotService;
        }

        [HttpPost]
        public async Task<IActionResult> Consultar([FromBody] string mensaje)
        {
            if (string.IsNullOrWhiteSpace(mensaje))
            {
                return Json(new
                {
                    respuesta = "Escribe un mensaje válido."
                });
            }

            try
            {
                bool estaLogueado =
                    User.Identity?.IsAuthenticated ?? false;

                int? idUsuario = null;

                string? userIdClaim =
                    User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(userIdClaim, out int id))
                {
                    idUsuario = id;
                }

                string nombreUsuario =
                    User.FindFirstValue(ClaimTypes.Name)
                    ?? "Invitado";

var respuesta =
    await _chatbotService.ResponderConsulta(
        idUsuario,
        mensaje,
        estaLogueado,
        nombreUsuario);


                return Json(new
                {
                    respuesta = respuesta
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    respuesta = ex.Message
                });
            }
        }
    }
}