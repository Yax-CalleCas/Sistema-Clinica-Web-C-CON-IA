namespace ClinPiura.Web.Models.DTOs
{
    // Para que el Chatbot entienda qué medicamentos hay
    public class MedicamentoChatDto
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public string Categoria { get; set; }
    }

}
