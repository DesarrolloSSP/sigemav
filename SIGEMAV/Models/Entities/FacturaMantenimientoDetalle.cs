using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class FacturaMantenimientoDetalle
{
    public int FacturaDetalleId { get; set; }

    public int? FacturaMantenimientoId { get; set; }

    public int? MantenimientoDetalleId { get; set; }

    public int? ConceptoServicioId { get; set; }

    public int? Cantidad { get; set; }

    public decimal? CostoManoObra { get; set; }

    public decimal? CostoRefacciones { get; set; }

    public decimal? Subtotal { get; set; }

    public decimal? Iva { get; set; }

    public decimal? TotalLinea { get; set; }

    public int? IdObjetoGastoManoObra { get; set; }

    public int? IdObjetoGastoRefacciones { get; set; }

    public virtual FacturaMantenimiento? FacturaMantenimiento { get; set; }
}
