using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class TransicionEstadoConfiguration
    : IEntityTypeConfiguration<TransicionEstado>
{
    public void Configure(EntityTypeBuilder<TransicionEstado> builder)
    {
        builder.ToTable("TransicionesEstado", table =>
        {
            table.HasCheckConstraint(
                "CK_Transiciones_EstadosDiferentes",
                "EstadoOrigenId <> EstadoDestinoId");
        });

        builder.HasKey(t => t.TransicionId);

        builder.HasOne(t => t.TipoServicio)
            .WithMany()
            .HasForeignKey(t => t.TipoServicioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.EstadoOrigen)
            .WithMany()
            .HasForeignKey(t => t.EstadoOrigenId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.EstadoDestino)
            .WithMany()
            .HasForeignKey(t => t.EstadoDestinoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new
        {
            t.TipoServicioId,
            t.EstadoOrigenId,
            t.EstadoDestinoId
        })
        .IsUnique();

        builder.HasIndex(t => t.EstadoDestinoId);
    }
}
