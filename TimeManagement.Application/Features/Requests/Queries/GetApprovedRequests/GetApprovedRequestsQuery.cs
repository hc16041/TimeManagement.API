using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeManagement.Application.Common.Interfaces;
using TimeManagement.Application.DTOs;
using TimeManagement.Domain.Enums;

namespace TimeManagement.Application.Features.Requests.Queries.GetApprovedRequests;

// Query: No pide ID de usuario, porque RRHH ve TODO
public record GetApprovedRequestsQuery : IRequest<List<RequestListDto>>;

public class GetApprovedRequestsQueryHandler : IRequestHandler<GetApprovedRequestsQuery, List<RequestListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetApprovedRequestsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RequestListDto>> Handle(GetApprovedRequestsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Requests
            .AsNoTracking()
            .Include(r => r.Employee)
            .ThenInclude(e => e.Department) // Incluimos depa para que RRHH sepa de dónde son
                                            // Filtro: Solo lo que ya autorizó Gerencia (Status 4)
            .Where(r => r.Status == RequestStatusEnum.Aprobado)
            .OrderBy(r => r.StartDate)
            .Select(r => new RequestListDto(
                r.Id,
                r.Employee.FullName, // Podrías concatenar " - " + r.Employee.Department.Name
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