using FluentValidation;
using FluentValidation.Results;
using SM.Application.DashBoard.Queries;
using SM.Application.Interactors.Interfaces;
using SM.Domain.Enum;
using System.Linq;

namespace SM.Application.DashBoard.Validators
{
    public class GetPositionsChartRequestValidators
        : AbstractValidator<GetPositionsChartRequest>
    {
        public GetPositionsChartRequestValidators(
            IPermissionUserInteractor permissionUserInteractor)
        {

            RuleFor(x => x)
                        .Custom((request, context) =>
                        {
                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;

                            //check acess in Dashboard 
                            var expDashBoard = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.DashBoard);

                            if (expDashBoard != null)
                            {
                                context.AddFailure(
                                  new ValidationFailure("DashBoard",
                                  $"Você não tem permissão para acessar a tela de DashBoard"));
                            }
                        });
        }
    }
}
