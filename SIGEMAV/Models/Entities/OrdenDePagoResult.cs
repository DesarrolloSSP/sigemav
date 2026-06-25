using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SIGEMAV.Models.Entities
{

    [Keyless]
    public class OrdenDePagoResult
    {

        public string Folio { get; set; }

        public int MantenimientoId { get; set; }

        public int MantenimientoDetalleId { get; set; }

        public int? IdObjetoGasto { get; set; }

        public string ClaveObjGasto { get; set; }        

        public string? Descripcion { get; set; }

        public decimal? CostoUnitario { get; set; }

        public decimal? Subtotal { get; set; }

        public string Tipo { get; set; }

        public string RFC { get; set; }

        public string RazonSocial { get; set; } = string.Empty;
        public DateTime? FechaSolicitud { get; set; }
    }
}
