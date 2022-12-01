using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.TableSalary.Command;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;

namespace SM.Application.TableSalary.Validators
{

    public class UpdateSalaryTablePermissionException
    {
        public AuxSalaryTable SalaryTable { get; set; }
        public long UserId { get; set; }
    }

    public class UpdateSalarialTablePemissionValidators : AbstractValidator<UpdateSalaryTablePermissionException>
    {

        public UpdateSalarialTablePemissionValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                .Custom((request, context) => {
                    var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                    //check acess in Salary Table
                    var expSalaryTable = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.TableSalary);

                    if (expSalaryTable != null)
                    {
                        context.AddFailure(new ValidationFailure("Tabela Salarial", $"Você não tem a tela Tabela Salarial"));
                    }

                    if (request.SalaryTable == null)
                    {
                        context.AddFailure(
                        new ValidationFailure("Tabela Salarial",
                        $"A tabela salarial está vazia"));
                        return;
                    }

                    if (request.SalaryTable != null && request.SalaryTable.SalaryTableName.Length == 0)
                        context.AddFailure(
                        new ValidationFailure("Tabela Salarial",
                        $"A nome da tabela salarial está vazio"));

                    if (request.SalaryTable != null && request.SalaryTable.GsmFinal < request.SalaryTable.GsmInitial)
                        context.AddFailure(
                        new ValidationFailure("Tabela Salarial",
                        $"GSM Final é menor que o inicial"));

                    if (request.SalaryTable != null && request.SalaryTable.SalaryTableValues == null || request.SalaryTable.SalaryTableValues.Count() == 0)
                        context.AddFailure(
                        new ValidationFailure("Valores", 
                        $"Os valores da tabela salarial foram enviados"));
                });
        }

    }
}
