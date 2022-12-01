using CMC.Common.Extensions;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Position.Querie;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Application.Position.Validators
{
    public class GetAllPositionsRequestValidators
        : AbstractValidator<GetAllPositionsRequest>
    {
        public GetAllPositionsRequestValidators(IPermissionUserInteractor permissionUserInteractor)
        {

            RuleFor(x => x.TableId).NotEmpty().NotNull().WithMessage("A TabelaSalarial Id é obrigatória")
            .OverridePropertyName("TabelaSalarial Id");

            RuleFor(x => x)
            .Custom((request, context) =>
            {
                var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                // permission in screen
                var expListPosition = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Position);

                if (expListPosition != null &&
                expListPosition.SubItems.Any(a => a == (long)ModulesSuItemsEnum.Architecture))
                {
                    context.AddFailure(
                      new ValidationFailure("Cargos-Lista",
                      $"Você não tem acesso a tela Lista em Cargos"));
                }

                //check tableSalaryId 
                var salaryTableItems = permissionResult.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable);

                if (salaryTableItems != null)
                {
                    if (salaryTableItems.SubItems.Contains(request.TableId))
                        context.AddFailure(
                        new ValidationFailure("Tabela Salarial",
                        $"Você não tem acesso a esta tabela Salarial: Id{request.TableId}"));
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
                    var listEnums = Enum.GetValues(typeof(PositionProjectColumnsEnum)) as
                                IEnumerable<PositionProjectColumnsEnum>;

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
