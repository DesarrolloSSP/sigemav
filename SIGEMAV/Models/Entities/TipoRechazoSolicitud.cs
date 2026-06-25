using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class TipoRechazoSolicitud
{
    public int TipoRechazoSolicitudId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool PermiteCorreccion { get; set; }

    public bool RequiereAutorizacionSuperior { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<SolicitudMantenimientoSeguimiento> SolicitudMantenimientoSeguimientos { get; set; } = new List<SolicitudMantenimientoSeguimiento>();
}
