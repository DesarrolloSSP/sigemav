using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Financieros;
using SIGEMAV.Models.ViewModels.Financieros.Disponibilidad;
using SIGEMAV.Models.ViewModels.Financieros.Dsp;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Services.Implementations.Financieros
{
    public class DispService : IDispService
    {

        private readonly BdSigeMavContext _context;

        public DispService(BdSigeMavContext context)
        {
            _context = context;
        }


        public async Task<List<ObjetoGastoVM>> ObtenerTipoGasto()
        {
            var data = await _context.ObjetoGastos
                .Select(x => new ObjetoGastoVM
                {
                    IdObjetoGasto = x.IdObjetoGasto,
                    ClaveObjGasto = x.ClaveObjGasto,
                    Descripcion = x.Descripcion
                }).ToListAsync();

            return data;
        }



        public async Task<List<CatalogoDsp>> ObtenerListadoDsp()
        {

            var data = await _context.Dsps
                .Select(x => new CatalogoDsp
                {
                    IdDsp = x.IdDsp,
                    NoDsp = x.NoDsp

                }).ToListAsync();

            return data;

        }



        public async Task<List<ClaveAdministrativaVM>> ObtenerClaveAdministrativa()
        {
            var data = await _context.ClaveAdministrativas.Select(x => new ClaveAdministrativaVM
            {
                IdClaveAdmin = x.IdClaveAdmin,
                ClaveAdmin = x.ClaveAdmin
            }).ToListAsync();

            return data;
        }


        public async Task<List<ProyectoVM>> ObtenerProyecto(int idClaveAdmin)
        {
            var data = await _context.Proyectos
               .Where(x => x.IdClaveAdmin == idClaveAdmin)
               .Select(x => new ProyectoVM
               {
                   IdProyecto = x.IdProyecto,
                   ClaveProyecto = x.ClaveProyecto
               })
               .ToListAsync();
            return data;
        }


        public async Task<List<AreasVM>> ObtenerArea()
        {
            var data = await _context.Areas.OrderBy(x => x.AreaNombre)
                .Select(x => new AreasVM
                {
                    AreaId = x.AreaId,
                    AreaNombre = x.AreaNombre

                }).ToListAsync();

            return data;
        }



        public async Task<bool> GuardarDisp(DispCrearVM model)
        {
            try
            {
                if (model == null)
                {
                    return false;
                }

                bool existe = await _context.DispPresupuestals
                    .AnyAsync(x =>
                        x.IdDsp == model.IDsp &&
                        x.IdObjetoGasto == model.IdObjetoGasto &&
                        x.IdClaveAdmin == model.IdClaveAdmin &&
                        x.IdProyecto == model.IdProyecto &&
                        x.IdArea == model.IdArea);

                if (existe)
                {
                    return false;
                }

                DispPresupuestal dsp = new()
                {
                    IdObjetoGasto = model.IdObjetoGasto!.Value,
                    IdClaveAdmin = model.IdClaveAdmin!.Value,
                    IdProyecto = model.IdProyecto!.Value,
                    IdArea = model.IdArea!.Value,
                    Importe = model.Importe!.Value,
                    IdDsp = model.IDsp!.Value,
                    Activo = true
                };

                _context.DispPresupuestals.Add(dsp);

                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }


        public async Task<List<DispListadoVM>> ObtenerDsp()
        {
            var data = await (
                from disp in _context.DispPresupuestals

                join obj in _context.ObjetoGastos
                    on disp.IdObjetoGasto equals obj.IdObjetoGasto

                join adm in _context.ClaveAdministrativas
                    on disp.IdClaveAdmin equals adm.IdClaveAdmin

                join proy in _context.Proyectos
                    on disp.IdProyecto equals proy.IdProyecto

                join area in _context.Areas
                    on disp.IdArea equals area.AreaId

                join dsp in _context.Dsps
                    on disp.IdDsp equals dsp.IdDsp


                select new DispListadoVM
                {
                    IdDatosDsp = disp.IdDispPresupuestal,

                    NoDsp = dsp.NoDsp,
                    ObjetoGasto =
                        obj.ClaveObjGasto + " - " + obj.Descripcion,

                    ClaveAdmin = adm.ClaveAdmin,

                    Proyecto = proy.ClaveProyecto,

                    Area = area.AreaNombre,

                    Importe = disp.Importe,

                    Disponible = disp.Importe - (_context.MovimientoPresupuestals.Where(x =>
                        x.IdDispPresupuestal == disp.IdDispPresupuestal &&
                        x.TipoMovimiento == "COMPROMETIDO" &&
                        x.Observaciones == null)
                    .Sum(x => (decimal?)x.Importe) ?? 0
                ),


                    Activo = disp.Activo
                }
            ).ToListAsync();

            return data;
        }

        public async Task<List<AreasVM>> ObtenerAreasPorProyecto(int idProyecto)
        {
            return await _context.ProyectoAreas

                .Where(x =>
                    x.IdProyecto == idProyecto &&
                    x.Activo)

                .Include(x => x.IdAreaNavigation)

                .Select(x => new AreasVM
                {
                    AreaId =
                        x.IdAreaNavigation.AreaId,

                    AreaNombre =
                        x.IdAreaNavigation.AreaNombre
                })

                .ToListAsync();
        }


        public async Task<DispEditarVM?> ObtenerPorId(int id)
        {
            var entity =
                await _context.DispPresupuestals
                    .FirstOrDefaultAsync(x =>
                        x.IdDispPresupuestal == id);

            if (entity == null)
            {
                return null;
            }

            return new DispEditarVM
            {
                IdDatosDsp = entity.IdDispPresupuestal,

                IdObjetoGasto = entity.IdObjetoGasto,

                IdClaveAdmin = entity.IdClaveAdmin,

                IdProyecto = entity.IdProyecto,

                IdArea = entity.IdArea,

                Importe = entity.Importe,

                IDsp = entity.IdDsp,

                Activo = entity.Activo
            };
        }



        public async Task ActualizarDsp(DispEditarVM model, string? rutaArchivo)
        {
            var dsp =
                await _context.DispPresupuestals

                .FirstOrDefaultAsync(x =>
                    x.IdDispPresupuestal ==
                    model.IdDatosDsp);

            if (dsp == null)
            {
                throw new Exception(
                    "Registro no encontrado");
            }


            dsp.IdObjetoGasto =
                model.IdObjetoGasto!.Value;

            dsp.IdClaveAdmin =
                model.IdClaveAdmin!.Value;

            dsp.IdProyecto =
                model.IdProyecto!.Value;

            dsp.IdArea =
                model.IdArea!.Value;

            dsp.Importe =
                model.Importe!.Value;

            //dsp.NoDsp =
            //    model.NoDsp!;

            dsp.Activo =
                model.Activo;


            // SOLO SI SUBE NUEVO PDF
            //if (!string.IsNullOrEmpty(rutaArchivo))
            //{
            //    dsp.RutaArchivo =
            //        rutaArchivo;
            //}

            await _context.SaveChangesAsync();
        }

        public async Task Activar(int id)
        {
            var entity =
                await _context.DispPresupuestals

                .FirstOrDefaultAsync(x =>
                    x.IdDispPresupuestal == id);

            entity!.Activo = true;

            await _context.SaveChangesAsync();
        }



        // DESACTIVAR

        public async Task Desactivar(int id)
        {
            var entity =
                await _context.DispPresupuestals

                .FirstOrDefaultAsync(x =>
                    x.IdDispPresupuestal == id);

            entity!.Activo = false;

            await _context.SaveChangesAsync();
        }





        Task<IActionResult> IDispService.Activar(int id)
        {
            throw new NotImplementedException();
        }

        Task<IActionResult> IDispService.Desactivar(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> Editar(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IActionResult> VerPdf(int id)
        {
            throw new NotImplementedException();
        }


    }

}
