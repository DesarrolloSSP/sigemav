using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class TransferenciaUnidadSeguimiento
{
    public int TransferenciaUnidadSeguimientoId { get; set; }

    public int TransferenciaUnidadId { get; set; }

    public int EstatusTransferenciaUnidadId { get; set; }

    public string? Observaciones { get; set; }

    public int UsuarioId { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public DateTime FechaCreacion { get; set; }

    public bool Activo { get; set; }

    public string? TipoMovimiento { get; set; }

    public string? DocumentoRuta { get; set; }

    public string? DocumentoNombreOriginal { get; set; }

    public string? HashDocumento { get; set; }

    public string? IpRegistro { get; set; }

    public string? EquipoRegistro { get; set; }

    public virtual EstatusTransferenciaUnidad EstatusTransferenciaUnidad { get; set; } = null!;

    public virtual TransferenciaUnidad TransferenciaUnidad { get; set; } = null!;
}
