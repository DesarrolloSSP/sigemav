using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Financieros;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Services.Implementations.Financieros
{
    public class ObjetoGastoCatalogoService : IObjetoGastoCatalogoService
    {

        private readonly BdSigeMavContext _context;

        public ObjetoGastoCatalogoService(
            BdSigeMavContext context)
        {
            _context = context;
        }

        public async Task<List<ObjetoGastoCrudVM>> Obtener()
        {
            return await _context.ObjetoGastos
                .Select(x => new ObjetoGastoCrudVM
                {
                    IdObjetoGasto = x.IdObjetoGasto,
                    ClaveObjGasto = x.ClaveObjGasto,
                    Descripcion = x.Descripcion != null ? x.Descripcion.Trim() : string.Empty,
                    Activo = x.Activo
                }).OrderBy(x => x.ClaveObjGasto).ToListAsync();
        }

        public async Task<ObjetoGastoCrudVM> ObtenerPorId(int id)
        {
            return await _context.ObjetoGastos
                .Where(x => x.IdObjetoGasto == id)
                .Select(x => new ObjetoGastoCrudVM
                {
                    IdObjetoGasto = x.IdObjetoGasto,
                    ClaveObjGasto = x.ClaveObjGasto,
                    Descripcion = x.Descripcion,
                    Activo = x.Activo
                }).FirstAsync();
        }

        public async Task Guardar(ObjetoGastoCrudVM model)
        {



            ObjetoGasto entity = new ObjetoGasto
            {
                ClaveObjGasto = model.ClaveObjGasto,
                Descripcion = model.Descripcion,
                Activo = model.Activo,
                FechaCreacion = DateTime.Now
            };

            _context.ObjetoGastos.Add(entity);

            await _context.SaveChangesAsync();


        }

        public async Task Actualizar(ObjetoGastoCrudVM model)
        {
            var entity = await _context.ObjetoGastos
                .FirstAsync(x =>
                    x.IdObjetoGasto == model.IdObjetoGasto);

            entity.ClaveObjGasto = model.ClaveObjGasto;
            entity.Descripcion = model.Descripcion;
            entity.Activo = model.Activo;

            await _context.SaveChangesAsync();
        }


        public async Task Desactivar(int id)
        {
            var entity = await _context.ObjetoGastos
                .FirstAsync(x =>
                    x.IdObjetoGasto == id);

            entity.Activo = false;

            await _context.SaveChangesAsync();
        }

        public async Task Activar(int id)
        {
            var entity = await _context.ObjetoGastos
                .FirstAsync(x =>
                    x.IdObjetoGasto == id);

            entity.Activo = true;

            await _context.SaveChangesAsync();
        }

    }



}
