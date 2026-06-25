using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class MovimientoPresupuestal
{
    public int IdMovimiento { get; set; }

    public int? IdDispPresupuestal { get; set; }

    public DateTime? FechaMovimiento { get; set; }

    public string? TipoMovimiento { get; set; }

    public decimal? Importe { get; set; }

    public string? Observaciones { get; set; }

    public DateTime? FechaCaptura { get; set; }

    public string? UsuarioCaptura { get; set; }

    public bool? Activo { get; set; }

    public int? SolicitudMantenimientoId { get; set; }

    public virtual DispPresupuestal? IdDispPresupuestalNavigation { get; set; }

    public virtual SolicitudMantenimiento? SolicitudMantenimiento { get; set; }
}
