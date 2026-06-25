using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Taller
{
    public int TallerId { get; set; }

    public string RazonSocial { get; set; } = null!;

    public string Rfc { get; set; } = null!;

    public int RecursoId { get; set; }

    public decimal MontoMinimo { get; set; }

    public string Contrato { get; set; } = null!;

    public int BancoId { get; set; }

    public string Ubicacion { get; set; } = null!;

    public bool Activo { get; set; }

    public decimal? MontoMaximo { get; set; }

    public virtual Banco Banco { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual ICollection<OrdenPago> OrdenPagos { get; set; } = new List<OrdenPago>();

    public virtual Recurso Recurso { get; set; } = null!;

    public virtual ICollection<SolicitudMantenimiento> SolicitudMantenimientos { get; set; } = new List<SolicitudMantenimiento>();

    public virtual ICollection<TallerControlPresupuestal> TallerControlPresupuestals { get; set; } = new List<TallerControlPresupuestal>();

    public virtual ICollection<TallerMovimientoPresupuesto> TallerMovimientoPresupuestos { get; set; } = new List<TallerMovimientoPresupuesto>();
}
