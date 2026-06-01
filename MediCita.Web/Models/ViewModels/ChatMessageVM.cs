namespace ClinPiura.Web.Models.ViewModels
{
    public class ChatMessageVM
    {
        public string Usuario { get; set; }
        public string Mensaje { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool EsIA { get; set; }
    }
}