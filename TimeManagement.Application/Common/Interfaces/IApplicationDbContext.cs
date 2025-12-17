using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TimeManagement.Domain.Entities;

namespace TimeManagement.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Request> Requests { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Department> Departments { get; }
    DbSet<TimeBalance> TimeBalances { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}