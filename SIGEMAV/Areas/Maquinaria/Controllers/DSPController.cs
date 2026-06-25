using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.ViewModels.Maquinaria;
using SIGEMAV.Services.Interfaces.Maquinaria;
using static SIGEMAV.Areas.Maquinaria.DTOs.DTOs;

namespace SIGEMAV.Areas.Maquinaria.Controllers
{
    [Area("Maquinaria")]
    [Authorize(Roles = "AdminMaquinaria,Sysadmin")]
    public class DSPController : Controller
    {
        private readonly IMaquinariaService _maquinariaService;
        private readonly BdSigeMavContext _context;


        public DSPController(IMaquinariaService maquinariaService, BdSigeMavContext context)
        {
            _maquinariaService = maquinariaService;
            _context = context;
        }




       





        // GET: DSPController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DSPController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DSPController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DSPController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DSPController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DSPController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DSPController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
