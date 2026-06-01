namespace ClinPiura.Web.Entidades
{
    using MediCita.Web.Entidades;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace ClinPiura.Web.Entidades
    {
        [Table("tb_ChatHistorial")]
        public class ChatHistorial
        {
            [Key]
            public int IdChat { get; set; }

            public int? IdUsuario { get; set; }

            [Required]
            public string MensajeUsuario { get; set; } = null!;

            [Required]
            public string RespuestaBot { get; set; } = null!;

            public DateTime FechaRegistro { get; set; }

            public string? SessionId { get; set; }

            // ==========================================
            // RELACION CON USUARIO
            // ==========================================
            [ForeignKey("IdUsuario")]
            public virtual Usuario? Usuario { get; set; }
        }
    }
}