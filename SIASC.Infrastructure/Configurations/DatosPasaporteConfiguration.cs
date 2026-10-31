using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class DatosPasaporteConfiguration
    : IEntityTypeConfiguration<DatosPasaporte>
{
    public void Configure(EntityTypeBuilder<DatosPasaporte> builder)
    {
        builder.ToTable("DatosPasaporte");

        builder.HasKey(d => d.DatosPasaporteId);

        builder.Property(d => d.CURP)
            .IsRequired()
            .HasMaxLength(18);

        builder.Property(d => d.Observaciones)
            .HasMaxLength(2000);

        builder.HasOne(d => d.Tramite)
            .WithOne()
            .HasForeignKey<DatosPasaporte>(d => d.TramiteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => d.TramiteId)
            .IsUnique();
    }
}
