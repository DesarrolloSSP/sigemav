using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class MantenimientoDetalle
{
    public int MantenimientoDetalleId { get; set; }

    public int MantenimientoId { get; set; }

    public int ConceptoServicioId { get; set; }

    public int Cantidad { get; set; }

    public decimal CostoUnitarioManoObra { get; set; }

    public decimal CostoUnitarioRefacciones { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int? IdObjetoGastoManoObra { get; set; }

    public int? IdObjetoGastoRefacciones { get; set; }

    public decimal? SubtotalManoObra { get; set; }

    public decimal? SubtotalRefacciones { get; set; }

    public decimal? TotalLinea { get; set; }

    public virtual ConceptoServicio ConceptoServicio { get; set; } = null!;

    public virtual ObjetoGasto? IdObjetoGastoManoObraNavigation { get; set; }

    public virtual ObjetoGasto? IdObjetoGastoRefaccionesNavigation { get; set; }

    public virtual Mantenimiento Mantenimiento { get; set; } = null!;
}
