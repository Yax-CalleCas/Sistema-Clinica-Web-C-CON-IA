using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbHorariosPlantilla
{
    public int IdHorarioPlantilla { get; set; }

    public TimeOnly HoraInicio { get; set; }

    public TimeOnly HoraFin { get; set; }
}
