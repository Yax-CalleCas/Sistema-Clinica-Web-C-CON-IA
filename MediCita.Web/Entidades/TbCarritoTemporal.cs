using System;
using System.Collections.Generic;

namespace ClinPiura.Web.Entidades;

public partial class TbCarritoTemporal
{
    public int IdCarrito { get; set; }

    public int IdUsuario { get; set; }

    public int IdMedicamento { get; set; }

    public int Cantidad { get; set; }

    public DateTime? FechaAgregado { get; set; }

    public virtual TbMedicamento IdMedicamentoNavigation { get; set; } = null!;

    public virtual TbUsuario IdUsuarioNavigation { get; set; } = null!;
}
