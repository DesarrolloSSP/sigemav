using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TallerEditViewModel
    {
        // =========================================================
        // IDENTIFICADOR
        // =========================================================

        public int TallerId { get; set; }


        // =========================================================
        // DATOS DEL TALLER
        // =========================================================

        [Required(
            ErrorMessage = "La razón social es obligatoria.")]
        [StringLength(
            200,
            ErrorMessage = "La razón social no puede exceder los 200 caracteres.")]
        [Display(Name = "Razón social")]
        public string RazonSocial { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "El RFC es obligatorio.")]
        [StringLength(
            13,
            MinimumLength = 12,
            ErrorMessage = "El RFC debe contener 12 o 13 caracteres.")]
        [RegularExpression(
            @"^[A-Za-zÑñ&]{3,4}\d{6}[A-Za-z0-9]{3}$",
            ErrorMessage = "El RFC debe tener un formato válido.")]
        [Display(Name = "RFC")]
        public string Rfc { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "Debe seleccionar un banco.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debe seleccionar un banco válido.")]
        [Display(Name = "Banco")]
        public int BancoId { get; set; }


        [Required(
            ErrorMessage = "La fianza es obligatoria.")]
        [StringLength(
            50,
            ErrorMessage = "La fianza no puede exceder los 50 caracteres.")]
        [Display(Name = "Fianza")]
        public string Fianza { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "La CLABE es obligatoria.")]
        [StringLength(
            18,
            MinimumLength = 18,
            ErrorMessage = "La CLABE debe contener exactamente 18 dígitos.")]
        [RegularExpression(
            @"^\d{18}$",
            ErrorMessage = "La CLABE debe contener exactamente 18 dígitos.")]
        [Display(Name = "CLABE")]
        public string Clabe { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "Debe seleccionar el objetivo de mantenimiento.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debe seleccionar un objetivo de mantenimiento válido.")]
        [Display(Name = "Objetivo de mantenimiento")]
        public int ObjetivoMantenimientoId { get; set; }


        [Required(
            ErrorMessage = "La ubicación es obligatoria.")]
        [StringLength(
            300,
            ErrorMessage = "La ubicación no puede exceder los 300 caracteres.")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; } = string.Empty;


        // =========================================================
        // ESTADO
        // =========================================================

        [Display(Name = "Activo")]
        public bool Activo { get; set; }


        // =========================================================
        // INFORMACIÓN DE CONFIGURACIÓN PRESUPUESTAL
        // =========================================================
        // SOLO CONSULTA.
        //
        // Estos datos NO se editan desde "Editar Taller".
        //
        // La configuración pertenece a:
        // Contrato + Recurso
        //
        // y puede estar compartida por varias ubicaciones.
        // =========================================================

        [Display(Name = "Recurso")]
        public string RecursoNombre { get; set; } = string.Empty;


        [Display(Name = "Contrato")]
        public string Contrato { get; set; } = string.Empty;


        [Display(Name = "Monto mínimo")]
        public decimal? MontoMinimo { get; set; }


        [Display(Name = "Monto máximo")]
        public decimal? MontoMaximo { get; set; }


        [Display(Name = "Porcentaje de alerta")]
        public decimal? PorcentajeAlerta { get; set; }


        [Display(Name = "Monto de alerta")]
        public decimal? MontoAlerta { get; set; }


        [Display(Name = "Porcentaje crítico")]
        public decimal? PorcentajeCritico { get; set; }


        [Display(Name = "Monto crítico")]
        public decimal? MontoCritico { get; set; }


        // =========================================================
        // CATÁLOGOS
        // =========================================================

        public IEnumerable<SelectListItem> Bancos { get; set; }
            = Enumerable.Empty<SelectListItem>();


        public IEnumerable<SelectListItem> ObjetivosMantenimiento { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}
