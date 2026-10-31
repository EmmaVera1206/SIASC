using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class FamiliarVisaConfiguration
    : IEntityTypeConfiguration<FamiliarVisa>
{
    public void Configure(EntityTypeBuilder<FamiliarVisa> builder)
    {
        builder.ToTable("FamiliaresVisa");

        builder.HasKey(f => f.FamiliarVisaId);

        builder.Property(f => f.NombreCompleto)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(f => f.Parentesco)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.EstatusLegalEEUU)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(f => f.FormularioVisa)
            .WithMany(v => v.Familiares)
            .HasForeignKey(f => f.FormularioVisaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

