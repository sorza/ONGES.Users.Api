using FluentValidation;
using ONGES.Users.Application.DTOs.Requests;
using ONGES.Users.Domain.Users.ValueObjects;

namespace ONGES.Users.Infrastructure.Validators
{
    public class UserRequestValidator : AbstractValidator<UserRequest>
    {
        public UserRequestValidator() 
        {
            RuleFor(x => x)
               .NotNull().WithMessage("A requisição não pode ser nula.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MaximumLength(150).WithMessage("O nome não pode exceder 150 caracteres.")
                .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("O email é obrigatório.")
                .EmailAddress().WithMessage("O email deve ser um endereço de email válido.")
                .MinimumLength(Email.MinLength).WithMessage($"O email deve ter pelo menos {Email.MinLength} caracteres.")
                .MaximumLength(Email.MaxLength).WithMessage($"O email não pode exceder {Email.MaxLength} caracteres.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(Password.MinLength).WithMessage($"A senha deve ter pelo menos {Password.MinLength} caracteres.")
                .MaximumLength(Password.MaxLength).WithMessage($"A senha não pode exceder {Password.MaxLength} caracteres.");
        }      
    }
}
