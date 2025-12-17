using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeManagement.Application.Common.Interfaces;
using TimeManagement.Application.DTOs;
using TimeManagement.Domain.Enums;

namespace TimeManagement.Application.Features.Requests.Queries.GetPendingApprovals;

// 1. La Query: "Dame lo que tengo pendiente de firmar YO"
public record GetPendingApprovalsQuery(int ApproverId) : IRequest<List<RequestListDto>>;

// 2. El Handler
public class GetPendingApprovalsQueryHandler : IRequestHandler<GetPendingApprovalsQuery, List<RequestListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPendingApprovalsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RequestListDto>> Handle(GetPendingApprovalsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Requests
            .AsNoTracking()
            .Include(r => r.Employee)
            .Where(r => r.CurrentApproverId == request.ApproverId
                     && r.Status != RequestStatusEnum.Rechazado
                     && r.Status != RequestStatusEnum.Procesado)
            .Select(r => new RequestListDto(
                r.Id,
                r.Employee.FullName,
                r.RequestType.ToString(),
                r.StartDate,
                r.EndDate,
                r.TotalHours,
                r.Status.ToString(),
                r.Justification
            ))
            .ToListAsync(cancellationToken);
    }
}