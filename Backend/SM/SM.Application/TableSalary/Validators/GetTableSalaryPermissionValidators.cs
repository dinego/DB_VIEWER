using CMC.Common.Extensions;
using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Application.TableSalary.Validators
{

    public class GetTableSalaryPermissionValidatorsException
    {
        public long UserId { get; set; }
        public long TableId { get; set; }
        public long GroupId { get; set; }
        public int? SortColumnId { get; set; }
        public ContractTypeEnum ContractType { get; set; }
        public DataBaseSalaryEnum HourType { get; set; }

    }

    public class GetTableSalaryPermissionValidators : AbstractValidator<GetTableSalaryPermissionValidatorsException>
    {

        public GetTableSalaryPermissionValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) =>
                        {

                            var permissionResult =  permissionUserInteractor.Handler(request.UserId).Result;

                            //check acess in Salary Table
                            var expSalaryTable = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.TableSalary);

                            if(expSalaryTable != null)
                            {
                                context.AddFailure(
                                  new ValidationFailure("Tabela Salarial",
                                  $"Você não tem a tela Tabela Salarial"));
                            }

                            //check tableSalaryId 
                            var salaryTableItems = permissionResult.Contents?
                            .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable);

                            if (salaryTableItems != null)
                            {
                                if(salaryTableItems.SubItems.Contains(request.TableId))
                                    context.AddFailure(
                                    new ValidationFailure("Tabela Salarial", 
                                    $"Você não tem acesso a esta tabela Salarial: Id{request.TableId}"));
                            }

                            //check tableSalaryId 
                            var groupItems = permissionResult.Contents?
                            .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group);

                            if (groupItems != null)
                            {
                                if (groupItems.SubItems.Contains(request.GroupId))
                                    context.AddFailure(
                                    new ValidationFailure("Perfil (grupo)",
                                    $"Você não tem acesso a este Perfil: Id{request.GroupId}"));
                            }

                            //check hourtype 
                            if (permissionResult.DataBase != null && permissionResult.DataBase.Any())
                            {
                                if (permissionResult.DataBase.Contains((long)request.HourType))
                                    context.AddFailure(
                                    new ValidationFailure("Tipo Hora",
                                    $"Você não tem acesso a este tipo de hora: {request.HourType.GetDescription()}"));
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
                                var listEnums = Enum.GetValues(typeof(TableSalaryColumnEnum)) as
                                            IEnumerable<TableSalaryColumnEnum>;

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
