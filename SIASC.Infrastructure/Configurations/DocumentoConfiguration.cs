using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class DocumentoConfiguration
    : IEntityTypeConfiguration<Documento>
{
    public void Configure(EntityTypeBuilder<Documento> builder)
    {
        builder.ToTable("Documentos", table =>
        {
            table.HasCheckConstraint(
                "CK_Documentos_TamanoPositivo",
                "TamanoBytes > 0");
        });

        builder.HasKey(d => d.DocumentoId);

        builder.Property(d => d.TipoDocumento)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.NombreOriginal)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.NombreAlmacenado)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.TipoMime)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(d => d.HashSHA256)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(d => d.Observaciones)
            .HasMaxLength(1000);

        builder.HasOne(d => d.Tramite)
            .WithMany()
            .HasForeignKey(d => d.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.NombreAlmacenado)
            .IsUnique();

        builder.HasIndex(d => new
        {
            d.TramiteId,
            d.TipoDocumento
        });
    }
}
