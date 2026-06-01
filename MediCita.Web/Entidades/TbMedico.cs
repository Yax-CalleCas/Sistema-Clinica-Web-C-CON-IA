using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbMedico
{
    public int IdMedico { get; set; }

    public int IdUsuario { get; set; }

    public int? IdEspecialidad { get; set; }

    public string Cmp { get; set; } = null!;

    public string? Rne { get; set; }

    public string? Telefono { get; set; }

    public decimal PrecioConsulta { get; set; }

    public int DuracionMinutos { get; set; }

    public string? TipoServicio { get; set; }

    public TimeOnly? HorarioAtencionInicio { get; set; }

    public TimeOnly? HorarioAtencionFin { get; set; }

    public string? ImagenUrl { get; set; }

    public virtual TbEspecialidade? IdEspecialidadNavigation { get; set; }

    public virtual TbUsuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<TbCita> TbCita { get; set; } = new List<TbCita>();

    public virtual ICollection<TbHorariosMedico> TbHorariosMedicos { get; set; } = new List<TbHorariosMedico>();
}
