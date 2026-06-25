using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class LogAcceso
{
    public int Id { get; set; }

    public string? Modulo { get; set; }

    public DateTime? Fecha { get; set; }

    public string? Ip { get; set; }

    public string? Usuario { get; set; }

    public string? Equipo { get; set; }

    public string? SessionId { get; set; }
}
