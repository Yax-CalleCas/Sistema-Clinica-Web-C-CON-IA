using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbMedicamento
{
    public int IdMedicamento { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Laboratorio { get; set; }

    public decimal Precio { get; set; }

    public int Stock { get; set; }

    public string? ImagenUrl { get; set; }

    public string? Descripcion { get; set; }

    public string? Promocion { get; set; }

    public string? Categoria { get; set; }

    public virtual ICollection<TbCarritoTemporal> TbCarritoTemporals { get; set; } = new List<TbCarritoTemporal>();

    public virtual ICollection<TbDetalleVentum> TbDetalleVenta { get; set; } = new List<TbDetalleVentum>();
}
