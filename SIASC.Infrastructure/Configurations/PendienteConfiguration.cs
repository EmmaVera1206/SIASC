using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class PendienteConfiguration
    : IEntityTypeConfiguration<Pendiente>
{
    public void Configure(EntityTypeBuilder<Pendiente> builder)
    {
        builder.ToTable("Pendientes");

        builder.HasKey(p => p.PendienteId);

        builder.Property(p => p.Titulo)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Descripcion)
            .HasMaxLength(2000);

        builder.Property(p => p.Prioridad)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.Estado)
            .IsRequired()
            .HasMaxLength(30);

        builder.HasOne(p => p.Tramite)
            .WithMany()
            .HasForeignKey(p => p.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.FechaVencimiento);

        builder.HasIndex(p => new
        {
            p.Estado,
            p.FechaVencimiento
        });
    }
}
