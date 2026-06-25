using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class SolicitudMantenimientoSeguimiento
{
    public int SolicitudMantenimientoSeguimientoId { get; set; }

    public int SolicitudMantenimientoId { get; set; }

    public int EstatusSolicitudMantenimientoId { get; set; }

    public string? Observaciones { get; set; }

    public int? UsuarioId { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public int? TipoRechazoSolicitudId { get; set; }

    public virtual EstatusSolicitudMantenimiento EstatusSolicitudMantenimiento { get; set; } = null!;

    public virtual SolicitudMantenimiento SolicitudMantenimiento { get; set; } = null!;

    public virtual TipoRechazoSolicitud? TipoRechazoSolicitud { get; set; }
}
