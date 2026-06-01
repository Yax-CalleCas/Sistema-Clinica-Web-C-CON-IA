using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbCita
{
    public int IdCita { get; set; }

    public int IdPaciente { get; set; }

    public int IdMedico { get; set; }

    public DateTime FechaCita { get; set; }

    public TimeOnly? HoraInicio { get; set; }

    public TimeOnly? HoraFin { get; set; }

    public decimal? MontoPagar { get; set; }

    public string? Estado { get; set; }

    public virtual TbMedico IdMedicoNavigation { get; set; } = null!;

    public virtual TbUsuario IdPacienteNavigation { get; set; } = null!;

    public virtual ICollection<TbCitaNota> TbCitaNota { get; set; } = new List<TbCitaNota>();
}
