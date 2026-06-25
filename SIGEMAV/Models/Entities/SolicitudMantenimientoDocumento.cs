using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class SolicitudMantenimientoDocumento
{
    public int SolicitudMantenimientoDocumentoId { get; set; }

    public int SolicitudMantenimientoId { get; set; }

    public string TipoDocumento { get; set; } = null!;

    public string NombreArchivo { get; set; } = null!;

    public string RutaArchivo { get; set; } = null!;

    public DateTime FechaCarga { get; set; }

    public int? UsuarioCargaId { get; set; }

    public bool Activo { get; set; }

    public string? HashArchivo { get; set; }

    public virtual SolicitudMantenimiento SolicitudMantenimiento { get; set; } = null!;
}
