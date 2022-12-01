using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Application.Position.Validators
{

    public class UpdateDisplayColumnsListPemissionException
    {
        public IEnumerable<int> ColumnIds { get; set; }
        public long UserId { get; set; }

    }

    public class UpdateDisplayColumnsListPemissionValidators : AbstractValidator<UpdateDisplayColumnsListPemissionException>
    {

        public UpdateDisplayColumnsListPemissionValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) =>
                        {

                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            //check acess in Salary Table
                            var expListPosition = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Position);
                            //check subitems 

                            if (expListPosition != null &&
                            expListPosition.SubItems.Any(a => a == (long)ModulesSuItemsEnum.Architecture))
                            {
                                context.AddFailure(
                                  new ValidationFailure("Cargos-Lista",
                                  $"Você não tem a tela Lista em cargos"));
                            }


                            var listListPositionColumns = Enum.GetValues(typeof(PositionProjectColumnsEnum)) as
                                            IEnumerable<PositionProjectColumnsEnum>;

                            var exp = request.ColumnIds.Except(listListPositionColumns.Select(s => (int)s));

                            if (exp.Any())
                                context.AddFailure(
                               new ValidationFailure("Exibição",
                               $"As colunasId estão incorretas {string.Join("-", exp)}"));
                        });
        }

    }
}
