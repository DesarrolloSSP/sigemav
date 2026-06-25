using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class OrdenPago
{
    public int IdOrdenPago { get; set; }

    public string? NumeroOrden { get; set; }

    public DateOnly? Fecha { get; set; }

    public int? ProovedorId { get; set; }

    public decimal? ImporteTotal { get; set; }

    public string? Estatus { get; set; }

    public string? RutaArchivo { get; set; }

    public DateTime? FechaCaptura { get; set; }

    public string? UsuarioCaptura { get; set; }

    public virtual ICollection<OrdenPagoDetalle> OrdenPagoDetalles { get; set; } = new List<OrdenPagoDetalle>();

    public virtual Taller? Proovedor { get; set; }
}
