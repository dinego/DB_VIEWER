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
    public class GetFullInfoComparativeAnalysisValidators
        : AbstractValidator<GetFullInfoComparativeAnalysisRequest>
    {
        public GetFullInfoComparativeAnalysisValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) =>
                        {
                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            var exp = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Positioning);
                            //check subitems 

                            if (exp != null &&
                                             exp.SubItems.Any(a => a == (long)ModulesSuItemsEnum.ComparativeAnalysis))
                            {
                                context.AddFailure(
                                  new ValidationFailure("Posicionamento",
                                  $"Você não tem permissão para acessar a tela de Análise Comparativa"));
                            }


                            if (permissionResult.Display != null &&
                            permissionResult.Display.Scenario.Any())
                            {
                                if (permissionResult.Display.Scenario.Any(sc => sc.Id == (long)request.Scenario))
                                {
                                    context.AddFailure(
                                    new ValidationFailure("Posicionamento",
                                    $"Você não tem acesso a este a: {request.Scenario.GetDescription()}"));
                                }
                            }
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
