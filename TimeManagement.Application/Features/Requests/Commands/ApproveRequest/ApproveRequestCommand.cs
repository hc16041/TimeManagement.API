using MediatR;
using Microsoft.EntityFrameworkCore;
using TimeManagement.Application.Common.Interfaces;
using TimeManagement.Domain.Enums;
// Asegúrate que NO haya ambigüedad con otros tipos llamada 'Request'

namespace TimeManagement.Application.Features.Requests.Commands.ApproveRequest;

// Comando
public record ApproveRequestCommand(int RequestId, int ApproverId, bool IsApproved, string? Comment) : IRequest<bool>;

// Handler
public class ApproveRequestCommandHandler : IRequestHandler<ApproveRequestCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ApproveRequestCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ApproveRequestCommand command, CancellationToken cancellationToken)
    {
        // Nota: usamos 'command' aquí, no 'request' para evitar confusión con la entidad Request
        var entity = await _context.Requests
            .Include(r => r.Employee)
            .ThenInclude(e => e.Department)
            .FirstOrDefaultAsync(r => r.Id == command.RequestId, cancellationToken);

        if (entity == null) throw new KeyNotFoundException($"Solicitud {command.RequestId} no encontrada");

        // 1. Lógica de Rechazo
        if (!command.IsApproved)
        {
            entity.Status = RequestStatusEnum.Rechazado;
            entity.CurrentApproverId = null;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        // 2. Máquina de Estados
        switch (entity.Status)
        {
            case RequestStatusEnum.PendienteJefe:
                entity.Status = RequestStatusEnum.PendienteGerencia;
                entity.CurrentApproverId = entity.Employee.Department.ManagerId;
                break;

            case RequestStatusEnum.PendienteGerencia:
                entity.Status = RequestStatusEnum.Aprobado;
                entity.CurrentApproverId = null;

                // Descontar saldo
                await DeductBalance(entity, cancellationToken);
                break;

            // AGREGAR ESTE CASO NUEVO:
            case RequestStatusEnum.Aprobado:
                // Si ya está aprobado y alguien lo firma de nuevo (RRHH), lo pasamos a Procesado
                entity.Status = RequestStatusEnum.Procesado; // Status 6
                // Aquí podrías agregar lógica de integración con sistema de nómina externo
                break;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Método auxiliar
    private async Task DeductBalance(Domain.Entities.Request requestEntity, CancellationToken ct)
    {
        var balance = await _context.TimeBalances
            .FirstOrDefaultAsync(b => b.EmployeeId == requestEntity.EmployeeId && b.Year == DateTime.Now.Year, ct);

        if (balance != null)
        {
            if (requestEntity.RequestType == RequestTypeEnum.Vacaciones)
                balance.VacationHoursAvailable -= requestEntity.TotalHours;

            else if (requestEntity.RequestType == RequestTypeEnum.SemanaTurno)
                balance.CompensatoryHoursAvailable -= requestEntity.TotalHours;
        }
    }
}