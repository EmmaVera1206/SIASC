using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class RedSocialVisaConfiguration
    : IEntityTypeConfiguration<RedSocialVisa>
{
    public void Configure(EntityTypeBuilder<RedSocialVisa> builder)
    {
        builder.ToTable("RedesSocialesVisa");

        builder.HasKey(r => r.RedSocialVisaId);

        builder.Property(r => r.Plataforma)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.NombreUsuario)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasOne(r => r.FormularioVisa)
            .WithMany(f => f.RedesSociales)
            .HasForeignKey(r => r.FormularioVisaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
