using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class TipoServicioConfiguration
    : IEntityTypeConfiguration<TipoServicio>
{
    public void Configure(EntityTypeBuilder<TipoServicio> builder)
    {
        builder.ToTable("TiposServicio");

        builder.HasKey(t => t.TipoServicioId);

        builder.Property(t => t.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(t => t.Descripcion)
            .HasMaxLength(500);

        builder.HasIndex(t => t.Codigo)
            .IsUnique();
    }
}

