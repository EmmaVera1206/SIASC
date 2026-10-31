using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class TramiteConfiguration : IEntityTypeConfiguration<Tramite>
{
    public void Configure(EntityTypeBuilder<Tramite> builder)
    {
        builder.ToTable("Tramites", table =>
        {
            table.HasCheckConstraint(
                "CK_Tramites_CostoNoNegativo",
                "CostoTotalCentavos >= 0");
        });

        builder.HasKey(t => t.TramiteId);

        builder.Property(t => t.CostoTotalCentavos)
            .IsRequired();

        builder.Property(t => t.ObservacionesGenerales)
            .HasMaxLength(3000);

        builder.Property(t => t.MotivoCancelacion)
            .HasMaxLength(1000);

        builder.HasOne(t => t.Cliente)
            .WithMany(c => c.Tramites)
            .HasForeignKey(t => t.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.TipoServicio)
            .WithMany(s => s.Tramites)
            .HasForeignKey(t => t.TipoServicioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.EstadoActual)
            .WithMany()
            .HasForeignKey(t => t.EstadoActualId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.ClienteId);

        builder.HasIndex(t => t.TipoServicioId);

        builder.HasIndex(t => t.EstadoActualId);

        builder.HasIndex(t => t.FechaInicio);
    }
}

