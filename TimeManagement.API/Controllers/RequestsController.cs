using MediatR;
using Microsoft.AspNetCore.Mvc;
using TimeManagement.Application.Features.Requests.Commands.ApproveRequest; // Asegúrate de importar esto
using TimeManagement.Application.Features.Requests.Commands.CreateRequest;
using TimeManagement.Application.Features.Requests.Queries.GetApprovedRequests;
using TimeManagement.Application.Features.Requests.Queries.GetPendingApprovals;
using TimeManagement.Application.Features.Requests.Queries.GetRequestsByEmployee;

namespace TimeManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // 1. CREAR SOLICITUD (Ya lo tenías)
    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateRequestCommand command)
    {
        var requestId = await _mediator.Send(command);
        return Ok(requestId);
    }

    // 2. APROBAR O RECHAZAR SOLICITUD (Nuevo)
    [HttpPost("approve")]
    public async Task<ActionResult> Approve(ApproveRequestCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(new { Success = result, Message = "Acción procesada correctamente" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Solicitud no encontrada");
        }
    }

    // 3. HISTORIAL DEL EMPLEADO (Nuevo)
    // Ejemplo: GET api/requests/history/5 (Donde 5 es el ID del empleado)
    [HttpGet("history/{employeeId}")]
    public async Task<ActionResult> GetMyHistory(int employeeId)
    {
        var query = new GetRequestsByEmployeeQuery(employeeId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // 4. BANDEJA DE ENTRADA DEL JEFE (Nuevo)
    // Ejemplo: GET api/requests/pending/2 (Donde 2 es el ID del Jefe Emanuel Acevedo)
    [HttpGet("pending/{approverId}")]
    public async Task<ActionResult> GetPending(int approverId)
    {
        var query = new GetPendingApprovalsQuery(approverId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // 5. BANDEJA DE RRHH (Nuevo)
    [HttpGet("approved")]
    public async Task<ActionResult> GetApproved()
    {
        var query = new GetApprovedRequestsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}