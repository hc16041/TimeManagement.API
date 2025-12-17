using MediatR;
using TimeManagement.Application.Common.Interfaces;
using TimeManagement.Domain.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace TimeManagement.Application.Features.Requests.Commands.CreateRequest;

// 1. El Comando: Es un DTO que representa la petición del usuario (Inputs)
// Devuelve un int (el ID de la solicitud creada)
public record CreateRequestCommand(
    int EmployeeId,
    RequestTypeEnum RequestType,
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalHours,
    string Justification
) : IRequest<int>;

// 2. El Handler: La lógica que ejecuta el comando
public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, int>
{
    // Usamos una interfaz para la DB (Inversión de Dependencia)
    private readonly IApplicationDbContext _context;

    public CreateRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        // 1. BUSCAR AL EMPLEADO (Necesitamos saber quién es su jefe: ReportsToId)
        var employee = await _context.Employees
            .FindAsync(new object[] { request.EmployeeId }, cancellationToken);

        if (employee == null)
            throw new KeyNotFoundException($"Empleado {request.EmployeeId} no encontrado");

        // Mapeo manual o con Mapster/AutoMapper
        var entity = new Domain.Entities.Request
        {
            EmployeeId = request.EmployeeId,
            RequestType = request.RequestType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalHours = request.TotalHours,
            Justification = request.Justification,
            Status = Domain.Enums.RequestStatusEnum.PendienteJefe,
            // ESTA ES LA LÍNEA QUE FALTABA:
            CurrentApproverId = employee.ReportsToId
        };

        _context.Requests.Add(entity);

        // Aquí se guardarían los logs o se dispararían eventos de dominio (Email)
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}