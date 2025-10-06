using FluentValidation;
using Entities.Models;
using System;

namespace WebApis.Validators
{
    public class AgendamentoCreateModelValidator : AbstractValidator<AgendamentoCreateModel>
    {
        public AgendamentoCreateModelValidator()
        {
            RuleFor(x => x.ContatoId)
                .GreaterThan(0);

            RuleFor(x => x.DataHora)
                .NotEmpty()
                .Must(d => d >= DateTime.UtcNow.AddMinutes(-1))
                .WithMessage("Data/hora não pode estar no passado.");

            RuleFor(x => x.Dscr)
                .MaximumLength(200);
        }
    }
}
