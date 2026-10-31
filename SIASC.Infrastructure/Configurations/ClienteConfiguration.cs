using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.ClienteId);

        builder.Property(c => c.NombreCompleto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Telefono)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(c => c.CorreoElectronico)
            .HasMaxLength(200);

        builder.Property(c => c.RecomendadoPor)
            .HasMaxLength(200);

        builder.HasIndex(c => c.NombreCompleto);

        builder.HasIndex(c => c.Telefono);

        builder.HasMany(c => c.Tramites)
            .WithOne(t => t.Cliente)
            .HasForeignKey(t => t.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

