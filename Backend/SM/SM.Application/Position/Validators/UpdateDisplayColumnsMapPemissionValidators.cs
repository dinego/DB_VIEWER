using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System.Linq;

namespace SM.Application.Position.Validators
{

    public class UpdateDisplayColumnsMapPemissionException
    {
        public long UserId { get; set; }

    }

    public class UpdateDisplayColumnsMapPemissionValidators : AbstractValidator<UpdateDisplayColumnsMapPemissionException>
    {

        public UpdateDisplayColumnsMapPemissionValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) =>
                        {

                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            //check acess in Salary Table
                            var expMapPosition = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Position);
                            //check subitems 

                            if (expMapPosition != null && 
                            expMapPosition.SubItems.Any(a=> a == (long)ModulesSuItemsEnum.Map))
                            {
                                context.AddFailure(
                                  new ValidationFailure("Cargos-Mapa",
                                  $"Você não tem acesso a tela Mapa em cargos"));
                            }
                          
                        });
        }

    }
}
