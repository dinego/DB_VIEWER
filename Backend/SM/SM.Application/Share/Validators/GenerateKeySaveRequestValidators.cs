using FluentValidation;
using SM.Application.Share.Command;

namespace SM.Application.Share.Validators
{
    public class GenerateKeySaveRequestValidators
        : AbstractValidator<GenerateKeySaveRequest>
    {
        public GenerateKeySaveRequestValidators()
        {
            RuleFor(x => x.UserId).NotEmpty().NotNull().WithMessage("O Usuário Id é obrigatório")
                        .OverridePropertyName("Id do usuário");
            RuleFor(x => x.ModuleId).NotEmpty().NotNull().WithMessage("O Modulo Id é obrigatório")
                        .OverridePropertyName("Id do Módulo");
            RuleFor(x => x.Parameters).NotEmpty().NotNull().WithMessage("Os parâmetros/filtro são obrigatórios.")
                        .OverridePropertyName("Parâmetros");
        }
    }
}
