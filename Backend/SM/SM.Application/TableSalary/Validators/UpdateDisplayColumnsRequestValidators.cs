using FluentValidation;
using SM.Application.TableSalary.Command;

namespace SM.Application.TableSalary.Validators
{
    public class UpdateDisplayColumnsRequestValidators
         : AbstractValidator<UpdateDisplayColumnsRequest>
    {
        public UpdateDisplayColumnsRequestValidators()
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull().WithMessage("O Usuário Id é obrigatório")
                       .OverridePropertyName("Id do usuário");
            RuleFor(x => x.DisplayColumns).NotEmpty().NotNull().WithMessage("É necessário ter colunas")
                        .OverridePropertyName("Colunas");

        }
    }
}
