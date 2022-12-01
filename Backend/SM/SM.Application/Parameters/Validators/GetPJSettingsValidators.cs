using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Parameters.Queries;
using SM.Domain.Enum;
using System.Linq;

namespace SM.Application.Parameters.Validators
{
    public class GetPJSettingsValidators
         : AbstractValidator<GetPJSettingsRequest>
    {
        public GetPJSettingsValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
            .Custom((request, context) =>
            {

                var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            //check acess in Levels - Parameter 
                            var expParameter = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Parameters);
                            //check subitems 

                            if (expParameter != null &&
                        expParameter.SubItems.Any(a => a == (long)ModulesSuItemsEnum.SettingsPJ))
                {
                    context.AddFailure(
                      new ValidationFailure("Parâmetros",
                      $"Você não tem acesso a tela Configurações PJ"));
                }

                if (!request.IsAdmin)
                    context.AddFailure(new ValidationFailure("IsAdmin", "Somente os administradores podem acessar os dados de Configurações PJ."));

            });
        }
    }
}
