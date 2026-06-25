using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class TransferenciaUnidad
{
    public int TransferenciaUnidadId { get; set; }

    public string Folio { get; set; } = null!;

    public int UnidadVehicularId { get; set; }

    public int AreaOrigenId { get; set; }

    public int AreaDestinoId { get; set; }

    public int EstatusTransferenciaUnidadId { get; set; }

    public DateTime FechaSolicitud { get; set; }

    public DateTime? FechaRespuesta { get; set; }

    public string? ObservacionesEntrega { get; set; }

    public string? ObservacionesRecepcion { get; set; }

    public string DocumentoRuta { get; set; } = null!;

    public string DocumentoNombreOriginal { get; set; } = null!;

    public int UsuarioEntregaId { get; set; }

    public int? UsuarioRecibeId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public string MotivoTransferencia { get; set; } = null!;

    public string? ComentariosAutorizacion { get; set; }

    public DateTime? FechaEntrega { get; set; }

    public DateTime? FechaRecepcion { get; set; }

    public DateTime? FechaCancelacion { get; set; }

    public int? UsuarioSolicitaId { get; set; }

    public int? UsuarioAutorizaId { get; set; }

    public DateTime? FechaTransferencia { get; set; }

    public string? Observaciones { get; set; }

    public virtual Area AreaDestino { get; set; } = null!;

    public virtual Area AreaOrigen { get; set; } = null!;

    public virtual EstatusTransferenciaUnidad EstatusTransferenciaUnidad { get; set; } = null!;

    public virtual ICollection<TransferenciaUnidadSeguimiento> TransferenciaUnidadSeguimientos { get; set; } = new List<TransferenciaUnidadSeguimiento>();

    public virtual UnidadVehicular UnidadVehicular { get; set; } = null!;
}
