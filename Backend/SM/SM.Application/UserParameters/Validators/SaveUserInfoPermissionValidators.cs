using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Abstractions.Validators.Abstractions;
using CMC.Common.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.UserParameters.Command;
using System.Linq;

namespace SM.Application.UserParameters.Validators
{
    public class SaveUserInfoPermissionValidators : AbstractValidator<SaveUserInformationRequest>
    {
        public SaveUserInfoPermissionValidators()
        {
            RuleFor(x => x)
                        .Custom((request, context) => {

                            if (!request.IsAdmin)
                                context.AddFailure(new ValidationFailure("IsAdmin", "Somente os administradores podem acessar os dados de Usuários."));

                        });
        }
    }

    public class SaveUserValidatorPermissionRule : CompositeRule<Usuarios>
    {
        private ValidatorResponse _validator;

        public SaveUserValidatorPermissionRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(Usuarios model)
        {
            bool isValid = model != null && model.UsuarioPermissaoSm != null && model.UsuarioPermissaoSm.FirstOrDefault() != null;
            if (!isValid)
                _validator.AddNotification("Não foi encontrado nenhuma regra de permissionamento para o usuário.");
            return isValid;
        }
    }
}
