using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbEspecialidade
{
    public int IdEspecialidad { get; set; }

    public string NombreEspec { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual ICollection<TbMedico> TbMedicos { get; set; } = new List<TbMedico>();
}
