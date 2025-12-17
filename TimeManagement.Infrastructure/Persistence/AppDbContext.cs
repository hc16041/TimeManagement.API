using Microsoft.EntityFrameworkCore;
using TimeManagement.Application.Common.Interfaces;
using TimeManagement.Domain.Entities;

namespace TimeManagement.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Request> Requests { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Employee> Employees { get; set; }

    public DbSet<TimeBalance> TimeBalances { get; set; }

    // ... resto de tablas

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configuraciones Fluent API si son necesarias
        base.OnModelCreating(modelBuilder);
    }

    // Implementación de SaveChangesAsync de la interfaz
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}