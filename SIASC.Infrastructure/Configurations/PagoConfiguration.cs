using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class PagoConfiguration : IEntityTypeConfiguration<Pago>
{
    public void Configure(EntityTypeBuilder<Pago> builder)
    {
        builder.ToTable("Pagos", table =>
        {
            table.HasCheckConstraint(
                "CK_Pagos_MontoPositivo",
                "MontoCentavos > 0");
        });

        builder.HasKey(p => p.PagoId);

        builder.Property(p => p.MontoCentavos)
            .IsRequired();

        builder.Property(p => p.FechaPago)
            .IsRequired();

        builder.Property(p => p.MetodoPago)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.TipoPago)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(p => p.Referencia)
            .HasMaxLength(150);

        builder.Property(p => p.Observaciones)
            .HasMaxLength(1000);

        builder.Property(p => p.MotivoAnulacion)
            .HasMaxLength(1000);

        builder.Property(p => p.Anulado)
            .HasDefaultValue(false);

        builder.HasOne(p => p.Tramite)
            .WithMany()
            .HasForeignKey(p => p.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.TramiteId);

        builder.HasIndex(p => p.FechaPago);

        builder.HasIndex(p => new
        {
            p.TramiteId,
            p.Anulado
        });
    }
}
