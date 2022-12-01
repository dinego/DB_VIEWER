using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Position.Queries;
using SM.Domain.Enum;
using System;
using System.Linq;

namespace SM.Application.Position.Validators
{
    public class GetMapPositionRequestValidators
        : AbstractValidator<GetMapPositionRequest>
    {
        public GetMapPositionRequestValidators(IPermissionUserInteractor permissionUserInteractor)
        {

            RuleFor(x => x)
            .Custom((request, context) =>
            {
                var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                //check acess in Salary Table
                var expSalaryTable = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Position);
                //check subitems 

                if (expSalaryTable != null &&
                expSalaryTable.SubItems.Any(a => a == (long)ModulesSuItemsEnum.Map))
                {
                    context.AddFailure(
                      new ValidationFailure("Cargos-Mapa",
                      $"Você não tem acesso a tela Mapa em cargos"));
                }

                //check tableSalaryId 
                var salaryTableItems = permissionResult.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.SalaryTable);

                if (salaryTableItems != null && request.TableId.HasValue)
                {
                    if (salaryTableItems.SubItems.Contains(request.TableId.Value))
                        context.AddFailure(
                        new ValidationFailure("Tabela Salarial",
                        $"Você não tem acesso a esta tabela Salarial: Id{request.TableId}"));
                }

                //check tableSalaryId 
                var groupItems = permissionResult.Contents?
                .FirstOrDefault(s => s.Id == (long)ContentsSubItemsEnum.Group);

                if (groupItems != null && request.GroupId.HasValue)
                {
                    if (groupItems.SubItems.Contains(request.GroupId.Value))
                        context.AddFailure(
                        new ValidationFailure("Perfil (grupo)",
                        $"Você não tem acesso a este Perfil: Id{request.GroupId}"));
                }

            });

        }
    }
}
