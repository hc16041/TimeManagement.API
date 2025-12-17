namespace TimeManagement.Application.DTOs;

public record RequestListDto(
    int Id,
    string EmployeeName,
    string RequestType,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalHours,
    string Status,
    string Justification
);