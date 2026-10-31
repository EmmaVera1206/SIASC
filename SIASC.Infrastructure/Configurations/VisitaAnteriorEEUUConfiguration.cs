using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SIASC.Domain.Entities;

namespace SIASC.Infrastructure.Configurations;

public class VisitaAnteriorEEUUConfiguration
    : IEntityTypeConfiguration<VisitaAnteriorEEUU>
{
    public void Configure(EntityTypeBuilder<VisitaAnteriorEEUU> builder)
    {
        builder.ToTable("VisitasAnterioresEEUU");

        builder.HasKey(v => v.VisitaAnteriorEEUUId);

        builder.Property(v => v.DuracionEstancia)
            .HasMaxLength(100);

        builder.HasOne(v => v.FormularioVisa)
            .WithMany(f => f.VisitasAnteriores)
            .HasForeignKey(v => v.FormularioVisaId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
