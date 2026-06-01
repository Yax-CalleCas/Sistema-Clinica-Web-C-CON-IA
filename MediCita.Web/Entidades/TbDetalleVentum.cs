using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbDetalleVentum
{
    public int IdDetalle { get; set; }

    public int IdVenta { get; set; }

    public int IdMedicamento { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal SubTotal { get; set; }

    public string? NombreMedicamento { get; set; }

    public string? ImagenUrl { get; set; }

    public string? Promocion { get; set; }

    public virtual TbMedicamento IdMedicamentoNavigation { get; set; } = null!;

    public virtual TbVenta IdVentaNavigation { get; set; } = null!;
}
