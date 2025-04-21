using Routes.Domain.ViewModels;
using FluentValidation;
using Routes.Domain.ViewModels;

namespace Routes.Domain.Validators;

public class AlunoAdicionarViewModelValidator : AbstractValidator<AlunoAdicionarViewModel>
{
    public AlunoAdicionarViewModelValidator()
    {
        RuleFor(e => e.PrimeiroNome)
            .NotEmpty().WithMessage("O primeiro nome é obrigatório.")
            .MinimumLength(3).WithMessage("O primeiro nome deve ter pelo menos 3 caracteres.");

        RuleFor(e => e.UltimoNome)
            .NotEmpty().WithMessage("O ultimo nome é obrigatório.")
            .MinimumLength(3).WithMessage("O ultimo nome deve ter pelo menos 3 caracteres.");

        RuleFor(e => e.Contato)
            .MinimumLength(8)
            .MaximumLength(9).WithMessage("O contato deve ter 8 ou 9 caracteres.");

        RuleFor(e => e.Email)
            .MinimumLength(6).WithMessage("O email deve ter 6 caracteres.");

        RuleFor(e => e.EnderecoPartidaId)
            .GreaterThan(0).WithMessage("O endereço deve ser informada.");
    }
}

public class AlunoViewModelValidator : AbstractValidator<AlunoViewModel>
{
    public AlunoViewModelValidator()
    {
        RuleFor(e => e.PrimeiroNome)
            .NotEmpty().WithMessage("O primeiro nome é obrigatório.")
            .MinimumLength(3).WithMessage("O primeiro nome deve ter pelo menos 3 caracteres.");

        RuleFor(e => e.UltimoNome)
            .NotEmpty().WithMessage("O ultimo nome é obrigatório.")
            .MinimumLength(3).WithMessage("O ultimo nome deve ter pelo menos 3 caracteres.");

        RuleFor(e => e.Contato)
            .MinimumLength(8)
            .MaximumLength(9).WithMessage("O contato deve ter 8 ou 9 caracteres.");

        RuleFor(e => e.Email)
            .MinimumLength(5).WithMessage("O email deve ter 5 caracteres.")
            .EmailAddress().WithMessage("Email informado é inválido");

        RuleFor(e => e.ResponsavelId)
            .GreaterThan(0).WithMessage("O responsável deve ser informado.");

        RuleFor(e => e.EmpresaId)
            .GreaterThan(0).WithMessage("A Empresa deve ser informada.");

        RuleFor(e => e.EnderecoPartidaId)
            .GreaterThan(0).WithMessage("O endereço deve ser informada.");
    }
}