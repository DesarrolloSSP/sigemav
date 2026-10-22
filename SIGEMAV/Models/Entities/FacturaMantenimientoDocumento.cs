using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class FacturaMantenimientoDocumento
{
    public int FacturaDocumentoId { get; set; }

    public int? FacturaMantenimientoId { get; set; }

    public string? TipoDocumento { get; set; }

    public string? NombreArchivo { get; set; }

    public string? RutaArchivo { get; set; }

    public string? HashArchivo { get; set; }

    public DateTime? FechaCarga { get; set; }

    public int? UsuarioCargaId { get; set; }

    public bool? Activo { get; set; }

    public virtual FacturaMantenimiento? FacturaMantenimiento { get; set; }
}
