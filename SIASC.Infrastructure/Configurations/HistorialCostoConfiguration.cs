using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Identity;

namespace SIASC.Infrastructure.Configurations;

public class HistorialCostoConfiguration
    : IEntityTypeConfiguration<HistorialCosto>
{
    public void Configure(
        EntityTypeBuilder<HistorialCosto> builder)
    {
        builder.ToTable("HistorialCostos", table =>
        {
            table.HasCheckConstraint(
                "CK_HistorialCostos_AnteriorNoNegativo",
                "CostoAnteriorCentavos >= 0");

            table.HasCheckConstraint(
                "CK_HistorialCostos_NuevoNoNegativo",
                "CostoNuevoCentavos >= 0");
        });

        builder.HasKey(h => h.HistorialCostoId);

        builder.Property(h => h.CostoAnteriorCentavos)
            .IsRequired();

        builder.Property(h => h.CostoNuevoCentavos)
            .IsRequired();

        builder.Property(h => h.FechaCambio)
            .IsRequired();

        builder.Property(h => h.Motivo)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(h => h.UsuarioId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasOne(h => h.Tramite)
            .WithMany()
            .HasForeignKey(h => h.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UsuarioSistema>()
            .WithMany()
            .HasForeignKey(h => h.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => h.TramiteId);

        builder.HasIndex(h => h.FechaCambio);
    }
}

