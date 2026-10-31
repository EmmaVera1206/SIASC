
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;
using SIASC.Infrastructure.Identity;

namespace SIASC.Infrastructure.Configurations;

public class HistorialCambioPagoConfiguration
    : IEntityTypeConfiguration<HistorialCambioPago>
{
    public void Configure(
        EntityTypeBuilder<HistorialCambioPago> builder)
    {
        builder.ToTable("HistorialCambiosPagos");

        builder.HasKey(h => h.CambioPagoId);

        builder.Property(h => h.UsuarioId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(h => h.TipoCambio)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(h => h.DatosAnteriores)
            .IsRequired();

        builder.Property(h => h.DatosNuevos);

        builder.Property(h => h.FechaCambio)
            .IsRequired();

        builder.Property(h => h.Motivo)
            .IsRequired()
            .HasMaxLength(1000);

        builder.HasOne(h => h.Pago)
            .WithMany()
            .HasForeignKey(h => h.PagoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UsuarioSistema>()
            .WithMany()
            .HasForeignKey(h => h.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(h => h.PagoId);

        builder.HasIndex(h => h.FechaCambio);
    }
}
