using TimeManagement.Domain.Common;
using TimeManagement.Domain.Enums;

namespace TimeManagement.Domain.Entities;

public class Request : BaseAuditableEntity
{
    // Relación con el Solicitante 
    public int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!; // <--- Esta faltaba

    // Datos de la Solicitud
    public RequestTypeEnum RequestType { get; set; } // 
    public RequestStatusEnum Status { get; set; } = RequestStatusEnum.PendienteJefe;

    public DateTime StartDate { get; set; } // 
    public DateTime EndDate { get; set; }
    public decimal TotalHours { get; set; } // Para controlar medios días
    public string Justification { get; set; } = string.Empty; // 
    public string? AttachmentUrl { get; set; } // 

    // Flujo de Aprobación
    public int? CurrentApproverId { get; set; } // <--- Esta faltaba
    public virtual Employee? CurrentApprover { get; set; } // <--- Esta faltaba
}