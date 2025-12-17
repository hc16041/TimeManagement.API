using TimeManagement.Domain.Common;

namespace TimeManagement.Domain.Entities;

public class TimeBalance : BaseAuditableEntity
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    // 2024, 2025, etc.
    public int Year { get; set; }

    // Saldo Vacaciones (en horas para precisión)
    public decimal VacationHoursAvailable { get; set; }

    // Saldo Compensatorio (Horas Extra/Turnos)
    public decimal CompensatoryHoursAvailable { get; set; }
}