using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class TipoMantenimiento
{
    public int TipoMantenimientoId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<SolicitudMantenimiento> SolicitudMantenimientos { get; set; } = new List<SolicitudMantenimiento>();
}
