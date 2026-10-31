using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Identity;

namespace SIASC.Infrastructure.Configurations;

public class HistorialEstadoConfiguration
    : IEntityTypeConfiguration<HistorialEstado>
{
    public void Configure(EntityTypeBuilder<HistorialEstado> builder)
    {
        builder.ToTable("HistorialEstados");

        builder.HasKey(h => h.HistorialId);

        builder.Property(h => h.FechaCambio)
            .IsRequired();

        builder.Property(h => h.UsuarioId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(h => h.MotivoCambio)
            .HasMaxLength(1000);

        builder.HasOne(h => h.Tramite)
            .WithMany()
            .HasForeignKey(h => h.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.EstadoAnterior)
            .WithMany()
            .HasForeignKey(h => h.EstadoAnteriorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(h => h.EstadoNuevo)
            .WithMany()
            .HasForeignKey(h => h.EstadoNuevoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UsuarioSistema>()
            .WithMany()
            .HasForeignKey(h => h.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => new
        {
            h.TramiteId,
            h.FechaCambio
        });
    }
}
