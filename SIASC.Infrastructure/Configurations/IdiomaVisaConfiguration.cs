using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class IdiomaVisaConfiguration
    : IEntityTypeConfiguration<IdiomaVisa>
{
    public void Configure(EntityTypeBuilder<IdiomaVisa> builder)
    {
        builder.ToTable("IdiomasVisa");

        builder.HasKey(i => i.IdiomaVisaId);

        builder.Property(i => i.Idioma)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(i => i.FormularioVisa)
            .WithMany(f => f.Idiomas)
            .HasForeignKey(i => i.FormularioVisaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
