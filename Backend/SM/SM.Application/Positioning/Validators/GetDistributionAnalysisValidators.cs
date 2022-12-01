using CMC.Common.Extensions;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum;
using System;
using System.Linq;

namespace SM.Application.Positioning.Validators
{
    public class GetDistributionAnalysisValidators
        : AbstractValidator<GetDistributionAnalysisRequest>
    {
        public GetDistributionAnalysisValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                            .Custom((request, context) =>
                            {
                                var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                                            var expDistributionAnalysis = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Positioning);
                                            //check subitems 

                                            if (expDistributionAnalysis != null &&
                                    expDistributionAnalysis.SubItems.Any(a => a == (long)ModulesSuItemsEnum.DistributionAnalysis))
                                {
                                    context.AddFailure(
                                      new ValidationFailure("Posicionamento",
                                      $"Você não tem permissão para acessar a tela de Análise de Distribuição"));
                                }


                                if (permissionResult.Display != null &&
                                    !permissionResult.Display.Scenario.Safe()
                                    .Any(sc => sc.Id == (long)request.Scenario && sc.IsChecked))
                                    {
                                                    context.AddFailure(
                                                    new ValidationFailure("Posicionamento",
                                                    $"Você não tem acesso a este a: {request.Scenario.GetDescription()}"));
                                                
                                            }

                            });
        }
    }
}
