using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class EstadoOdometro
{
    public byte EstadoOdometroId { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<SolicitudMantenimiento> SolicitudMantenimientos { get; set; } = new List<SolicitudMantenimiento>();
}
