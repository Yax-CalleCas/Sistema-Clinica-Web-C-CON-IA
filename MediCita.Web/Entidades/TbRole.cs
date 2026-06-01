using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbRole
{
    public int IdRol { get; set; }

    public string NombreRol { get; set; } = null!;

    public virtual ICollection<TbUsuario> TbUsuarios { get; set; } = new List<TbUsuario>();
}
