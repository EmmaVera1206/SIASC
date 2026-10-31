using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Identity;

namespace SIASC.Infrastructure.Configurations;

public class SeguimientoConfiguration
    : IEntityTypeConfiguration<Seguimiento>
{
    public void Configure(EntityTypeBuilder<Seguimiento> builder)
    {
        builder.ToTable("Seguimientos");

        builder.HasKey(s => s.SeguimientoId);

        builder.Property(s => s.Descripcion)
            .IsRequired()
            .HasMaxLength(3000);

        builder.Property(s => s.FechaRegistro)
            .IsRequired();

        builder.Property(s => s.UsuarioId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasOne(s => s.Tramite)
            .WithMany()
            .HasForeignKey(s => s.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UsuarioSistema>()
            .WithMany()
            .HasForeignKey(s => s.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new
        {
            s.TramiteId,
            s.FechaRegistro
        });
    }
}
