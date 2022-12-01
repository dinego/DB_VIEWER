using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Parameters.Command;
using SM.Domain.Enum;
using System.Linq;

namespace SM.Application.Parameters.Validators
{
    public class UpdateGlobalLabelsRequestValidators
        : AbstractValidator<UpdateGlobalLabelsRequest>
    {
        public UpdateGlobalLabelsRequestValidators(
            IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (!request.IsAdmin)
                    context.AddFailure(new ValidationFailure("IsAdmin", "Somente os administradores podem acessar os dados de Rótulos e Exibições."));

                var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                var modules = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Parameters);

                if (modules != null &&
                    modules.SubItems.Any(a => a == (long)ModulesSuItemsEnum.GlobalLabels))
                {
                    context.AddFailure(
                      new ValidationFailure("Parâmetros", $"Você não tem permissão para acessar a tela de Rótulos e Exibições"));
                }

                if (!request.GlobalLabels.Any(gl => gl.IsDefault))
                    context.AddFailure(
                      new ValidationFailure("Parâmetros", $"É necessário ter pelo menos um parâmetro definido como padrão."));

                if (request.GlobalLabels.Count(gl => gl.IsDefault) > 1)
                    context.AddFailure(
                      new ValidationFailure("Parâmetros", $"É permitido ter somente um parâmetro definido como padrão."));
            });

        }

    }
}
