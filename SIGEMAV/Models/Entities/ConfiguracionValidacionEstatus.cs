using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class ConfiguracionValidacionEstatus
{
    public int ConfiguracionValidacionEstatusId { get; set; }

    public int ConfiguracionValidacionTecnicaId { get; set; }

    public int EstatusSolicitudMantenimientoId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaModificacion { get; set; }

    public virtual ConfiguracionValidacionTecnica ConfiguracionValidacionTecnica { get; set; } = null!;

    public virtual EstatusSolicitudMantenimiento EstatusSolicitudMantenimiento { get; set; } = null!;
}
