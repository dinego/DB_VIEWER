using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Abstractions.Validators.Abstractions;
using CMC.Common.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.UserParameters.Queries;
using SM.Domain.Common;
using System.Collections.Generic;

namespace SM.Application.UserParameters.Validators
{
    public class UserValidators : AbstractValidator<GetUserInfoPermissionsRequest>
    {
        public UserValidators()
        {
            RuleFor(x => x)
                        .Custom((request, context) =>
                        {

                            if (!request.IsAdmin)
                                context.AddFailure(new ValidationFailure("IsAdmin", "Somente os administradores podem acessar os dados de Usuários."));

                        });
        }
    }
    public class UserValidatorPermissionRule : CompositeRule<PermissionJson>
    {
        private ValidatorResponse _validator;

        public UserValidatorPermissionRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(PermissionJson model)
        {
            bool isValid = model != null;
            if (!isValid)
                _validator.AddNotification("Não foi encontrado nenhuma regra de permissionamento para o usuário.");
            return isValid;
        }
    }

    public class UserValidatorProjectRule : CompositeRule<ProjectCompanyUser>
    {
        private ValidatorResponse _validator;

        public UserValidatorProjectRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(ProjectCompanyUser model)
        {
            bool isValid = model != null && model.CompanyFatherId.HasValue;
            if (!isValid)
                _validator.AddNotification("Não foi encontrado nenhum projeto associado a empresa do usuário.");
            return isValid;
        }
    }

    public class UserValidatorSalaryMarkProjectRule : CompositeRule<List<ProjetosSalaryMarkEmpresas>>
    {
        private ValidatorResponse _validator;

        public UserValidatorSalaryMarkProjectRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(List<ProjetosSalaryMarkEmpresas> model)
        {
            bool isValid = model != null && model.Count > 0;
            if (!isValid)
                _validator.AddNotification("Não foi encontrado nenhum projeto associado a empresa do usuário.");
            return isValid;
        }
    }

    public class UserValidatorGroupsRule : CompositeRule<List<GruposProjetosSalaryMark>>
    {
        private ValidatorResponse _validator;

        public UserValidatorGroupsRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(List<GruposProjetosSalaryMark> model)
        {
            bool isValid = model != null && model.Count > 0;
            if (!isValid)
                _validator.AddNotification("Não foi encontrado nenhum grupo associado a empresa do usuário.");
            return isValid;
        }
    }

    public class UserValidatorSalaryTableRule : CompositeRule<List<TabelasSalariais>>
    {
        private ValidatorResponse _validator;

        public UserValidatorSalaryTableRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(List<TabelasSalariais> model)
        {
            bool isValid = model != null && model.Count > 0;
            if (!isValid)
                _validator.AddNotification("Não foi encontrado nenhuma tabela salarial associado a empresa do usuário.");
            return isValid;
        }
    }
}
