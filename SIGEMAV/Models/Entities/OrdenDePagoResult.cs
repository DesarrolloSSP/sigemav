using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SIGEMAV.Models.Entities
{

    [Keyless]

    public class OrdenDePagoResult
    {
        // Datos de la solicitud
        public string NumeroOficio { get; set; } = string.Empty;

        // Clasificación presupuestal
        public string Partida { get; set; } = string.Empty;

        public int IdObjetoGasto { get; set; }
        public string ClaveObjGasto { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        // Identificadores
        public int FacturaMantenimientoId { get; set; }
        public int FacturaDetalleId { get; set; }
        public int MantenimientoDetalleId { get; set; }

        // Datos fiscales de la factura
        public string? UUID { get; set; }
        public string? Serie { get; set; }
        public string Folio { get; set; } = string.Empty;
        public DateTime FechaFactura { get; set; }

        // Datos del proveedor
        public string RFCEmisor { get; set; } = string.Empty;
        public string RazonSocial { get; set; } = string.Empty;
        public string ClabeInterbancaria  { get; set; } = string.Empty;
        public int Cantidad { get; set; } = 0;

        // Información del vehículo y servicio
        public string? PlacaActual { get; set; }
        public string? ConceptoServicioNombre { get; set; }

        // Importe facturado
        public string Origen { get; set; } = string.Empty;
        public decimal Importe { get; set; }

    }

}
