using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Firmante
{
    public int FirmanteId { get; set; }

    public string ClavePuesto { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public string Cargo { get; set; } = null!;

    public int? AreaId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public virtual Area? Area { get; set; }
}
