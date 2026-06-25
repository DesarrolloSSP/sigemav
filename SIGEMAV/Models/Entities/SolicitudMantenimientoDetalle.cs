using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class SolicitudMantenimientoDetalle
{
    public int SolicitudMantenimientoDetalleId { get; set; }

    public int SolicitudMantenimientoId { get; set; }

    public int ConceptoServicioId { get; set; }

    public int Cantidad { get; set; }

    public decimal CostoEstimadoManoObra { get; set; }

    public decimal CostoEstimadoRefacciones { get; set; }

    public string? Observaciones { get; set; }

    public DateTime FechaCreacion { get; set; }

    public bool Activo { get; set; }

    public int? IdObjetoGastoManoObra { get; set; }

    public int? IdObjetoGastoRefacciones { get; set; }

    public virtual ConceptoServicio ConceptoServicio { get; set; } = null!;

    public virtual ObjetoGasto? IdObjetoGastoManoObraNavigation { get; set; }

    public virtual ObjetoGasto? IdObjetoGastoRefaccionesNavigation { get; set; }

    public virtual SolicitudMantenimiento SolicitudMantenimiento { get; set; } = null!;
}
