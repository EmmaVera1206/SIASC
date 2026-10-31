using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class CitaConfiguration : IEntityTypeConfiguration<Cita>
{
    public void Configure(EntityTypeBuilder<Cita> builder)
    {
        builder.ToTable("Citas");

        builder.HasKey(c => c.CitaId);

        builder.Property(c => c.TipoCita)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.EstadoCita)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.Lugar)
            .HasMaxLength(300);

        builder.Property(c => c.Observaciones)
            .HasMaxLength(2000);

        builder.HasOne(c => c.Tramite)
            .WithMany()
            .HasForeignKey(c => c.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.FechaTentativa);

        builder.HasIndex(c => c.FechaConfirmada);

        builder.HasIndex(c => new
        {
            c.TramiteId,
            c.EstadoCita
        });
    }
}
