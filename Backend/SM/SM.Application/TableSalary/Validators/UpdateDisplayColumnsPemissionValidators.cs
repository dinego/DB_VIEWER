using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;

namespace SM.Application.TableSalary.Validators
{
    public class UpdateDisplayColumnsPemissionException
    {
        public IEnumerable<int> ColumnIds { get; set; }
        public long UserId { get; set; }
        public bool CanEditGlobalLabels { get; set; }
    }

    public class UpdateDisplayColumnsPemissionValidators : AbstractValidator<UpdateDisplayColumnsPemissionException>
    {
        public UpdateDisplayColumnsPemissionValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) =>
                        {
                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;
                            var expSalaryTable = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.TableSalary);

                            if (expSalaryTable != null)
                                context.AddFailure(new ValidationFailure("Tabela Salarial", $"Você não tem acesso a tela Tabela Salarial"));


                            if (!request.CanEditGlobalLabels)
                                context.AddFailure(new ValidationFailure("Tabela Salarial", $"Você não pode alterar o rótulo."));

                            var listTableSalaryColumns = new List<int> {
                                (int)TableSalaryColumnEnum.GSM,
                                (int)TableSalaryColumnEnum.TableSalaryName,
                                (int)TableSalaryColumnEnum.Company,
                                (int) TableSalaryColumnPositionEnum.Profile,
                                (int) TableSalaryColumnPositionEnum.PositionSM
                                };
                            var exp = request.ColumnIds.Except(listTableSalaryColumns);
                            if (exp.Any())
                                context.AddFailure(new ValidationFailure("Exibição", $"O sistema não encontrou as colunas {string.Join("-", exp)}"));
                        });
        }
    }
}
