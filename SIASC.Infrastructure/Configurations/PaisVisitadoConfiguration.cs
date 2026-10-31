using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class PaisVisitadoConfiguration
    : IEntityTypeConfiguration<PaisVisitado>
{
    public void Configure(EntityTypeBuilder<PaisVisitado> builder)
    {
        builder.ToTable("PaisesVisitados");

        builder.HasKey(p => p.PaisVisitadoId);

        builder.Property(p => p.Pais)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(p => p.FormularioVisa)
            .WithMany(f => f.PaisesVisitados)
            .HasForeignKey(p => p.FormularioVisaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}


