using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Financieros.ClaveAdministrativa;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Services.Implementations.Financieros
{
    public class ClaveAdministrativaService : IClaveAdministrativaService
    {
        private readonly BdSigeMavContext _context;

        public ClaveAdministrativaService(BdSigeMavContext context)
        {
            _context = context;
        }


        // OBTENER TODOS

        public async Task<List<ClaveAdministrativaCrudVM>> Obtener()
        {
            return await _context.ClaveAdministrativas

                .OrderBy(x => x.ClaveAdmin)

                .Select(x => new ClaveAdministrativaCrudVM
                {
                    IdClaveAdmin = x.IdClaveAdmin,
                    ClaveAdmin = x.ClaveAdmin != null ? x.ClaveAdmin.Trim() : string.Empty,
                    Activo = (bool)x.Activo,
                    FechaCreacion = x.FechaCreacion
                }).OrderBy(x => x.ClaveAdmin)
                .ToListAsync();
        }


        // OBTENER POR ID

        public async Task<ClaveAdministrativaCrudVM?>
            ObtenerPorId(int id)
        {
            return await _context.ClaveAdministrativas

                .Where(x => x.IdClaveAdmin == id)

                .Select(x => new ClaveAdministrativaCrudVM
                {
                    IdClaveAdmin = x.IdClaveAdmin,
                    ClaveAdmin = x.ClaveAdmin,
                    //Activo = x.Activo,
                    //FechaCreacion = x.FechaCreacion
                })

                .FirstOrDefaultAsync();
        }



        public async Task<bool> Guardar(ClaveAdministrativaCrudVM model)
        {
            string clave = model.ClaveAdmin.Trim().ToUpper();

            bool existe = await _context.ClaveAdministrativas
                .AnyAsync(x => x.ClaveAdmin.ToUpper() == clave);

            if (existe)
            {
                return false;
            }

            ClaveAdministrativa entity = new ClaveAdministrativa
            {
                ClaveAdmin = clave,
                Activo = model.Activo,
                FechaCreacion = DateTime.Now
            };

            _context.ClaveAdministrativas.Add(entity);

            await _context.SaveChangesAsync();

            return true;
        }



        // ACTUALIZAR

        public async Task Actualizar(
            ClaveAdministrativaCrudVM model)
        {
            var entity =
                await _context.ClaveAdministrativas

                .FirstOrDefaultAsync(x =>
                    x.IdClaveAdmin == model.IdClaveAdmin);

            if (entity == null)
            {
                throw new Exception(
                    "Registro no encontrado");
            }

            entity.ClaveAdmin = model.ClaveAdmin;

            entity.Activo = model.Activo;

            await _context.SaveChangesAsync();
        }



        // DESACTIVAR

        public async Task Desactivar(int id)
        {
            var entity =
                await _context.ClaveAdministrativas

                .FirstOrDefaultAsync(x =>
                    x.IdClaveAdmin == id);

            if (entity == null)
            {
                throw new Exception(
                    "Registro no encontrado");
            }

            entity.Activo = false;

            await _context.SaveChangesAsync();
        }



        // ACTIVAR

        public async Task Activar(int id)
        {
            var entity =
                await _context.ClaveAdministrativas

                .FirstOrDefaultAsync(x =>
                    x.IdClaveAdmin == id);

            if (entity == null)
            {
                throw new Exception(
                    "Registro no encontrado");
            }

            entity.Activo = true;

            await _context.SaveChangesAsync();
        }


    }

}





