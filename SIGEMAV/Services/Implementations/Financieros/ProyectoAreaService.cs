using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Services.Implementations.Financieros
{
    public class ProyectoAreaService : IProyectoAreaService
    {
        private readonly BdSigeMavContext _context;

        public ProyectoAreaService(
            BdSigeMavContext context)
        {
            _context = context;
        }


        public async Task<List<ProyectoAreaCrudVM>> Obtener()
        {
            return await _context.ProyectoAreas

                .Include(x => x.IdProyectoNavigation)
                .Include(x => x.IdAreaNavigation)

                .OrderBy(x =>
                    x.IdProyectoNavigation.ClaveProyecto)

                .Select(x => new ProyectoAreaCrudVM
                {
                    IdProyectoArea =
                        x.IdProyectoArea,

                    IdProyecto =
                        x.IdProyecto,

                    IdArea =
                        x.IdArea,

                    Proyecto =
                        x.IdProyectoNavigation
                            .ClaveProyecto,

                    Area =
                        x.IdAreaNavigation.AreaNombre,

                    Activo =
                        x.Activo
                })

                .ToListAsync();
        }



        public async Task<ProyectoAreaCrudVM?>
            ObtenerPorId(int id)
        {
            return await _context.ProyectoAreas

                .Where(x =>
                    x.IdProyectoArea == id)

                .Select(x => new ProyectoAreaCrudVM
                {
                    IdProyectoArea =
                        x.IdProyectoArea,

                    IdProyecto =
                        x.IdProyecto,

                    IdArea =
                        x.IdArea,

                    Activo =
                        x.Activo
                })

                .FirstOrDefaultAsync();
        }



        public async Task Guardar(
            ProyectoAreaCrudVM model)
        {
            ProyectoArea entity =
                new ProyectoArea
                {
                    IdProyecto =
                        model.IdProyecto!.Value,

                    IdArea =
                        model.IdArea!.Value,

                    Activo =
                        model.Activo,

                    FechaCreacion =
                        DateTime.Now
                };

            _context.ProyectoAreas
                .Add(entity);

            await _context.SaveChangesAsync();
        }


        public async Task Actualizar(ProyectoAreaCrudVM model)
        {
            var entity = await _context.ProyectoAreas

                .FirstOrDefaultAsync(x =>
                    x.IdProyectoArea ==
                    model.IdProyectoArea);

            if (entity == null)
            {
                throw new Exception(
                    "Registro no encontrado");
            }

            entity.IdProyecto =
                model.IdProyecto!.Value;

            entity.IdArea =
                model.IdArea!.Value;

            entity.Activo =
                model.Activo;

            await _context.SaveChangesAsync();
        }


        public async Task Desactivar(int id)
        {
            var entity =
                await _context.ProyectoAreas

                .FirstOrDefaultAsync(x =>
                    x.IdProyectoArea == id);

            entity!.Activo = false;

            await _context.SaveChangesAsync();
        }



        public async Task Activar(int id)
        {
            var entity =
                await _context.ProyectoAreas

                .FirstOrDefaultAsync(x =>
                    x.IdProyectoArea == id);

            entity!.Activo = true;

            await _context.SaveChangesAsync();
        }



        public async Task<List<ProyectoComboVM>> ObtenerProyectos()
        {
            return await _context.Proyectos

                .Where(x => x.Activo)

                .OrderBy(x => x.ClaveProyecto)

                .Select(x => new ProyectoComboVM
                {
                    IdProyecto =
                        x.IdProyecto,

                    Proyecto =
                        x.ClaveProyecto
                })

                .ToListAsync();
        }



        public async Task<List<AreaComboVM>> ObtenerAreas()
        {
            return await _context.Areas

                .Where(x => x.Activo)

                .OrderBy(x => x.AreaNombre)

                .Select(x => new AreaComboVM
                {
                    IdArea =
                        x.AreaId,

                    Area =
                        x.AreaNombre
                })

                .ToListAsync();
        }
    }
}
