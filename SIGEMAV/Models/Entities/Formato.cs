using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Formato
{
    public int IdFormato { get; set; }

    public string Nombre { get; set; } = null!;

    public string RutaArchivo { get; set; } = null!;

    public DateTime FechaCreacion { get; set; }

    public bool Activo { get; set; }

    public int IdArea { get; set; }

    public virtual Area IdAreaNavigation { get; set; } = null!;
}
