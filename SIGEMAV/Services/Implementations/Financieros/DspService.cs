using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Data;
using SIGEMAV.Models.Entities;
using SIGEMAV.Models.ViewModels.Financieros.Dsp;
using SIGEMAV.Models.ViewModels.Financieros.Proyecto;
using SIGEMAV.Services.Interfaces.Financieros;

namespace SIGEMAV.Services.Implementations.Financieros
{
    public class DspService : IDspService
    {
        private readonly BdSigeMavContext _context;
        private readonly IWebHostEnvironment _environment;


        public DspService(BdSigeMavContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }

        public async Task<List<DspVM>> Obtener(int estado = 1)
        {
            IQueryable<Dsp> query = _context.Dsps;

            switch (estado)
            {
                case 1:
                    query = query.Where(x => x.Activo);
                    break;

                case 0:
                    query = query.Where(x => !x.Activo);
                    break;
            }

            return await query
                .Select(x => new DspVM
                {
                    IdDsp = x.IdDsp,
                    Fecha = x.Fecha,
                    NoDsp = x.NoDsp,
                    Descripcion = x.Descripcion,
                    Importe = x.Importe,
                    RutaArchivo = x.RutaArchivo,
                    Activo = x.Activo
                })
                .ToListAsync();
        }


        public async Task<bool> Guardar(DspVM model, string rutaArchivo)
        {
            try
            {
                if (model == null)
                    return false;

                if (string.IsNullOrWhiteSpace(model.NoDsp))
                    return false;

                var noDsp = model.NoDsp.Trim().ToUpper();
                bool existe = await _context.Dsps.AnyAsync(x => x.NoDsp.Trim().ToUpper() == noDsp);

                if (existe) return false;

                Dsp entity = new()
                {
                    Fecha = model.Fecha!.Value,
                    NoDsp = model.NoDsp.Trim(),
                    Descripcion = model.Descripcion?.Trim(),
                    Importe = model.Importe!.Value,
                    RutaArchivo = rutaArchivo,
                    Activo = true
                };

                _context.Dsps.Add(entity);

                return await _context.SaveChangesAsync() > 0;

            }
            catch (Exception)
            {
                return false;
            }

        }


        public async Task<bool> Editar(DspEditarVM model, string? rutaArchivo)
        {
            try
            {
                if (model == null)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(model.NoDsp))
                {
                    return false;
                }

                var noDsp = model.NoDsp.Trim().ToUpper();

                var entity = await _context.Dsps.FirstOrDefaultAsync(x => x.IdDsp == model.IdDsp);

                if (entity == null)
                {
                    return false;
                }

                // =========================
                // VALIDAR DSP DUPLICADO
                // =========================

                bool existe = await _context.Dsps.AnyAsync(x => x.NoDsp.Trim().ToUpper() == noDsp
                        && x.IdDsp != model.IdDsp);

                if (existe)
                {
                    return false;
                }

                // =========================
                // ACTUALIZAR DATOS
                // =========================

                entity.Fecha = model.Fecha!.Value;

                entity.NoDsp = model.NoDsp.Trim();

                entity.Descripcion = model.Descripcion?.Trim();

                entity.Importe = model.Importe!.Value;

                // =========================
                // REEMPLAZAR PDF
                // =========================

                if (!string.IsNullOrWhiteSpace(rutaArchivo))
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(entity.RutaArchivo) &&
                            File.Exists(entity.RutaArchivo))
                        {
                            File.Delete(entity.RutaArchivo);
                        }
                    }
                    catch
                    {
                        // Aquí podrías registrar un log
                    }

                    entity.RutaArchivo = rutaArchivo;
                }

                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }






        public async Task Activar(int id)
        {
            var entity = await _context.Dsps.FirstOrDefaultAsync(x => x.IdDsp == id);

            if (entity == null)
            {
                throw new Exception(
                    "Registro no encontrado");
            }

            entity.Activo = true;

            await _context.SaveChangesAsync();
        }


        public async Task Desactivar(int id)
        {
            var entity = await _context.Dsps.FirstOrDefaultAsync(x => x.IdDsp == id);

            if (entity == null)
            {
                throw new Exception("Registro no encontrado");
            }

            entity.Activo = false;

            await _context.SaveChangesAsync();
        }


        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var dsp = await _context.Dsps
                    .FirstOrDefaultAsync(x => x.IdDsp == id);

                if (dsp == null)
                    return false;

                _context.Dsps.Remove(dsp);

                await _context.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }


        public async Task<string?> VerPdf(int id)
        {
            return await _context.Dsps
                .Where(x => x.IdDsp == id)
                .Select(x => x.RutaArchivo)
                .FirstOrDefaultAsync();
        }


        public async Task<DspEditarVM?> ObtenerPorId(int id)
        {
            var dsp = await _context.Dsps.FirstOrDefaultAsync(x => x.IdDsp == id);

            if (dsp == null)
                return null;

            return new DspEditarVM
            {
                IdDsp = dsp.IdDsp,
                Fecha = dsp.Fecha,
                Descripcion = dsp.Descripcion.Trim(),
                Importe = dsp.Importe,
                RutaArchivo = dsp.RutaArchivo,
                NoDsp = dsp.NoDsp.Trim(),
                Activo = dsp.Activo

            };

        }





    }
}
