using CMC.Common.Repositories;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Parameters.Queries;
using SM.Domain.Enum;
using System.Linq;

namespace SM.Application.Parameters.Validators
{
    public class GetLevelsConfigurationRequestValidators
        : AbstractValidator<GetLevelsConfigurationRequest>
    {
        public GetLevelsConfigurationRequestValidators(IUnitOfWork unitOfWork,
            IPermissionUserInteractor permissionUserInteractor)
        {

            RuleFor(x => x)
                        .Custom((request, context) =>
                        {

                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            //check acess in Levels - Parameter 
                            var expLevelsParameter = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Parameters);
                            //check subitems 

                            if (expLevelsParameter != null &&
                                    expLevelsParameter.SubItems.Any(a => a == (long)ModulesSuItemsEnum.Levels))
                            {
                                context.AddFailure(
                                  new ValidationFailure("Parâmetros",
                                  $"Você não tem acesso a tela Níveis- Parâmetros"));
                            }

                            if (!request.IsAdmin)
                                context.AddFailure(new ValidationFailure("IsAdmin", "Somente os administradores podem acessar os dados de Níveis."));

                        });

        }
    }
}
