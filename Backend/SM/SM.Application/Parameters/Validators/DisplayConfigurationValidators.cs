using System.Collections.Generic;
using System.Linq;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Abstractions.Validators.Abstractions;
using CMC.Common.Extensions;
using SM.Domain.Common;
using SM.Domain.Enum;

namespace SM.Application.Parameters.Validators
{
    public class DisplayConfigurationValidator
    {
        public bool IsAdmin { get; set; }
    }
    public class DisplayConfigurationRule : CompositeRule<DisplayConfigurationValidator>
    {
        private ValidatorResponse _validator;

        public DisplayConfigurationRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(DisplayConfigurationValidator request)
        {
            bool isValid = request.IsAdmin;
            if (!isValid)
                _validator.AddNotification("Você não tem acesso a tela Configurar Exibição");
            return isValid;
        }
    }

    public class DisplayConfigurationModuleValidator
    {
        public IEnumerable<FieldCheckedUserJson> Modules { get; set; }
        public ModulesSuItemsEnum? SubModules { get; set; }
    }

    public class DisplayConfigurationModulesRule : CompositeRule<DisplayConfigurationModuleValidator>
    {
        private ValidatorResponse _validator;

        public DisplayConfigurationModulesRule(ValidatorResponse validator)
        {
            _validator = validator;
        }
        public override bool IsSatisfiedBy(DisplayConfigurationModuleValidator request)
        {
            if (request.SubModules.HasValue)
                return !request.Modules.Safe().Any(m => m.Id == (long)ModulesEnum.Parameters &&
                                                              m.SubItems.Contains((long)request.SubModules));

            var lstSubModules = new List<long> { (long)ModulesSuItemsEnum.GlobalLabels, (long)ModulesSuItemsEnum.DisplayConfiguration };
            bool doesNotHaveAccess = request.Modules.Safe().Any(m => m.Id == (long)ModulesEnum.Parameters &&
                                              m.SubItems.Count() == lstSubModules.Count());
            if (doesNotHaveAccess)
                _validator.AddNotification("Você não tem acesso a tela Configurar Exibição");
            return !doesNotHaveAccess;
        }
    }
}
