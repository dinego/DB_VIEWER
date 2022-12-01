using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System;
using System.Linq;

namespace SM.Application.Position.Validators
{

    public class GetFullInfoPositionValidatorsException
    {
        public long UserId { get; set; }
        public long Area { get; set; }
        public long LevelId { get; set; }
        public long GroupId { get; set; }
    }
    public class GetFullInfoPositionValidators : AbstractValidator<GetFullInfoPositionValidatorsException>
    {

        public GetFullInfoPositionValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) =>
                        {

                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            var expPosition = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Position);
                            //check subitems 

                            if (expPosition != null &&
                            expPosition.SubItems.Any(a => a == (long)ModulesSuItemsEnum.Map))
                            {
                                context.AddFailure(
                                  new ValidationFailure("Cargos-Mapa",
                                  $"Você não tem a tela Mapa em cargos"));
                            }

                            var groupItems = permissionResult.Contents?
                            .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group);

                            if (groupItems != null)
                            {
                                if (groupItems.SubItems.Contains(request.GroupId))
                                    context.AddFailure(
                                    new ValidationFailure("Perfil (grupo)",
                                    $"Você não tem acesso a este Perfil: Id{request.GroupId}"));
                            }

                            //check level 
                            if (permissionResult.Levels != null &&
                            permissionResult.Levels.Any())
                            {
                                if (permissionResult.Levels.Contains(request.LevelId))
                                    context.AddFailure(
                                    new ValidationFailure("Nível",
                                    $"Você não tem acesso a este nivel: {request.LevelId}"));
                            }

                            //check level 
                            if (permissionResult.Areas != null &&
                            permissionResult.Areas.Any())
                            {
                                if (permissionResult.Areas.Contains(request.Area))
                                    context.AddFailure(
                                    new ValidationFailure("Area",
                                    $"Você não tem acesso a esta area: {request.Area}"));
                            }
                        });
        }

    }
}
