using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class MantenimientoDocumento
{
    public int MantenimientoDocumentoId { get; set; }

    public int MantenimientoId { get; set; }

    public string TipoDocumento { get; set; } = null!;

    public string NombreArchivo { get; set; } = null!;

    public string RutaArchivo { get; set; } = null!;

    public string? HashArchivo { get; set; }

    public int UsuarioCargaId { get; set; }

    public DateTime FechaCarga { get; set; }

    public bool Activo { get; set; }

    public virtual Mantenimiento Mantenimiento { get; set; } = null!;
}
