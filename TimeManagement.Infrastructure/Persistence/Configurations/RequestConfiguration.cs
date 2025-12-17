using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeManagement.Domain.Entities;

namespace TimeManagement.Infrastructure.Persistence.Configurations;

public class RequestConfiguration : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("Requests");

        builder.HasKey(t => t.Id);

        // Convertir Enum a String en BD (más legible) o Int (más rápido). Usaremos String por claridad.
        builder.Property(t => t.RequestType)
            .HasConversion<string>();

        builder.Property(t => t.Status)
            .HasConversion<string>();

        builder.Property(t => t.TotalHours)
            .HasPrecision(10, 2);

        // Relación con Empleado
        builder.HasOne(r => r.Employee)
            .WithMany()
            .HasForeignKey(r => r.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict); // No borrar solicitud si se borra empleado
    }
}