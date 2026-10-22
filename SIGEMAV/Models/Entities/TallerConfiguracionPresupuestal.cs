using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class TallerConfiguracionPresupuestal
{
    public int TallerConfiguracionPresupuestalId { get; set; }

    public string Contrato { get; set; } = null!;

    public int RecursoId { get; set; }

    public decimal MontoMinimo { get; set; }

    public decimal MontoMaximo { get; set; }

    public decimal? PorcentajeAlerta { get; set; }

    public decimal? MontoAlerta { get; set; }

    public decimal? PorcentajeCritico { get; set; }

    public decimal? MontoCritico { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public DateTime? FechaActualizacion { get; set; }

    public virtual Recurso Recurso { get; set; } = null!;

    public virtual ICollection<SolicitudMantenimiento> SolicitudMantenimientos { get; set; } = new List<SolicitudMantenimiento>();

    public virtual ICollection<TallerConfiguracion> TallerConfiguracions { get; set; } = new List<TallerConfiguracion>();

    public virtual ICollection<TallerControlPresupuestal> TallerControlPresupuestals { get; set; } = new List<TallerControlPresupuestal>();
}
