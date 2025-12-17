using TimeManagement.Domain.Common;

namespace TimeManagement.Domain.Entities;

public class Employee : BaseAuditableEntity
{
    public string EmployeeCode { get; set; } = string.Empty; // 
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Jerarquía
    public int? ReportsToId { get; set; }
    public Employee? ReportsTo { get; set; } // Jefe Inmediato 

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    // Saldos
    public virtual ICollection<TimeBalance> TimeBalances { get; set; } = new List<TimeBalance>();
}