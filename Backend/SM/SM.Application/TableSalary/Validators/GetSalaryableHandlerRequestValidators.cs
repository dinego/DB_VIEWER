using FluentValidation;
using SM.Application.GetSalaryTable;

namespace SM.Application.TableSalary.Validators
{
    public class GetSalaryableHandlerRequestValidators
         : AbstractValidator<GetSalaryTableRequest>
    {
        public GetSalaryableHandlerRequestValidators()
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull().WithMessage("O Usuário Id é obrigatório")
                       .OverridePropertyName("Id do usuário");
            RuleFor(x => x.TableId).NotEmpty().NotNull().WithMessage("É necessário ter id da Tabela")
                        .OverridePropertyName("Tabela Id");
        }
    }
}
