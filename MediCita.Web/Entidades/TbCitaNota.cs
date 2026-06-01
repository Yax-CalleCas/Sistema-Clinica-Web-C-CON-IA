using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbCitaNota
{
    public int IdNota { get; set; }

    public int IdCita { get; set; }

    public string? Nota { get; set; }

    public DateTime? FechaNota { get; set; }

    public virtual TbCita IdCitaNavigation { get; set; } = null!;
}
