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
    public class GetFrameworkRequestValidators
         : AbstractValidator<GetFrameworkRequest>
    {
        public GetFrameworkRequestValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                .Custom((request, context) =>
                {
                    var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                    //check acess in Framework 
                    var expFramework = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Positioning);
                    //check subitems 

                    if (expFramework != null &&
        expFramework.SubItems.Any(a => a == (long)ModulesSuItemsEnum.Framework))
                    {
                        context.AddFailure(
                          new ValidationFailure("Posicionamento",
                          $"Você não tem permissão para acessar a tela de enquadramento"));
                    }

                    if (!request.IsMI && !request.IsMM)
                        context.AddFailure(
                            new ValidationFailure("MM/MI",
                            "Pelo menos o MM ou MI deve estar ativo"));

                    if (permissionResult.Display != null &&
                    permissionResult.Display.Scenario.Any())
                    {
                        if (request.IsMI && permissionResult.Display.Scenario.Any(sc => sc.Id == (long)DisplayMMMIEnum.MI))
                        {
                            context.AddFailure(
                            new ValidationFailure("Enquadramento",
                            $"Você não tem acesso a este a: {DisplayMMMIEnum.MI.GetDescription()}"));
                        }

                        if (request.IsMM && permissionResult.Display.Scenario.Any(sc => sc.Id == (long)DisplayMMMIEnum.MM))
                        {
                            context.AddFailure(
                            new ValidationFailure("Enquadramento",
                            $"Você não tem acesso a este a: {DisplayMMMIEnum.MM.GetDescription()}"));
                        }
                    }

                    //check hourtype 
                    if (permissionResult.DataBase != null && permissionResult.DataBase.Any())
                    {
                        if (permissionResult.DataBase.Contains((long)request.HoursType))
                            context.AddFailure(
                            new ValidationFailure("Tipo Hora",
                            $"Você não tem acesso a este tipo de hora: {request.HoursType.GetDescription()}"));
                    }

                    //check hourtype 
                    if (permissionResult.TypeOfContract != null &&
                    permissionResult.TypeOfContract.Any())
                    {
                        if (permissionResult.TypeOfContract.Contains((long)request.ContractType))
                            context.AddFailure(
                            new ValidationFailure("Tipo de Contrato",
                            $"Você não tem acesso a este tipo de contrato: {request.ContractType.GetDescription()}"));
                    }

                    // checkSortId
                    if (request.SortColumnId.HasValue)
                    {
                        var listEnums = Enum.GetValues(typeof(FrameworkColumnsMainEnum)) as
                                    IEnumerable<FrameworkColumnsMainEnum>;

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
