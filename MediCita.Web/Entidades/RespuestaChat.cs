namespace ClinPiura.Web.Entidades
{
    public class RespuestaChat
    {
        public string Texto { get; set; }
        public List<OpcionAccion> Opciones { get; set; } = new List<OpcionAccion>();
    }

    public class OpcionAccion
    {
        public string Etiqueta { get; set; }
        public string Valor { get; set; } // Lo que se envía al servidor al hacer clic

    }
}