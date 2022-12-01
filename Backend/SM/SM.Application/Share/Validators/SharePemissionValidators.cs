using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System.Linq;

namespace SM.Application.Share.Validators
{

    public class SharePemissionException
    {
        public long UserId { get; set; }

    }

    public class SharePemissionValidators : AbstractValidator<SharePemissionException>
    {

        public SharePemissionValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) =>
                        {
                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            //check acesss in Salary Table
                            var expSalaryTable = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.TableSalary);

                            if (expSalaryTable != null)
                            {
                                context.AddFailure(
                                  new ValidationFailure("Tabela Salarial",
                                  $"Você não tem acesso a tela Tabela Salarial"));
                            }

                            //check if user can shared 
                            if (permissionResult.Permission != null && permissionResult.Permission
                            .Any(s => s.Id == (long)PermissionItensEnum.Share && !s.IsChecked))
                                context.AddFailure(
                                   new ValidationFailure("Compartilhar",
                                   $"Você não tem permissão para compartilhar"));
                        });
        }

    }
}
