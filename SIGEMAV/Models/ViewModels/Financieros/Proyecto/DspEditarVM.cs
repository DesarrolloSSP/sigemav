using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIGEMAV.Models.ViewModels.Financieros.Proyecto
{
    public class DispEditarVM
    {

        public int IdDatosDsp { get; set; }


        [Required(ErrorMessage =
            "Seleccione el objeto del gasto")]
        [Display(Name = "Objeto del gasto")]
        public int? IdObjetoGasto { get; set; }


        [Required(ErrorMessage =
            "Seleccione la clave administrativa")]
        [Display(Name = "Administrativa")]
        public int? IdClaveAdmin { get; set; }


        [Required(ErrorMessage =
            "Seleccione el proyecto")]
        [Display(Name = "Proceso proyecto")]
        public int? IdProyecto { get; set; }


        [Display(Name =
            "Descripción del objeto del gasto")]
        public string? DescripcionObjetoGasto
        {
            get; set;
        }


        [Required(ErrorMessage =
            "Seleccione el área usuaria")]
        [Display(Name = "Área usuaria")]
        public int? IdArea { get; set; }


        [Required(ErrorMessage =
            "Capture el importe")]

        [Range(0.01, 20000000,
            ErrorMessage =
            "El importe no puede exceder 20 millones")]

        [Display(Name = "Importe")]
        public decimal? Importe { get; set; }


        [Required(ErrorMessage = "Seleccione No. Dsp")]
        [Display(Name = "Dsp")]
        public int IDsp { get; set; }


        public bool Activo { get; set; }

        // COMBOS

        public List<SelectListItem> ObjetosGasto { get; set; } = new();

        public List<SelectListItem> ClavesAdministrativas { get; set; } = new();

        public List<SelectListItem> Proyectos { get; set; } = new();

        public List<SelectListItem> AreasUsuarias { get; set; } = new();

        public List<SelectListItem> ListadoDsp { get; set; } = new();

    }


}



