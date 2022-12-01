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
    public class GetFinancialImpactValidators
         : AbstractValidator<GetFinancialImpactRequest>
    {
        public GetFinancialImpactValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                       //check acess in Framework 
                      var expFinancialImpact = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Positioning);
                       //check subitems 

                                if (expFinancialImpact != null &&
                    expFinancialImpact.SubItems.Any(a => a == (long)ModulesSuItemsEnum.FinancialImpact))
                    {
                        context.AddFailure(
                          new ValidationFailure("Posicionamento",
                          $"Você não tem permissão para acessar a tela de Impacto Financeiro"));
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
