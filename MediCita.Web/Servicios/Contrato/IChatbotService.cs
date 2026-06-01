
namespace ClinPiura.Web.Servicios.Contrato
{
    public interface IChatbotService
    {
        Task<string> ResponderConsulta(
            int? idUsuario,
            string mensaje,
            bool estaLogueado,
            string nombreUsuario);
    }
}