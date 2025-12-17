using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeManagement.Application.Common.Interfaces;
using TimeManagement.Domain.Enums;

namespace TimeManagement.Application.Features.Requests.Queries.GetPendingRequests;

// DTO de Salida
public record RequestSummaryDto(int Id, string EmployeeName, string Type, DateTime Date, string Status);

// Query
public record GetPendingRequestsQuery(int ApproverId) : IRequest<List<RequestSummaryDto>>;

// Handler
public class GetPendingRequestsQueryHandler : IRequestHandler<GetPendingRequestsQuery, List<RequestSummaryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingRequestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RequestSummaryDto>> Handle(GetPendingRequestsQuery request, CancellationToken cancellationToken)
    {
        // Buscar todas las solicitudes donde "Yo" soy el CurrentApproverId
        return await _context.Requests
            .Where(r => r.CurrentApproverId == request.ApproverId && r.Status != RequestStatusEnum.Rechazado)
            .Select(r => new RequestSummaryDto(
                r.Id,
                r.Employee.FullName,
                r.RequestType.ToString(),
                r.StartDate,
                r.Status.ToString()
            ))
            .ToListAsync(cancellationToken);
    }
}