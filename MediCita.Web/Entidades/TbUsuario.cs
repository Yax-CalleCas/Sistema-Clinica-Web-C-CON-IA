using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbUsuario
{
    public int IdUsuario { get; set; }

    public string NombreCompleto { get; set; } = null!;

    public string? Dni { get; set; }

    public string Correo { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public int IdRol { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual TbRole IdRolNavigation { get; set; } = null!;

    public virtual ICollection<TbCarritoTemporal> TbCarritoTemporals { get; set; } = new List<TbCarritoTemporal>();

    public virtual ICollection<TbCita> TbCita { get; set; } = new List<TbCita>();

    public virtual TbMedico? TbMedico { get; set; }

    public virtual ICollection<TbVenta> TbVenta { get; set; } = new List<TbVenta>();
}
