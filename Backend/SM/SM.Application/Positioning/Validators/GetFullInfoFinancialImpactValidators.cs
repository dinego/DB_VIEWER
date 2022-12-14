using CMC.Common.Extensions;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Application.Positioning.Validators
{
    public class GetFullInfoFinancialImpactValidators
        : AbstractValidator<GetFullInfoFinancialImpactRequest>
    {
        public GetFullInfoFinancialImpactValidators(IPermissionUserInteractor permissionUserInteractor)
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


                            // if (permissionResult.Display != null &&
                            // permissionResult.Display.Any())
                            // {
                            //     if (permissionResult.Display.Contains((long)request.Scenario))
                            //     {
                            //         context.AddFailure(
                            //         new ValidationFailure("Posicionamento",
                            //         $"Você não tem acesso a este a: {request.Scenario.GetDescription()}"));
                            //     }
                            // }
                            // checkSortId
                            if (request.SortColumnId.HasValue)
                            {
                                var listEnums = Enum.GetValues(typeof(FullInfoPositioningEnum)) as
                                            IEnumerable<FullInfoPositioningEnum>;

                                var hasValueSort =
                                listEnums.Any(f => (int)f == request.SortColumnId.Value);

                                if (!hasValueSort)
                                    context.AddFailure(
                                   new ValidationFailure("Coluna para Ordenação",
                                   $"Coluna Id para ordenação errado"));
                            }

                        });
        }
    }
}
