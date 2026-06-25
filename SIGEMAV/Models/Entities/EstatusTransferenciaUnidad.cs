using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class EstatusTransferenciaUnidad
{
    public int EstatusTransferenciaUnidadId { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }

    public virtual ICollection<TransferenciaUnidadSeguimiento> TransferenciaUnidadSeguimientos { get; set; } = new List<TransferenciaUnidadSeguimiento>();

    public virtual ICollection<TransferenciaUnidad> TransferenciaUnidads { get; set; } = new List<TransferenciaUnidad>();
}
