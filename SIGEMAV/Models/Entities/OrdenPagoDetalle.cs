using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class OrdenPagoDetalle
{
    public int IdDetalle { get; set; }

    public int IdOrdenPago { get; set; }

    public string Concepto { get; set; } = null!;

    public int IdPartida { get; set; }

    public string Serie { get; set; } = null!;

    public string Folio { get; set; } = null!;

    public DateOnly FechaFactura { get; set; }

    public string NoPlaca { get; set; } = null!;

    public decimal Importe { get; set; }

    public virtual OrdenPago IdOrdenPagoNavigation { get; set; } = null!;

    public virtual Partidum IdPartidaNavigation { get; set; } = null!;
}
