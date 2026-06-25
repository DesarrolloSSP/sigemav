using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class MantenimientoSeguimiento
{
    public int MantenimientoSeguimientoId { get; set; }

    public int MantenimientoId { get; set; }

    public int EstatusMantenimientoId { get; set; }

    public string? Observaciones { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public virtual EstatusMantenimiento EstatusMantenimiento { get; set; } = null!;

    public virtual Mantenimiento Mantenimiento { get; set; } = null!;
}
