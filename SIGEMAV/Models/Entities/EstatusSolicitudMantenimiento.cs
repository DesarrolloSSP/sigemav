using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class EstatusSolicitudMantenimiento
{
    public int EstatusSolicitudMantenimientoId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Color { get; set; }

    public int Orden { get; set; }

    public bool Activo { get; set; }

    public virtual ICollection<ConfiguracionValidacionEstatus> ConfiguracionValidacionEstatuses { get; set; } = new List<ConfiguracionValidacionEstatus>();

    public virtual ICollection<SolicitudMantenimientoSeguimiento> SolicitudMantenimientoSeguimientos { get; set; } = new List<SolicitudMantenimientoSeguimiento>();

    public virtual ICollection<SolicitudMantenimiento> SolicitudMantenimientos { get; set; } = new List<SolicitudMantenimiento>();
}
