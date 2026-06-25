using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Financieros.Dsp
{
    public class DspEditarVM
    {
        public int IdDsp { get; set; }

        [Required(ErrorMessage = "La fecha es requerida")]
        public DateOnly? Fecha { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Capture el importe")]
        [Display(Name = "Importe")]
        public decimal? Importe { get; set; }

        public string? RutaArchivo { get; set; }

        // Opcional en edición
        [Display(Name = "Archivo PDF")]
        public IFormFile? Archivo { get; set; }

        [Required(ErrorMessage = "Capture el No DSP")]
        [StringLength(30, ErrorMessage = "Máximo 30 caracteres")]
        [Display(Name = "No DSP")]
        public string? NoDsp { get; set; }

        public bool Activo { get; set; }

    }


}
