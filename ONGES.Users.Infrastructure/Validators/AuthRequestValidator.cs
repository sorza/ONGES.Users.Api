using FluentValidation;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Domain.Users.ValueObjects;

namespace ONGES.Users.Infrastructure.Validators
{
    public class AuthRequestValidator : AbstractValidator<AuthRequest>
    {
        public AuthRequestValidator() 
        {
            RuleFor(x => x)
               .NotNull().WithMessage("Informe os dados solicitados.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .EmailAddress().WithMessage("O email deve ser um endereço de email válido.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(Password.MinLength).WithMessage($"A senha deve ter pelo menos {Password.MinLength} caracteres.")
                .MaximumLength(Password.MaxLength).WithMessage($"A senha não pode exceder {Password.MaxLength} caracteres.");
        }      

    }
}
