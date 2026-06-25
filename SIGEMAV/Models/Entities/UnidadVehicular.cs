using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class UnidadVehicular
{
    public int UnidadVehicularId { get; set; }

    public string NumeroSerie { get; set; } = null!;

    public string PlacaActual { get; set; } = null!;

    public string NumeroEconomico { get; set; } = null!;

    public int MarcaId { get; set; }

    public int ModeloId { get; set; }

    public int AnioId { get; set; }

    public int ColorId { get; set; }

    public int MunicipioId { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int? AreaId { get; set; }

    public int? CilindroId { get; set; }

    public int? TransmisionId { get; set; }

    public string? Observaciones { get; set; }

    public virtual Anio Anio { get; set; } = null!;

    public virtual Area? Area { get; set; }

    public virtual Cilindro? Cilindro { get; set; }

    public virtual Color Color { get; set; } = null!;

    public virtual ICollection<Mantenimiento> Mantenimientos { get; set; } = new List<Mantenimiento>();

    public virtual Marca Marca { get; set; } = null!;

    public virtual Modelo Modelo { get; set; } = null!;

    public virtual Municipio Municipio { get; set; } = null!;

    public virtual ICollection<SolicitudMantenimiento> SolicitudMantenimientos { get; set; } = new List<SolicitudMantenimiento>();

    public virtual ICollection<TransferenciaUnidad> TransferenciaUnidads { get; set; } = new List<TransferenciaUnidad>();

    public virtual Transmision? Transmision { get; set; }
}
