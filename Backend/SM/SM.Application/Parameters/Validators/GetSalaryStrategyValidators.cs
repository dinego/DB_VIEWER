using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Parameters.Queries;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Application.Parameters.Validators
{
    public class GetSalaryStrategyValidators
        : AbstractValidator<GetSalaryStrategyRequest>
    {

        public GetSalaryStrategyValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) =>
                        {

                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            //check acess in Levels - Parameter 
                            var expStrategyParameter = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Parameters);
                            //check subitems 

                            if (expStrategyParameter != null &&
                                    expStrategyParameter.SubItems.Any(a => a == (long)ModulesSuItemsEnum.SalaryStrategy))
                            {
                                context.AddFailure(
                                  new ValidationFailure("Parâmetros",
                                  $"Você não tem acesso a tela Níveis- Parâmetros"));
                            }

                            if (!request.IsAdmin)
                                context.AddFailure(new ValidationFailure("IsAdmin", "Somente os administradores podem acessar os dados de Estratégia Salarial."));

                            // checkSortId
                            if (request.SortColumnId.HasValue)
                            {
                                var listEnums = Enum.GetValues(typeof(SalaryStrategyColumnEnum)) as
                                            IEnumerable<SalaryStrategyColumnEnum>;

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
