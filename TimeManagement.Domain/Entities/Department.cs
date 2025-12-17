using TimeManagement.Domain.Common;

namespace TimeManagement.Domain.Entities;

public class Department : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string CostCenterCode { get; set; } = string.Empty;

    // ID del Gerente del área (referencia circular suelta para evitar loops en EF)
    public int? ManagerId { get; set; }
}