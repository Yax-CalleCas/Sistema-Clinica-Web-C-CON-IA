using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbVenta
{
    public int IdVenta { get; set; }

    public int IdPaciente { get; set; }

    public DateTime? FechaVenta { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Igv { get; set; }

    public decimal TotalFinal { get; set; }

    public string MetodoPago { get; set; } = null!;

    public virtual TbUsuario IdPacienteNavigation { get; set; } = null!;

    public virtual ICollection<TbDetalleVentum> TbDetalleVenta { get; set; } = new List<TbDetalleVentum>();
}
