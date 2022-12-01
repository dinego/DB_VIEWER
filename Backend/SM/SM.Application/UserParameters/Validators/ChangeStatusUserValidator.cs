using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Abstractions.Validators.Abstractions;
using CMC.Common.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.UserParameters.Command;

namespace SM.Application.UserParameters.Validators
{
    public class ChangeStatusUserValidator : AbstractValidator<ChangeStatusUserRequest>
    {
        public ChangeStatusUserValidator()
        {
            RuleFor(x => x)
                        .Custom((request, context) => {

                            if (!request.IsAdmin)
                                context.AddFailure(new ValidationFailure("IsAdmin", "Somente os administradores podem acessar os dados de Usuários."));

                        });
        }
    }

    public class ChangeStatusUserValidatorRule : CompositeRule<Usuarios>
    {
        private ValidatorResponse _validator;

        public ChangeStatusUserValidatorRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(Usuarios model)
        {
            bool isValid = model != null;
            if (!isValid)
                _validator.AddNotification("Usuário não encontrado.");
            return isValid;
        }
    }
}
