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
    public class GetProposedMovementsValidators
        : AbstractValidator<GetProposedMovementsRequest>
    {

        public GetProposedMovementsValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                    //check acess in Framework 
                    var expFinancialImpact = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Positioning);
                    //check subitems 

                    if (expFinancialImpact != null &&
                        expFinancialImpact.SubItems.Any(a => a == (long)ModulesSuItemsEnum.ProposedMovements))
                    {
                        context.AddFailure(
                          new ValidationFailure("Posicionamento",
                          $"Você não tem permissão para acessar a tela de Movimentos Propostos"));
                    }


                    // if (permissionResult.Display != null &&
                    // permissionResult.Display.Any())
                    // {
                    //     if (permissionResult.Display.Contains((long)request.Scenario))
                    //     {
                    //         context.AddFailure(
                    //         new ValidationFailure("Enquadramento",
                    //         $"Você não tem acesso a este a: {request.Scenario.GetDescription()}"));
                    //     }
                    // }


                });
        }
    }
}
