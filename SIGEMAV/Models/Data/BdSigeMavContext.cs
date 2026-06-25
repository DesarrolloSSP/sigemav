using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SIGEMAV.Models.Entities;

namespace SIGEMAV.Models.Data;

public partial class BdSigeMavContext : DbContext
{
    public BdSigeMavContext()
    {
    }

    public BdSigeMavContext(DbContextOptions<BdSigeMavContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Anio> Anios { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Banco> Bancos { get; set; }

    public virtual DbSet<Cilindro> Cilindros { get; set; }

    public virtual DbSet<ClaveAdministrativa> ClaveAdministrativas { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<ConceptoServicio> ConceptoServicios { get; set; }

    public virtual DbSet<ConceptoServicioCosto> ConceptoServicioCostos { get; set; }

    public virtual DbSet<ConfiguracionValidacionEstatus> ConfiguracionValidacionEstatuses { get; set; }

    public virtual DbSet<ConfiguracionValidacionTecnica> ConfiguracionValidacionTecnicas { get; set; }

    public virtual DbSet<DispPresupuestal> DispPresupuestals { get; set; }

    public virtual DbSet<Dsp> Dsps { get; set; }

    public virtual DbSet<EstatusMantenimiento> EstatusMantenimientos { get; set; }

    public virtual DbSet<EstatusSolicitudMantenimiento> EstatusSolicitudMantenimientos { get; set; }

    public virtual DbSet<EstatusTransferenciaUnidad> EstatusTransferenciaUnidads { get; set; }

    public virtual DbSet<Fianza> Fianzas { get; set; }

    public virtual DbSet<Mantenimiento> Mantenimientos { get; set; }

    public virtual DbSet<MantenimientoDetalle> MantenimientoDetalles { get; set; }

    public virtual DbSet<MantenimientoDocumento> MantenimientoDocumentos { get; set; }

    public virtual DbSet<MantenimientoSeguimiento> MantenimientoSeguimientos { get; set; }

    public virtual DbSet<Marca> Marcas { get; set; }

    public virtual DbSet<Modelo> Modelos { get; set; }

    public virtual DbSet<MovimientoPresupuestal> MovimientoPresupuestals { get; set; }

    public virtual DbSet<Municipio> Municipios { get; set; }

    public virtual DbSet<ObjetoGasto> ObjetoGastos { get; set; }

    public virtual DbSet<OrdenPago> OrdenPagos { get; set; }

    public virtual DbSet<OrdenPagoDetalle> OrdenPagoDetalles { get; set; }

    public virtual DbSet<Partidum> Partida { get; set; }

    public virtual DbSet<Proyecto> Proyectos { get; set; }

    public virtual DbSet<ProyectoArea> ProyectoAreas { get; set; }

    public virtual DbSet<Recurso> Recursos { get; set; }

    public virtual DbSet<SolicitudMantenimiento> SolicitudMantenimientos { get; set; }

    public virtual DbSet<SolicitudMantenimientoDetalle> SolicitudMantenimientoDetalles { get; set; }

    public virtual DbSet<SolicitudMantenimientoDocumento> SolicitudMantenimientoDocumentos { get; set; }

    public virtual DbSet<SolicitudMantenimientoDsp> SolicitudMantenimientoDsps { get; set; }

    public virtual DbSet<SolicitudMantenimientoSeguimiento> SolicitudMantenimientoSeguimientos { get; set; }

    public virtual DbSet<Taller> Tallers { get; set; }

    public virtual DbSet<TallerControlPresupuestal> TallerControlPresupuestals { get; set; }

    public virtual DbSet<TallerMovimientoPresupuesto> TallerMovimientoPresupuestos { get; set; }

    public virtual DbSet<TipoCatalogoServicio> TipoCatalogoServicios { get; set; }

    public virtual DbSet<TipoMantenimiento> TipoMantenimientos { get; set; }

    public virtual DbSet<TipoRechazoSolicitud> TipoRechazoSolicituds { get; set; }

    public virtual DbSet<TransferenciaUnidad> TransferenciaUnidads { get; set; }

    public virtual DbSet<TransferenciaUnidadSeguimiento> TransferenciaUnidadSeguimientos { get; set; }

    public virtual DbSet<Transmision> Transmisions { get; set; }

    public virtual DbSet<UnidadVehicular> UnidadVehiculars { get; set; }

    public virtual DbSet<OrdenDePagoResult> OrdenDePagoResults { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=10.8.3.115;Database=DBSigemav;User Id=sassp;Password=s4$5pBr2025;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anio>(entity =>
        {
            entity.HasKey(e => e.AnioId).HasName("PK__Anio__BAA5DC26A7316BE3");

            entity.ToTable("Anio");

            entity.HasIndex(e => e.AnioDescripcion, "UX_Anio_Descripcion").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.AnioDescripcion)
                .HasMaxLength(4)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.AreaId).HasName("PK__Area__70B8204863D08260");

            entity.ToTable("Area");

            entity.HasIndex(e => e.AreaNombre, "UX_Area_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.AreaNombre).HasMaxLength(200);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
        });

        modelBuilder.Entity<Banco>(entity =>
        {
            entity.HasKey(e => e.BancoId).HasName("PK__Banco__4A8BAFF56F46ECE2");

            entity.ToTable("Banco");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(20);

            entity.HasOne(d => d.Fianza).WithMany(p => p.Bancos)
                .HasForeignKey(d => d.FianzaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Banco_Fianza");
        });

        modelBuilder.Entity<Cilindro>(entity =>
        {
            entity.HasKey(e => e.CilindroId).HasName("PK__Cilindro__311BD9ED61FDE5EE");

            entity.ToTable("Cilindro");

            entity.HasIndex(e => e.CilindroNombre, "UX_Cilindro_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CilindroNombre).HasMaxLength(2);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
        });

        modelBuilder.Entity<ClaveAdministrativa>(entity =>
        {
            entity.HasKey(e => e.IdClaveAdmin);

            entity.ToTable("ClaveAdministrativa");

            entity.Property(e => e.ClaveAdmin)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("PK__Color__8DA7674DBDD8612A");

            entity.ToTable("Color");

            entity.HasIndex(e => e.ColorNombre, "UX_Color_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.ColorNombre).HasMaxLength(35);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
        });

        modelBuilder.Entity<ConceptoServicio>(entity =>
        {
            entity.HasKey(e => e.ConceptoServicioId).HasName("PK__Concepto__FD8BC367296E9B19");

            entity.ToTable("ConceptoServicio");

            entity.HasIndex(e => new { e.TipoCatalogoServicioId, e.ConceptoServicioNombre }, "UX_ConceptoServicio_Tipo_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.ConceptoServicioDescripcion).HasMaxLength(500);
            entity.Property(e => e.ConceptoServicioNombre).HasMaxLength(200);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.TipoCatalogoServicio).WithMany(p => p.ConceptoServicios)
                .HasForeignKey(d => d.TipoCatalogoServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConceptoServicio_TipoCatalogoServicio");
        });

        modelBuilder.Entity<ConceptoServicioCosto>(entity =>
        {
            entity.HasKey(e => e.ConceptoServicioCostoId).HasName("PK__Concepto__CF4FD2731C35AB67");

            entity.ToTable("ConceptoServicioCosto");

            entity.HasIndex(e => new { e.MarcaId, e.ModeloId, e.AnioId, e.TransmisionId, e.CilindroId }, "IX_ConceptoServicioCosto_BusquedaVehicular");

            entity.HasIndex(e => e.ConceptoServicioId, "IX_ConceptoServicioCosto_Concepto");

            entity.HasIndex(e => e.ConceptoServicioId, "IX_ConceptoServicioCosto_ConceptoServicio");

            entity.HasIndex(e => e.ConceptoServicioId, "IX_ConceptoServicioCosto_ConceptoServicioId");

            entity.HasIndex(e => new { e.FechaInicioVigencia, e.FechaFinVigencia }, "IX_ConceptoServicioCosto_Vigencia");

            entity.HasIndex(e => new { e.MarcaId, e.ModeloId, e.AnioId, e.TransmisionId, e.CilindroId, e.ConceptoServicioId, e.FechaInicioVigencia }, "UX_ConceptoServicioCosto").IsUnique();

            entity.HasIndex(e => new { e.MarcaId, e.ModeloId, e.AnioId, e.TransmisionId, e.CilindroId, e.ConceptoServicioId }, "UX_ConceptoServicioCosto_Regla")
                .IsUnique()
                .HasFilter("([Activo]=(1))");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CostoManoObra).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostoRefacciones).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Observaciones).HasMaxLength(500);

            entity.HasOne(d => d.Anio).WithMany(p => p.ConceptoServicioCostos)
                .HasForeignKey(d => d.AnioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConceptoServicioCosto_Anio");

            entity.HasOne(d => d.Cilindro).WithMany(p => p.ConceptoServicioCostos)
                .HasForeignKey(d => d.CilindroId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConceptoServicioCosto_Cilindro");

            entity.HasOne(d => d.ConceptoServicio).WithMany(p => p.ConceptoServicioCostos)
                .HasForeignKey(d => d.ConceptoServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConceptoServicioCosto_ConceptoServicio");

            entity.HasOne(d => d.Marca).WithMany(p => p.ConceptoServicioCostos)
                .HasForeignKey(d => d.MarcaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConceptoServicioCosto_Marca");

            entity.HasOne(d => d.Modelo).WithMany(p => p.ConceptoServicioCostos)
                .HasForeignKey(d => d.ModeloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConceptoServicioCosto_Modelo");

            entity.HasOne(d => d.Transmision).WithMany(p => p.ConceptoServicioCostos)
                .HasForeignKey(d => d.TransmisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConceptoServicioCosto_Transmision");
        });

        modelBuilder.Entity<ConfiguracionValidacionEstatus>(entity =>
        {
            entity.HasKey(e => e.ConfiguracionValidacionEstatusId).HasName("PK__Configur__E9B849A1851B2104");

            entity.ToTable("ConfiguracionValidacionEstatus");

            entity.HasIndex(e => e.ConfiguracionValidacionTecnicaId, "IX_ConfigValidacionEstatus_ConfigId");

            entity.HasIndex(e => e.EstatusSolicitudMantenimientoId, "IX_ConfigValidacionEstatus_Estatus");

            entity.HasIndex(e => new { e.ConfiguracionValidacionTecnicaId, e.EstatusSolicitudMantenimientoId }, "UQ_Config_Estatus").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

            entity.HasOne(d => d.ConfiguracionValidacionTecnica).WithMany(p => p.ConfiguracionValidacionEstatuses)
                .HasForeignKey(d => d.ConfiguracionValidacionTecnicaId)
                .HasConstraintName("FK_ConfiguracionValidacionEstatus_Configuracion");

            entity.HasOne(d => d.EstatusSolicitudMantenimiento).WithMany(p => p.ConfiguracionValidacionEstatuses)
                .HasForeignKey(d => d.EstatusSolicitudMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ConfiguracionValidacionEstatus_EstatusSolicitudMantenimiento");
        });

        modelBuilder.Entity<ConfiguracionValidacionTecnica>(entity =>
        {
            entity.HasKey(e => e.ConfiguracionValidacionTecnicaId).HasName("PK__Configur__B924094804E81DFB");

            entity.ToTable("ConfiguracionValidacionTecnica");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.AdvertenciaPorcentaje).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EstatusSolicitudesAbiertas).HasMaxLength(100);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaModificacion).HasColumnType("datetime");

            entity.HasOne(d => d.ConceptoServicio).WithMany(p => p.ConfiguracionValidacionTecnicas)
                .HasForeignKey(d => d.ConceptoServicioId)
                .HasConstraintName("FK_ConfiguracionValidacionTecnica_ConceptoServicios");

            entity.HasOne(d => d.TipoCatalogoServicio).WithMany(p => p.ConfiguracionValidacionTecnicas)
                .HasForeignKey(d => d.TipoCatalogoServicioId)
                .HasConstraintName("FK_ConfiguracionValidacionTecnica_TipoCatalogoServicios");
        });

        modelBuilder.Entity<DispPresupuestal>(entity =>
        {
            entity.HasKey(e => e.IdDispPresupuestal).HasName("PK_DatosDsp");

            entity.ToTable("DispPresupuestal");

            entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdAreaNavigation).WithMany(p => p.DispPresupuestals)
                .HasForeignKey(d => d.IdArea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DispPresupuestal_Area");

            entity.HasOne(d => d.IdClaveAdminNavigation).WithMany(p => p.DispPresupuestals)
                .HasForeignKey(d => d.IdClaveAdmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DispPresupuestal_ClaveAdministrativa");

            entity.HasOne(d => d.IdDspNavigation).WithMany(p => p.DispPresupuestals)
                .HasForeignKey(d => d.IdDsp)
                .HasConstraintName("FK_DispPresupuestal_Dsp");

            entity.HasOne(d => d.IdObjetoGastoNavigation).WithMany(p => p.DispPresupuestals)
                .HasForeignKey(d => d.IdObjetoGasto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DispPresupuestal_ObjetoGasto");

            entity.HasOne(d => d.IdProyectoNavigation).WithMany(p => p.DispPresupuestals)
                .HasForeignKey(d => d.IdProyecto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DispPresupuestal_Proyecto");
        });

        modelBuilder.Entity<Dsp>(entity =>
        {
            entity.HasKey(e => e.IdDsp);

            entity.ToTable("Dsp");

            entity.HasIndex(e => e.NoDsp, "UX_Dsp_NoDsp").IsUnique();

            entity.Property(e => e.Descripcion).IsUnicode(false);
            entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NoDsp)
                .HasMaxLength(30)
                .IsFixedLength();
            entity.Property(e => e.RutaArchivo).HasMaxLength(200);
        });

        modelBuilder.Entity<EstatusMantenimiento>(entity =>
        {
            entity.ToTable("EstatusMantenimiento");

            entity.Property(e => e.EstatusMantenimientoId).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<EstatusSolicitudMantenimiento>(entity =>
        {
            entity.HasKey(e => e.EstatusSolicitudMantenimientoId).HasName("PK__EstatusS__9C9F86BBF4FBFC3F");

            entity.ToTable("EstatusSolicitudMantenimiento");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Color).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<EstatusTransferenciaUnidad>(entity =>
        {
            entity.HasKey(e => e.EstatusTransferenciaUnidadId).HasName("PK__EstatusT__066C34A66A090DBF");

            entity.ToTable("EstatusTransferenciaUnidad");

            entity.Property(e => e.EstatusTransferenciaUnidadId).ValueGeneratedNever();
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<Fianza>(entity =>
        {
            entity.HasKey(e => e.FianzaId).HasName("PK__Fianza__335F7C01A94684C2");

            entity.ToTable("Fianza");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Clabe).HasMaxLength(18);
            entity.Property(e => e.Fianza1)
                .HasMaxLength(25)
                .HasColumnName("Fianza");
        });

        modelBuilder.Entity<Mantenimiento>(entity =>
        {
            entity.HasKey(e => e.MantenimientoId).HasName("PK__Mantenim__A62E61A23CA77448");

            entity.ToTable("Mantenimiento");

            entity.HasIndex(e => e.EstatusMantenimientoId, "IX_Mantenimiento_EstatusMantenimientoId");

            entity.HasIndex(e => e.SolicitudMantenimientoId, "IX_Mantenimiento_SolicitudMantenimientoId");

            entity.HasIndex(e => new { e.SolicitudMantenimientoId, e.Activo }, "IX_Mantenimiento_Solicitud_Activo");

            entity.HasIndex(e => e.UnidadVehicularId, "IX_Mantenimiento_UnidadVehicularId");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.CostoRealTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Diagnostico).HasMaxLength(2000);
            entity.Property(e => e.DiasFueraServicio).HasComputedColumnSql("(datediff(day,[FechaIngreso],[FechaSalida]))", true);
            entity.Property(e => e.FechaCierre).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.FechaEjecucion).HasColumnType("datetime");
            entity.Property(e => e.FechaFinTrabajo).HasColumnType("datetime");
            entity.Property(e => e.FechaInicioTrabajo).HasColumnType("datetime");
            entity.Property(e => e.MotivoMantenimiento).HasMaxLength(1000);
            entity.Property(e => e.Observaciones).HasMaxLength(2000);
            entity.Property(e => e.TrabajoRealizado).HasMaxLength(4000);

            entity.HasOne(d => d.EstatusMantenimiento).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.EstatusMantenimientoId)
                .HasConstraintName("FK_Mantenimiento_EstatusMantenimiento");

            entity.HasOne(d => d.SolicitudMantenimiento).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.SolicitudMantenimientoId)
                .HasConstraintName("FK_Mantenimiento_SolicitudMantenimiento");

            entity.HasOne(d => d.Taller).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.TallerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mantenimiento_Taller");

            entity.HasOne(d => d.TipoMantenimiento).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.TipoMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mantenimiento_TipoMantenimiento");

            entity.HasOne(d => d.UnidadVehicular).WithMany(p => p.Mantenimientos)
                .HasForeignKey(d => d.UnidadVehicularId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Mantenimiento_UnidadVehicular");
        });

        modelBuilder.Entity<MantenimientoDetalle>(entity =>
        {
            entity.HasKey(e => e.MantenimientoDetalleId).HasName("PK__Mantenim__1925C6364FACB300");

            entity.ToTable("MantenimientoDetalle");

            entity.HasIndex(e => e.MantenimientoId, "IX_MantenimientoDetalle_MantenimientoId");

            entity.Property(e => e.Cantidad).HasDefaultValue(1);
            entity.Property(e => e.CostoUnitarioManoObra).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostoUnitarioRefacciones).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Observaciones).HasMaxLength(1000);
            entity.Property(e => e.SubtotalManoObra)
                .HasComputedColumnSql("([Cantidad]*[CostoUnitarioManoObra])", false)
                .HasColumnType("decimal(29, 2)");
            entity.Property(e => e.SubtotalRefacciones)
                .HasComputedColumnSql("([Cantidad]*[CostoUnitarioRefacciones])", false)
                .HasColumnType("decimal(29, 2)");
            entity.Property(e => e.TotalLinea)
                .HasComputedColumnSql("([Cantidad]*[CostoUnitarioManoObra]+[Cantidad]*[CostoUnitarioRefacciones])", false)
                .HasColumnType("decimal(30, 2)");

            entity.HasOne(d => d.ConceptoServicio).WithMany(p => p.MantenimientoDetalles)
                .HasForeignKey(d => d.ConceptoServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MantenimientoDetalle_ConceptoServicio");

            entity.HasOne(d => d.IdObjetoGastoManoObraNavigation).WithMany(p => p.MantenimientoDetalleIdObjetoGastoManoObraNavigations)
                .HasForeignKey(d => d.IdObjetoGastoManoObra)
                .HasConstraintName("FK_MantenimientoDetalle_ObjetoGastoManoObra");

            entity.HasOne(d => d.IdObjetoGastoRefaccionesNavigation).WithMany(p => p.MantenimientoDetalleIdObjetoGastoRefaccionesNavigations)
                .HasForeignKey(d => d.IdObjetoGastoRefacciones)
                .HasConstraintName("FK_MantenimientoDetalle_ObjetoGastoRefacciones");

            entity.HasOne(d => d.Mantenimiento).WithMany(p => p.MantenimientoDetalles)
                .HasForeignKey(d => d.MantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MantenimientoDetalle_Mantenimiento");
        });

        modelBuilder.Entity<MantenimientoDocumento>(entity =>
        {
            entity.ToTable("MantenimientoDocumento");

            entity.HasIndex(e => e.MantenimientoId, "IX_MantenimientoDocumento_MantenimientoId");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCarga)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HashArchivo)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.NombreArchivo).HasMaxLength(300);
            entity.Property(e => e.RutaArchivo).HasMaxLength(1000);
            entity.Property(e => e.TipoDocumento).HasMaxLength(100);

            entity.HasOne(d => d.Mantenimiento).WithMany(p => p.MantenimientoDocumentos)
                .HasForeignKey(d => d.MantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MantenimientoDocumento_Mantenimiento");
        });

        modelBuilder.Entity<MantenimientoSeguimiento>(entity =>
        {
            entity.ToTable("MantenimientoSeguimiento");

            entity.HasIndex(e => e.MantenimientoId, "IX_MantenimientoSeguimiento_MantenimientoId");

            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Observaciones).HasMaxLength(1000);

            entity.HasOne(d => d.EstatusMantenimiento).WithMany(p => p.MantenimientoSeguimientos)
                .HasForeignKey(d => d.EstatusMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MantenimientoSeguimiento_Estatus");

            entity.HasOne(d => d.Mantenimiento).WithMany(p => p.MantenimientoSeguimientos)
                .HasForeignKey(d => d.MantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MantenimientoSeguimiento_Mantenimiento");
        });

        modelBuilder.Entity<Marca>(entity =>
        {
            entity.HasKey(e => e.MarcaId).HasName("PK__Marca__D5B1CD8B8624BDF5");

            entity.ToTable("Marca");

            entity.HasIndex(e => e.MarcaNombre, "UX_Marca_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.MarcaNombre).HasMaxLength(35);
        });

        modelBuilder.Entity<Modelo>(entity =>
        {
            entity.HasKey(e => e.ModeloId).HasName("PK__Modelo__FA60529A60538FC4");

            entity.ToTable("Modelo");

            entity.HasIndex(e => e.MarcaId, "IX_Modelo_MarcaId");

            entity.HasIndex(e => new { e.ModeloNombre, e.MarcaId }, "UX_Modelo_Nombre_Marca").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.ModeloNombre).HasMaxLength(50);

            entity.HasOne(d => d.Marca).WithMany(p => p.Modelos)
                .HasForeignKey(d => d.MarcaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Modelo_Marca");
        });

        modelBuilder.Entity<MovimientoPresupuestal>(entity =>
        {
            entity.HasKey(e => e.IdMovimiento).HasName("PK__Movimien__881A6AE0831983B6");

            entity.ToTable("MovimientoPresupuestal");

            entity.Property(e => e.FechaCaptura).HasColumnType("datetime");
            entity.Property(e => e.FechaMovimiento).HasColumnType("datetime");
            entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioCaptura)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDispPresupuestalNavigation).WithMany(p => p.MovimientoPresupuestals)
                .HasForeignKey(d => d.IdDispPresupuestal)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MovimientoPresupuestal_DispPresupuestal");

            entity.HasOne(d => d.SolicitudMantenimiento).WithMany(p => p.MovimientoPresupuestals)
                .HasForeignKey(d => d.SolicitudMantenimientoId)
                .HasConstraintName("FK_MovimientoPresupuestal_SolicitudMantenimiento");
        });

        modelBuilder.Entity<Municipio>(entity =>
        {
            entity.ToTable("Municipio");

            entity.Property(e => e.MunicipioId).ValueGeneratedNever();
            entity.Property(e => e.CveDeleg).HasColumnName("CVE_DELEG");
            entity.Property(e => e.CveMun)
                .HasMaxLength(3)
                .HasColumnName("CVE_MUN");
            entity.Property(e => e.CveZona).HasColumnName("CVE_ZONA");
            entity.Property(e => e.Delegacion)
                .HasMaxLength(120)
                .HasColumnName("DELEGACION");
            entity.Property(e => e.Nombre)
                .HasMaxLength(120)
                .HasColumnName("NOMBRE");
            entity.Property(e => e.TcmunicipioAlerta).HasColumnName("tcmunicipioAlerta");
            entity.Property(e => e.TcmunicipioCpdh).HasColumnName("tcmunicipioCPDH");
            entity.Property(e => e.TcmunicipioIndigena).HasColumnName("tcmunicipioIndigena");
            entity.Property(e => e.TcmunicipioRegion)
                .HasMaxLength(40)
                .IsUnicode(false)
                .HasColumnName("tcmunicipioRegion");
            entity.Property(e => e.UnidadVehicularId).ValueGeneratedOnAdd();
            entity.Property(e => e.Zona)
                .HasMaxLength(50)
                .HasColumnName("ZONA");
        });

        modelBuilder.Entity<ObjetoGasto>(entity =>
        {
            entity.HasKey(e => e.IdObjetoGasto);

            entity.ToTable("ObjetoGasto");

            entity.Property(e => e.ClaveObjGasto)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
        });

        modelBuilder.Entity<OrdenPago>(entity =>
        {
            entity.HasKey(e => e.IdOrdenPago).HasName("PK__OrdenPag__C64E47F6954023E2");

            entity.ToTable("OrdenPago");

            entity.Property(e => e.Estatus)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FechaCaptura).HasColumnType("datetime");
            entity.Property(e => e.ImporteTotal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NumeroOrden)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RutaArchivo)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioCaptura)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Proovedor).WithMany(p => p.OrdenPagos)
                .HasForeignKey(d => d.ProovedorId)
                .HasConstraintName("FK_OrdenPago_Taller");
        });

        modelBuilder.Entity<OrdenPagoDetalle>(entity =>
        {
            entity.HasKey(e => e.IdDetalle).HasName("PK__OrdenPag__E43646A51C33460A");

            entity.ToTable("OrdenPagoDetalle");

            entity.Property(e => e.Concepto)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Folio)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NoPlaca)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Serie)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.IdOrdenPagoNavigation).WithMany(p => p.OrdenPagoDetalles)
                .HasForeignKey(d => d.IdOrdenPago)
                .HasConstraintName("FK_OrdenPagoDetalle_OrdenPago");

            entity.HasOne(d => d.IdPartidaNavigation).WithMany(p => p.OrdenPagoDetalles)
                .HasForeignKey(d => d.IdPartida)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrdenPagoDetalle_Partida");
        });

        modelBuilder.Entity<Partidum>(entity =>
        {
            entity.HasKey(e => e.IdPartida);

            entity.Property(e => e.ClavePartida)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");
            entity.Property(e => e.Partida)
                .HasMaxLength(150)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Proyecto>(entity =>
        {
            entity.HasKey(e => e.IdProyecto);

            entity.ToTable("Proyecto");

            entity.Property(e => e.ClaveProyecto)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

            entity.HasOne(d => d.IdClaveAdminNavigation).WithMany(p => p.Proyectos)
                .HasForeignKey(d => d.IdClaveAdmin)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Proyecto_ClaveAdministrativa");
        });

        modelBuilder.Entity<ProyectoArea>(entity =>
        {
            entity.HasKey(e => e.IdProyectoArea);

            entity.ToTable("ProyectoArea");

            entity.Property(e => e.FechaCreacion).HasColumnType("datetime");

            entity.HasOne(d => d.IdAreaNavigation).WithMany(p => p.ProyectoAreas)
                .HasForeignKey(d => d.IdArea)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProyectoArea_Area");

            entity.HasOne(d => d.IdProyectoNavigation).WithMany(p => p.ProyectoAreas)
                .HasForeignKey(d => d.IdProyecto)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProyectoArea_Proyecto");
        });

        modelBuilder.Entity<Recurso>(entity =>
        {
            entity.HasKey(e => e.RecursoId).HasName("PK__Recurso__82F2B1843943F398");

            entity.ToTable("Recurso");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(20);
        });

        modelBuilder.Entity<SolicitudMantenimiento>(entity =>
        {
            entity.HasKey(e => e.SolicitudMantenimientoId).HasName("PK__Solicitu__F8D8FEF5EBA06DAF");

            entity.ToTable("SolicitudMantenimiento");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.DiagnosticoInicial).HasMaxLength(3000);
            entity.Property(e => e.FacturaFolio)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.FacturaSerie)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Folio).HasMaxLength(20);
            entity.Property(e => e.ImporteCotizacion).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MotivoSolicitud).HasMaxLength(2000);
            entity.Property(e => e.NumeroOficio).HasMaxLength(100);
            entity.Property(e => e.Observaciones).HasMaxLength(2000);
            entity.Property(e => e.PlacaVehiculo)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.AreaSolicitante).WithMany(p => p.SolicitudMantenimientos)
                .HasForeignKey(d => d.AreaSolicitanteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudMantenimiento_Area");

            entity.HasOne(d => d.EstatusSolicitudMantenimiento).WithMany(p => p.SolicitudMantenimientos)
                .HasForeignKey(d => d.EstatusSolicitudMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudMantenimiento_Estatus");

            entity.HasOne(d => d.Taller).WithMany(p => p.SolicitudMantenimientos)
                .HasForeignKey(d => d.TallerId)
                .HasConstraintName("FK_SolicitudMantenimiento_Taller");

            entity.HasOne(d => d.TipoMantenimiento).WithMany(p => p.SolicitudMantenimientos)
                .HasForeignKey(d => d.TipoMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudMantenimiento_TipoMantenimiento");

            entity.HasOne(d => d.UnidadVehicular).WithMany(p => p.SolicitudMantenimientos)
                .HasForeignKey(d => d.UnidadVehicularId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudMantenimiento_UnidadVehicular");
        });

        modelBuilder.Entity<SolicitudMantenimientoDetalle>(entity =>
        {
            entity.HasKey(e => e.SolicitudMantenimientoDetalleId).HasName("PK__Solicitu__38435846187D6476");

            entity.ToTable("SolicitudMantenimientoDetalle");

            entity.HasIndex(e => e.IdObjetoGastoManoObra, "IX_SMD_IdObjetoGastoManoObra");

            entity.HasIndex(e => e.IdObjetoGastoRefacciones, "IX_SMD_IdObjetoGastoRefacciones");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Cantidad).HasDefaultValue(1);
            entity.Property(e => e.CostoEstimadoManoObra).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CostoEstimadoRefacciones).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Observaciones).HasMaxLength(1000);

            entity.HasOne(d => d.ConceptoServicio).WithMany(p => p.SolicitudMantenimientoDetalles)
                .HasForeignKey(d => d.ConceptoServicioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Concepto");

            entity.HasOne(d => d.IdObjetoGastoManoObraNavigation).WithMany(p => p.SolicitudMantenimientoDetalleIdObjetoGastoManoObraNavigations)
                .HasForeignKey(d => d.IdObjetoGastoManoObra)
                .HasConstraintName("FK_SMD_ObjetoGastoMO");

            entity.HasOne(d => d.IdObjetoGastoRefaccionesNavigation).WithMany(p => p.SolicitudMantenimientoDetalleIdObjetoGastoRefaccionesNavigations)
                .HasForeignKey(d => d.IdObjetoGastoRefacciones)
                .HasConstraintName("FK_SMD_ObjetoGastoRef");

            entity.HasOne(d => d.SolicitudMantenimiento).WithMany(p => p.SolicitudMantenimientoDetalles)
                .HasForeignKey(d => d.SolicitudMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDetalle_Solicitud");
        });

        modelBuilder.Entity<SolicitudMantenimientoDocumento>(entity =>
        {
            entity.HasKey(e => e.SolicitudMantenimientoDocumentoId).HasName("PK__Solicitu__BE49CE1690D2CC2D");

            entity.ToTable("SolicitudMantenimientoDocumento");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCarga).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.HashArchivo)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.NombreArchivo).HasMaxLength(300);
            entity.Property(e => e.RutaArchivo).HasMaxLength(1000);
            entity.Property(e => e.TipoDocumento).HasMaxLength(100);

            entity.HasOne(d => d.SolicitudMantenimiento).WithMany(p => p.SolicitudMantenimientoDocumentos)
                .HasForeignKey(d => d.SolicitudMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudDocumento_Solicitud");
        });

        modelBuilder.Entity<SolicitudMantenimientoDsp>(entity =>
        {
            entity.HasKey(e => e.SolicitudMantenimientoDspid).HasName("PK__Solicitu__C525F1276657CDB0");

            entity.ToTable("SolicitudMantenimientoDSP");

            entity.Property(e => e.SolicitudMantenimientoDspid).HasColumnName("SolicitudMantenimientoDSPId");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.FechaDsp).HasColumnName("FechaDSP");
            entity.Property(e => e.ImporteAutorizado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ImporteSolicitado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.NumeroDsp)
                .HasMaxLength(50)
                .HasColumnName("NumeroDSP");
            entity.Property(e => e.Observaciones).HasMaxLength(2000);
            entity.Property(e => e.PresupuestoDisponible).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.SolicitudMantenimiento).WithMany(p => p.SolicitudMantenimientoDsps)
                .HasForeignKey(d => d.SolicitudMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolicitudMantenimientoDSP_Solicitud");
        });

        modelBuilder.Entity<SolicitudMantenimientoSeguimiento>(entity =>
        {
            entity.HasKey(e => e.SolicitudMantenimientoSeguimientoId).HasName("PK__Solicitu__3D82FDD8BB55A265");

            entity.ToTable("SolicitudMantenimientoSeguimiento");

            entity.Property(e => e.FechaMovimiento).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.Observaciones).HasMaxLength(2000);

            entity.HasOne(d => d.EstatusSolicitudMantenimiento).WithMany(p => p.SolicitudMantenimientoSeguimientos)
                .HasForeignKey(d => d.EstatusSolicitudMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Seguimiento_Estatus");

            entity.HasOne(d => d.SolicitudMantenimiento).WithMany(p => p.SolicitudMantenimientoSeguimientos)
                .HasForeignKey(d => d.SolicitudMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Seguimiento_Solicitud");

            entity.HasOne(d => d.TipoRechazoSolicitud).WithMany(p => p.SolicitudMantenimientoSeguimientos)
                .HasForeignKey(d => d.TipoRechazoSolicitudId)
                .HasConstraintName("FK_SolicitudMantenimientoSeguimiento_TipoRechazo");
        });

        modelBuilder.Entity<Taller>(entity =>
        {
            entity.HasKey(e => e.TallerId).HasName("PK__Taller__2DABE265C3FEDB4A");

            entity.ToTable("Taller");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Contrato).HasMaxLength(15);
            entity.Property(e => e.MontoMaximo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.MontoMinimo).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RazonSocial).HasMaxLength(100);
            entity.Property(e => e.Rfc)
                .HasMaxLength(13)
                .IsFixedLength()
                .HasColumnName("RFC");
            entity.Property(e => e.Ubicacion).HasMaxLength(150);

            entity.HasOne(d => d.Banco).WithMany(p => p.Tallers)
                .HasForeignKey(d => d.BancoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Taller_Banco");

            entity.HasOne(d => d.Recurso).WithMany(p => p.Tallers)
                .HasForeignKey(d => d.RecursoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Taller_Recurso");
        });

        modelBuilder.Entity<TallerControlPresupuestal>(entity =>
        {
            entity.HasKey(e => e.IdControlTaller).HasName("PK__TallerCo__838B3567E5D40CDC");

            entity.ToTable("TallerControlPresupuestal");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaActualizacion).HasColumnType("datetime");
            entity.Property(e => e.PresupuestoAsignado).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PresupuestoComprometido).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PresupuestoDisponible).HasColumnType("decimal(19, 2)");
            entity.Property(e => e.PresupuestoEjercido).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Taller).WithMany(p => p.TallerControlPresupuestals)
                .HasForeignKey(d => d.TallerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TallerControlPresupuestal_Taller");
        });

        modelBuilder.Entity<TallerMovimientoPresupuesto>(entity =>
        {
            entity.HasKey(e => e.IdMovimientoTaller).HasName("PK__TallerMo__54D165C3AC3329DB");

            entity.ToTable("TallerMovimientoPresupuesto");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Importe).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TipoMovimiento)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.UsuarioCaptura)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.SolicitudMantenimiento).WithMany(p => p.TallerMovimientoPresupuestos)
                .HasForeignKey(d => d.SolicitudMantenimientoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TallerMov__Solic__379037E3");

            entity.HasOne(d => d.Taller).WithMany(p => p.TallerMovimientoPresupuestos)
                .HasForeignKey(d => d.TallerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TallerMov__Talle__369C13AA");
        });

        modelBuilder.Entity<TipoCatalogoServicio>(entity =>
        {
            entity.HasKey(e => e.TipoCatalogoServicioId).HasName("PK__TipoCata__36EDB0F206C444A6");

            entity.ToTable("TipoCatalogoServicio");

            entity.HasIndex(e => e.TipoCatalogoServicioNombre, "UX_TipoCatalogoServicio_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TipoCatalogoServicioNombre).HasMaxLength(100);
        });

        modelBuilder.Entity<TipoMantenimiento>(entity =>
        {
            entity.HasKey(e => e.TipoMantenimientoId).HasName("PK__TipoMant__CF3911C72EB06883");

            entity.ToTable("TipoMantenimiento");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<TipoRechazoSolicitud>(entity =>
        {
            entity.HasKey(e => e.TipoRechazoSolicitudId).HasName("PK__TipoRech__68AC009BE0508965");

            entity.ToTable("TipoRechazoSolicitud");

            entity.Property(e => e.TipoRechazoSolicitudId).ValueGeneratedNever();
            entity.Property(e => e.Nombre).HasMaxLength(100);
        });

        modelBuilder.Entity<TransferenciaUnidad>(entity =>
        {
            entity.HasKey(e => e.TransferenciaUnidadId).HasName("PK__Transfer__9DA0D81AE8C2E688");

            entity.ToTable("TransferenciaUnidad");

            entity.HasIndex(e => new { e.UnidadVehicularId, e.EstatusTransferenciaUnidadId }, "IX_TransferenciaUnidad_Unidad_Estatus");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.ComentariosAutorizacion).HasMaxLength(1000);
            entity.Property(e => e.DocumentoNombreOriginal).HasMaxLength(250);
            entity.Property(e => e.DocumentoRuta).HasMaxLength(500);
            entity.Property(e => e.FechaCancelacion).HasColumnType("datetime");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaEntrega).HasColumnType("datetime");
            entity.Property(e => e.FechaRecepcion).HasColumnType("datetime");
            entity.Property(e => e.FechaRespuesta).HasColumnType("datetime");
            entity.Property(e => e.FechaSolicitud)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaTransferencia).HasColumnType("datetime");
            entity.Property(e => e.Folio).HasMaxLength(50);
            entity.Property(e => e.MotivoTransferencia)
                .HasMaxLength(500)
                .HasDefaultValue("");
            entity.Property(e => e.Observaciones).HasMaxLength(1000);
            entity.Property(e => e.ObservacionesEntrega).HasMaxLength(1000);
            entity.Property(e => e.ObservacionesRecepcion).HasMaxLength(1000);

            entity.HasOne(d => d.AreaDestino).WithMany(p => p.TransferenciaUnidadAreaDestinos)
                .HasForeignKey(d => d.AreaDestinoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransferenciaUnidad_AreaDestino");

            entity.HasOne(d => d.AreaOrigen).WithMany(p => p.TransferenciaUnidadAreaOrigens)
                .HasForeignKey(d => d.AreaOrigenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransferenciaUnidad_AreaOrigen");

            entity.HasOne(d => d.EstatusTransferenciaUnidad).WithMany(p => p.TransferenciaUnidads)
                .HasForeignKey(d => d.EstatusTransferenciaUnidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransferenciaUnidad_Estatus");

            entity.HasOne(d => d.UnidadVehicular).WithMany(p => p.TransferenciaUnidads)
                .HasForeignKey(d => d.UnidadVehicularId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransferenciaUnidad_UnidadVehicular");
        });

        modelBuilder.Entity<TransferenciaUnidadSeguimiento>(entity =>
        {
            entity.HasKey(e => e.TransferenciaUnidadSeguimientoId).HasName("PK__Transfer__C08575F98FAB597B");

            entity.ToTable("TransferenciaUnidadSeguimiento");

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.DocumentoNombreOriginal).HasMaxLength(255);
            entity.Property(e => e.DocumentoRuta).HasMaxLength(500);
            entity.Property(e => e.EquipoRegistro).HasMaxLength(150);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FechaMovimiento)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.HashDocumento).HasMaxLength(128);
            entity.Property(e => e.IpRegistro).HasMaxLength(50);
            entity.Property(e => e.Observaciones).HasMaxLength(1000);
            entity.Property(e => e.TipoMovimiento).HasMaxLength(100);

            entity.HasOne(d => d.EstatusTransferenciaUnidad).WithMany(p => p.TransferenciaUnidadSeguimientos)
                .HasForeignKey(d => d.EstatusTransferenciaUnidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransferenciaUnidadSeguimiento_Estatus");

            entity.HasOne(d => d.TransferenciaUnidad).WithMany(p => p.TransferenciaUnidadSeguimientos)
                .HasForeignKey(d => d.TransferenciaUnidadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TransferenciaUnidadSeguimiento_Transferencia");
        });

        modelBuilder.Entity<Transmision>(entity =>
        {
            entity.HasKey(e => e.TransmisionId).HasName("PK__Transmis__B1558C62D2F4EFA2");

            entity.ToTable("Transmision");

            entity.HasIndex(e => e.TransmisionNombre, "UX_Transmision_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TransmisionNombre).HasMaxLength(20);
        });

        modelBuilder.Entity<UnidadVehicular>(entity =>
        {
            entity.HasKey(e => e.UnidadVehicularId).HasName("PK__UnidadVe__5E73E4C0C401F800");

            entity.ToTable("UnidadVehicular");

            entity.HasIndex(e => new { e.MarcaId, e.ModeloId }, "IX_UnidadVehicular_MarcaModelo");

            entity.HasIndex(e => e.NumeroEconomico, "IX_UnidadVehicular_NumeroEconomico");

            entity.HasIndex(e => e.PlacaActual, "IX_UnidadVehicular_Placa");

            entity.HasIndex(e => e.NumeroSerie, "UX_UnidadVehicular_NumeroSerie").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.NumeroEconomico).HasMaxLength(15);
            entity.Property(e => e.NumeroSerie).HasMaxLength(30);
            entity.Property(e => e.Observaciones).HasMaxLength(500);
            entity.Property(e => e.PlacaActual).HasMaxLength(10);

            entity.HasOne(d => d.Anio).WithMany(p => p.UnidadVehiculars)
                .HasForeignKey(d => d.AnioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnidadVehicular_Anio");

            entity.HasOne(d => d.Area).WithMany(p => p.UnidadVehiculars)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("FK_UnidadVehicular_Area");

            entity.HasOne(d => d.Cilindro).WithMany(p => p.UnidadVehiculars)
                .HasForeignKey(d => d.CilindroId)
                .HasConstraintName("FK_UnidadVehicular_Cilindro");

            entity.HasOne(d => d.Color).WithMany(p => p.UnidadVehiculars)
                .HasForeignKey(d => d.ColorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnidadVehicular_Color");

            entity.HasOne(d => d.Marca).WithMany(p => p.UnidadVehiculars)
                .HasForeignKey(d => d.MarcaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnidadVehicular_Marca");

            entity.HasOne(d => d.Modelo).WithMany(p => p.UnidadVehiculars)
                .HasForeignKey(d => d.ModeloId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnidadVehicular_Modelo");

            entity.HasOne(d => d.Municipio).WithMany(p => p.UnidadVehiculars)
                .HasForeignKey(d => d.MunicipioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UnidadVehicular_Municipio");

            entity.HasOne(d => d.Transmision).WithMany(p => p.UnidadVehiculars)
                .HasForeignKey(d => d.TransmisionId)
                .HasConstraintName("FK_UnidadVehicular_Transmision");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
