using FluentValidation;
using Entities.Models;
using System;

namespace WebApis.Validators
{
    public class AgendamentoModelValidator : AbstractValidator<AgendamentoModel>
    {
        public AgendamentoModelValidator()
        {
            RuleFor(a => a.DataHora)
                .NotEmpty().WithMessage("Data/hora é obrigatória.")
                .Must(d => d >= DateTime.UtcNow.AddMinutes(-1)).WithMessage("Data/hora não pode estar no passado.");

            RuleFor(a => a.Dscr)
                .MaximumLength(200);
        }
    }

    public class ContatoModelValidator : AbstractValidator<ContatoModel>
    {
        public ContatoModelValidator()
        {
            RuleFor(c => c.Nome)
                .NotEmpty().WithMessage("Nome é obrigatório.")
                .MaximumLength(100);

            RuleFor(c => c.Email)
                .MaximumLength(200)
                .EmailAddress().When(c => !string.IsNullOrWhiteSpace(c.Email));

            RuleFor(c => c.Telefone)
                .MaximumLength(20);

            RuleForEach(c => c.Agendamentos)
                .SetValidator(new AgendamentoModelValidator());
        }
    }
}
