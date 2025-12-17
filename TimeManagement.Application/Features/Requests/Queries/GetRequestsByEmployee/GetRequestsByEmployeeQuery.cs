using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeManagement.Application.Common.Interfaces;
using TimeManagement.Application.DTOs;

namespace TimeManagement.Application.Features.Requests.Queries.GetRequestsByEmployee;

// 1. La Query: "Dame las solicitudes del empleado X"
public record GetRequestsByEmployeeQuery(int EmployeeId) : IRequest<List<RequestListDto>>;

// 2. El Handler
public class GetRequestsByEmployeeQueryHandler : IRequestHandler<GetRequestsByEmployeeQuery, List<RequestListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRequestsByEmployeeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RequestListDto>> Handle(GetRequestsByEmployeeQuery request, CancellationToken cancellationToken)
    {
        return await _context.Requests
            .AsNoTracking() // Optimización para lectura
            .Include(r => r.Employee)
            .Where(r => r.EmployeeId == request.EmployeeId)
            .OrderByDescending(r => r.CreatedAt) // Las más recientes primero
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