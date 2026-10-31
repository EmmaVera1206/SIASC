using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class AcompananteViajeConfiguration
    : IEntityTypeConfiguration<AcompananteViaje>
{
    public void Configure(EntityTypeBuilder<AcompananteViaje> builder)
    {
        builder.ToTable("AcompanantesViaje");

        builder.HasKey(a => a.AcompananteViajeId);

        builder.Property(a => a.NombreCompleto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Parentesco)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(a => a.FormularioVisa)
            .WithMany(f => f.Acompanantes)
            .HasForeignKey(a => a.FormularioVisaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

