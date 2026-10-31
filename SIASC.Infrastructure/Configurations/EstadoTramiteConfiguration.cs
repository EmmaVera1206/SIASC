using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class EstadoTramiteConfiguration
    : IEntityTypeConfiguration<EstadoTramite>
{
    public void Configure(EntityTypeBuilder<EstadoTramite> builder)
    {
        builder.ToTable("EstadosTramite");

        builder.HasKey(e => e.EstadoId);

        builder.Property(e => e.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(e => e.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(e => e.Descripcion)
            .HasMaxLength(500);

        builder.HasIndex(e => e.Codigo)
            .IsUnique();
    }
}
