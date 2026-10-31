using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Identity;

namespace SIASC.Infrastructure.Data;

public class SIASCDbContext : IdentityDbContext<UsuarioSistema>
{
    public SIASCDbContext(
        DbContextOptions<SIASCDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<TipoServicio> TiposServicio => Set<TipoServicio>();
    public DbSet<Tramite> Tramites => Set<Tramite>();
    public DbSet<EstadoTramite> EstadosTramite => Set<EstadoTramite>();

    public DbSet<HistorialEstado> HistorialEstados => Set<HistorialEstado>();
    public DbSet<EstadoPorServicio> EstadosPorServicio => Set<EstadoPorServicio>();
    public DbSet<TransicionEstado> TransicionesEstado => Set<TransicionEstado>();
    public DbSet<Seguimiento> Seguimientos => Set<Seguimiento>();

    public DbSet<DatosPasaporte> DatosPasaporte => Set<DatosPasaporte>();

    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<HistorialCambioPago> HistorialCambiosPagos
        => Set<HistorialCambioPago>();
    public DbSet<HistorialCosto> HistorialCostos => Set<HistorialCosto>();

    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<Pendiente> Pendientes => Set<Pendiente>();
    public DbSet<Documento> Documentos => Set<Documento>();

    public DbSet<FormularioVisa> FormulariosVisa => Set<FormularioVisa>();
    public DbSet<AcompananteViaje> AcompanantesViaje
        => Set<AcompananteViaje>();
    public DbSet<FamiliarVisa> FamiliaresVisa => Set<FamiliarVisa>();
    public DbSet<EmpleoAnterior> EmpleosAnteriores => Set<EmpleoAnterior>();
    public DbSet<RedSocialVisa> RedesSocialesVisa => Set<RedSocialVisa>();
    public DbSet<IdiomaVisa> IdiomasVisa => Set<IdiomaVisa>();
    public DbSet<PaisVisitado> PaisesVisitados => Set<PaisVisitado>();
    public DbSet<VisitaAnteriorEEUU> VisitasAnterioresEEUU
        => Set<VisitaAnteriorEEUU>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SIASCDbContext).Assembly);

        modelBuilder.Entity<EstadoPorServicio>()
            .HasKey(e => new { e.TipoServicioId, e.EstadoId });

        modelBuilder.Entity<EstadoPorServicio>()
            .HasOne(e => e.TipoServicio)
            .WithMany()
            .HasForeignKey(e => e.TipoServicioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EstadoPorServicio>()
            .HasOne(e => e.Estado)
            .WithMany()
            .HasForeignKey(e => e.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HistorialEstado>()
            .HasOne(e => e.EstadoAnterior)
            .WithMany()
            .HasForeignKey(e => e.EstadoAnteriorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<HistorialEstado>()
            .HasOne(e => e.EstadoNuevo)
            .WithMany()
            .HasForeignKey(e => e.EstadoNuevoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TransicionEstado>()
            .HasOne(e => e.EstadoOrigen)
            .WithMany()
            .HasForeignKey(e => e.EstadoOrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TransicionEstado>()
            .HasOne(e => e.EstadoDestino)
            .WithMany()
            .HasForeignKey(e => e.EstadoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TransicionEstado>()
            .HasIndex(e => new
            {
                e.TipoServicioId,
                e.EstadoOrigenId,
                e.EstadoDestinoId
            })
            .IsUnique();

        modelBuilder.Entity<DatosPasaporte>()
            .HasIndex(e => e.TramiteId)
            .IsUnique();

        modelBuilder.Entity<FormularioVisa>()
            .HasIndex(e => e.TramiteId)
            .IsUnique();

        modelBuilder.Entity<Documento>()
            .HasIndex(e => e.NombreAlmacenado)
            .IsUnique();

        modelBuilder.Entity<TipoServicio>()
            .HasIndex(e => e.Codigo)
            .IsUnique();

        modelBuilder.Entity<EstadoTramite>()
            .HasIndex(e => e.Codigo)
            .IsUnique();
    }
}
