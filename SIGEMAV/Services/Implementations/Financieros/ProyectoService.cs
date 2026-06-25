using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Financieros.ClaveAdministrativa;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Services.Implementations.Financieros
{
    public class ProyectoService : IProyectoService
    {
        private readonly BdSigeMavContext _context;

        public ProyectoService(
            BdSigeMavContext context)
        {
            _context = context;
        }


        public async Task<List<ProyectoCrudVM>>
            Obtener()
        {
            return await _context.Proyectos

                .Include(x => x.IdClaveAdminNavigation)

                .OrderBy(x => x.ClaveProyecto)

                .Select(x => new ProyectoCrudVM
                {
                    IdProyecto = x.IdProyecto,

                    IdClaveAdmin = x.IdClaveAdmin,

                    ClaveAdministrativa = x.IdClaveAdminNavigation.ClaveAdmin,

                    ClaveProyecto = x.ClaveProyecto != null ? x.ClaveProyecto.Trim() : string.Empty,

                    Descripcion = x.Descripcion != null ? x.Descripcion.Trim() : string.Empty,

                    Activo = x.Activo
                })

                .ToListAsync();
        }


        public async Task<ProyectoCrudVM?> ObtenerPorId(int id)
        {
            return await _context.Proyectos

                .Where(x => x.IdProyecto == id)

                .Select(x => new ProyectoCrudVM
                {
                    IdProyecto = x.IdProyecto,

                    IdClaveAdmin = x.IdClaveAdmin,

                    ClaveProyecto = x.ClaveProyecto != null ? x.ClaveProyecto.Trim() : string.Empty,

                    Descripcion = x.Descripcion != null ? x.Descripcion.Trim() : string.Empty,

                    Activo = x.Activo
                })

                .FirstOrDefaultAsync();
        }



        public async Task Guardar(ProyectoCrudVM model)
        {
            Proyecto entity = new Proyecto
            {
                IdClaveAdmin = model.IdClaveAdmin.Value,

                ClaveProyecto = model.ClaveProyecto,

                Descripcion = model.Descripcion,

                Activo = model.Activo,

                FechaCreacion = DateTime.Now
            };

            _context.Proyectos.Add(entity);

            await _context.SaveChangesAsync();
        }


        public async Task Actualizar(ProyectoCrudVM model)
        {
            var entity = await _context.Proyectos.FirstOrDefaultAsync(x => x.IdProyecto == model.IdProyecto);

            if (entity == null)
            {
                throw new Exception(
                    "Registro no encontrado");
            }

            entity.IdClaveAdmin = model.IdClaveAdmin.Value;

            entity.ClaveProyecto =
                model.ClaveProyecto;

            entity.Descripcion =
                model.Descripcion;

            entity.Activo =
                model.Activo;

            await _context.SaveChangesAsync();
        }


        public async Task Desactivar(int id)
        {
            var entity = await _context.Proyectos

                .FirstOrDefaultAsync(x =>
                    x.IdProyecto == id);

            entity!.Activo = false;

            await _context.SaveChangesAsync();
        }

        public async Task Activar(int id)
        {
            var entity = await _context.Proyectos

                .FirstOrDefaultAsync(x =>
                    x.IdProyecto == id);

            entity!.Activo = true;

            await _context.SaveChangesAsync();
        }


        public async Task<List<ClaveAdministrativaComboVM>> ObtenerClavesAdministrativas()
        {
            return await _context.ClaveAdministrativas

                .Where(x => x.Activo)

                .OrderBy(x => x.ClaveAdmin)

                .Select(x =>
                    new ClaveAdministrativaComboVM
                    {
                        IdClaveAdmin =
                            x.IdClaveAdmin,

                        ClaveAdmin =
                            x.ClaveAdmin
                    })

                .ToListAsync();
        }
    }
}
