using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class ConceptoServicioCosto
{
    public int ConceptoServicioCostoId { get; set; }

    public int ConceptoServicioId { get; set; }

    public decimal CostoManoObra { get; set; }

    public decimal CostoRefacciones { get; set; }

    public DateOnly FechaInicioVigencia { get; set; }

    public DateOnly? FechaFinVigencia { get; set; }

    public string? Observaciones { get; set; }

    public bool Activo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public int MarcaId { get; set; }

    public int ModeloId { get; set; }

    public int AnioId { get; set; }

    public int TransmisionId { get; set; }

    public int CilindroId { get; set; }

    public virtual Anio Anio { get; set; } = null!;

    public virtual Cilindro Cilindro { get; set; } = null!;

    public virtual ConceptoServicio ConceptoServicio { get; set; } = null!;

    public virtual Marca Marca { get; set; } = null!;

    public virtual Modelo Modelo { get; set; } = null!;

    public virtual Transmision Transmision { get; set; } = null!;
}
