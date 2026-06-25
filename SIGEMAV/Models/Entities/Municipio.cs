using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Municipio
{
    public int MunicipioId { get; set; }

    public string? Nombre { get; set; }

    public string? CveMun { get; set; }

    public int? CveZona { get; set; }

    public string? Zona { get; set; }

    public int? CveDeleg { get; set; }

    public string? Delegacion { get; set; }

    public bool? TcmunicipioAlerta { get; set; }

    public bool? TcmunicipioIndigena { get; set; }

    public string? TcmunicipioRegion { get; set; }

    public bool? TcmunicipioCpdh { get; set; }

    public int UnidadVehicularId { get; set; }

    public virtual ICollection<UnidadVehicular> UnidadVehiculars { get; set; } = new List<UnidadVehicular>();
}
