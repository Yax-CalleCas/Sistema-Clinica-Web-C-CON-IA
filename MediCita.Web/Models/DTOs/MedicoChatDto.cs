namespace ClinPiura.Web.Models.DTOs
{
    // Para enviar info segura de medicos al Chatbot
    public class MedicoChatDto
    {
        public string NombreCompleto { get; set; }
        public string Especialidad { get; set; }
        public decimal Precio { get; set; }
        public string Imagen { get; set; }
    }

}