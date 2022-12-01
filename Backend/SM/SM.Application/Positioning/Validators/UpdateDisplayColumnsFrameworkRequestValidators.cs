using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Positioning.Command;
using SM.Domain.Enum;
using SM.Domain.Enum.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SM.Application.Positioning.Validators
{
    public class UpdateDisplayColumnsFrameworkRequestValidators
        : AbstractValidator<UpdateDisplayColumnsFrameworkRequest>
    {
        public UpdateDisplayColumnsFrameworkRequestValidators(IPermissionUserInteractor permissionUserInteractor)
        {

            RuleFor(x => x.DisplayColumns).NotEmpty().NotNull().WithMessage("É necessário ter colunas")
               .OverridePropertyName("Colunas");

            RuleFor(x => x)
            .Custom((request, context) =>
            {

                var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            //check acess in Salary Table
                            var expListPosition = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Positioning);
                            //check subitems 

                            if (expListPosition != null &&
                expListPosition.SubItems.Any(a => a == (long)ModulesSuItemsEnum.Framework))
                {
                    context.AddFailure(
                      new ValidationFailure("Posicionamento",
                      $"Você não tem permissão para acessar a tela Enquadramento"));
                }

                var listEnumIds = new List<int>();


                var valuesMain = Enum.GetValues(typeof(FrameworkColumnsMainEnum));
                foreach (FrameworkColumnsMainEnum item in valuesMain)
                {
                    listEnumIds.Add((int)item);
                }

                var exp = request.DisplayColumns.Select(s=> s.ColumnId).Except(listEnumIds);

                if (exp.Any())
                    context.AddFailure(
                               new ValidationFailure("Exibição",
                               $"As colunasId estão incorretas {string.Join("-", exp)}"));
            });

        }

    }
}
