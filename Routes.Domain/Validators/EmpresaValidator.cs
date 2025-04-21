using Routes.Domain.Models;
using FluentValidation;

namespace Routes.Domain.Validators;

public class EmpresaValidator : AbstractValidator<Empresa>
{
    public EmpresaValidator()
    {
        RuleFor(e => e.NomeFantasia)
            .NotEmpty().WithMessage("O Nome Fantasia é obrigatório.")
            .MinimumLength(3).WithMessage("O Nome Fantasia deve ter pelo menos 3 caracteres.");

        RuleFor(e => e.RazaoSocial)
            .NotEmpty().WithMessage("A Razão Social é obrigatória.")
            .MinimumLength(3).WithMessage("A Razão Social deve ter pelo menos 3 caracteres.");

        RuleFor(e => e.Apelido)
            .MinimumLength(4)
            .MaximumLength(20).WithMessage("O Apelido não pode ter mais de 20 caracteres.");
    }
}