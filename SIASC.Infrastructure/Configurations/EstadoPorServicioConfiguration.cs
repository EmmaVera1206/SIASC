using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class EstadoPorServicioConfiguration
    : IEntityTypeConfiguration<EstadoPorServicio>
{
    public void Configure(EntityTypeBuilder<EstadoPorServicio> builder)
    {
        builder.ToTable("EstadosPorServicio");

        builder.HasKey(e => new
        {
            e.TipoServicioId,
            e.EstadoId
        });

        builder.HasOne(e => e.TipoServicio)
            .WithMany()
            .HasForeignKey(e => e.TipoServicioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Estado)
            .WithMany()
            .HasForeignKey(e => e.EstadoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.EstadoId);
    }
}

