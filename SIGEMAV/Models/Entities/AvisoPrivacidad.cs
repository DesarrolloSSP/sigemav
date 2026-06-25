using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class AvisoPrivacidad
{
    public int IdAvisoPrivacidad { get; set; }

    public string Area { get; set; } = null!;

    public string AvisoIntegralUrl { get; set; } = null!;

    public string AvisoSimplificadoUrl { get; set; } = null!;

    public string SistemaDatosUrl { get; set; } = null!;

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }
}
