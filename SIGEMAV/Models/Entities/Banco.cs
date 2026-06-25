using System;
using System.Collections.Generic;

namespace SIGEMAV.Models.Entities;

public partial class Banco
{
    public int BancoId { get; set; }

    public string Nombre { get; set; } = null!;

    public int FianzaId { get; set; }

    public bool Activo { get; set; }

    public virtual Fianza Fianza { get; set; } = null!;

    public virtual ICollection<Taller> Tallers { get; set; } = new List<Taller>();
}
