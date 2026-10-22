using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SIGEMAV.Models.ViewModels.Maquinaria
{
    public class TallerCreateViewModel
    {
        public int TallerId { get; set; }


        // =====================================================
        // DATOS GENERALES
        // =====================================================

        [Required(
            ErrorMessage = "La razón social es obligatoria.")]
        [StringLength(
            100,
            ErrorMessage = "La razón social no puede exceder 100 caracteres.")]
        [Display(Name = "Razón social")]
        public string RazonSocial { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "El RFC es obligatorio.")]
        [StringLength(
            13,
            ErrorMessage = "El RFC no puede exceder 13 caracteres.")]
        [Display(Name = "RFC")]
        public string Rfc { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "Debe seleccionar un recurso.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debe seleccionar un recurso válido.")]
        [Display(Name = "Recurso")]
        public int RecursoId { get; set; }


        [Required(
            ErrorMessage = "El contrato es obligatorio.")]
        [StringLength(
            15,
            ErrorMessage = "El contrato no puede exceder 15 caracteres.")]
        [Display(Name = "Contrato")]
        public string Contrato { get; set; } = string.Empty;


        [Required(
            ErrorMessage = "Debe seleccionar un banco.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debe seleccionar un banco válido.")]
        [Display(Name = "Banco")]
        public int BancoId { get; set; }


        // =====================================================
        // FIANZA
        // =====================================================

        [Required(
            ErrorMessage = "La fianza es obligatoria.")]
        [StringLength(
            25,
            ErrorMessage = "La fianza no puede exceder 25 caracteres.")]
        [Display(Name = "Fianza")]
        public string Fianza { get; set; } = string.Empty;


        // =====================================================
        // CLABE
        // =====================================================

        [Required(
            ErrorMessage = "La CLABE interbancaria es obligatoria.")]
        [StringLength(
            18,
            MinimumLength = 18,
            ErrorMessage = "La CLABE debe contener exactamente 18 dígitos.")]
        [RegularExpression(
            @"^\d{18}$",
            ErrorMessage = "La CLABE debe contener exactamente 18 dígitos.")]
        [Display(Name = "CLABE interbancaria")]
        public string Clabe { get; set; } = string.Empty;


        // =====================================================
        // OBJETIVO DE MANTENIMIENTO
        // =====================================================

        [Required(
            ErrorMessage = "Debe seleccionar un objetivo de mantenimiento.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "Debe seleccionar un objetivo de mantenimiento válido.")]
        [Display(Name = "Objetivo de mantenimiento")]
        public int ObjetivoMantenimientoId { get; set; }


        // =====================================================
        // UBICACIÓN
        // =====================================================

        [Required(
            ErrorMessage = "La ubicación es obligatoria.")]
        [StringLength(
            150,
            ErrorMessage = "La ubicación no puede exceder 150 caracteres.")]
        [Display(Name = "Ubicación")]
        public string Ubicacion { get; set; } = string.Empty;


        // =====================================================
        // DATOS PRESUPUESTALES
        // =====================================================

        [Required(
            ErrorMessage = "El monto mínimo es obligatorio.")]
        [Range(
            0,
            double.MaxValue,
            ErrorMessage = "El monto mínimo debe ser mayor o igual a cero.")]
        [Display(Name = "Monto mínimo")]
        public decimal? MontoMinimo { get; set; }


        [Required(
            ErrorMessage = "El monto máximo es obligatorio.")]
        [Range(
            0,
            double.MaxValue,
            ErrorMessage = "El monto máximo debe ser mayor o igual a cero.")]
        [Display(Name = "Monto máximo")]
        public decimal? MontoMaximo { get; set; }


        // =====================================================
        // ALERTA PREVENTIVA
        // =====================================================

        [Required(
            ErrorMessage = "El porcentaje de alerta es obligatorio.")]
        [Range(
            0.01,
            100,
            ErrorMessage = "El porcentaje de alerta debe estar entre 0.01 y 100.")]
        [Display(Name = "Porcentaje de alerta")]
        public decimal? PorcentajeAlerta { get; set; }


        [Display(Name = "Monto de alerta")]
        public decimal? MontoAlerta { get; set; }


        // =====================================================
        // ALERTA CRÍTICA
        // =====================================================

        [Required(
            ErrorMessage = "El porcentaje crítico es obligatorio.")]
        [Range(
            0.01,
            100,
            ErrorMessage = "El porcentaje crítico debe estar entre 0.01 y 100.")]
        [Display(Name = "Porcentaje crítico")]
        public decimal? PorcentajeCritico { get; set; }


        [Display(Name = "Monto crítico")]
        public decimal? MontoCritico { get; set; }


        // =====================================================
        // ESTADO
        // =====================================================

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;


        // =====================================================
        // CATÁLOGOS
        // =====================================================

        public IEnumerable<SelectListItem> Recursos { get; set; }
            = Enumerable.Empty<SelectListItem>();


        public IEnumerable<SelectListItem> Bancos { get; set; }
            = Enumerable.Empty<SelectListItem>();


        public IEnumerable<SelectListItem> ObjetivosMantenimiento { get; set; }
            = Enumerable.Empty<SelectListItem>();
    }
}
