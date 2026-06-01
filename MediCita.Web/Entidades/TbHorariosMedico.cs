using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbHorariosMedico
{
    public int IdHorario { get; set; }

    public int IdMedico { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }

    public bool Disponible { get; set; }

    public DateOnly? Fecha { get; set; }

    public virtual TbMedico IdMedicoNavigation { get; set; } = null!;
}
