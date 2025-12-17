using FluentValidation;

namespace TimeManagement.Application.Features.Requests.Commands.CreateRequest;

public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(v => v.EmployeeId).GreaterThan(0);
        RuleFor(v => v.RequestType).IsInEnum();

        RuleFor(v => v.StartDate)
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("La fecha no puede ser en el pasado.");

        RuleFor(v => v.EndDate)
            .GreaterThan(v => v.StartDate).WithMessage("La fecha fin debe ser mayor a la fecha de inicio.");

        RuleFor(v => v.TotalHours)
            .GreaterThan(0)
            .LessThanOrEqualTo(120).WithMessage("No puedes pedir más de 15 días continuos en una sola solicitud."); // Regla de negocio implícita

        RuleFor(v => v.Justification)
            .NotEmpty().WithMessage("La justificación es obligatoria."); // 
    }
}