using FluentValidation;
using FluentValidation.Results;
using SM.Application.Interactors.Interfaces;
using SM.Application.Reports.Command;
using SM.Application.Reports.Queries;
using SM.Domain.Enum;
using System.Linq;

namespace SM.Application.Reports.Validators
{
    public class ReportValidators : AbstractValidator<GetReportsRequest>
    {
        public ReportValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) => {

                            var permissionResult = permissionUserInteractor.Handler(request.UserId).Result;
                            var reportException = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Reports);
                            if(reportException != null)
                                context.AddFailure(new ValidationFailure("UserId", $"Você não tem acesso a tela de Meus Relatórios."));
                        });
        }
    }

    public class DownloadReportValidators : AbstractValidator<DownloadReportRequest>
    {
        public DownloadReportValidators(IPermissionUserInteractor permissionUserInteractor)
        {
            RuleFor(x => x)
                        .Custom((request, context) => {

                            var permissionResult = permissionUserInteractor.Handler(request.User).Result;
                            var reportException = permissionResult.Modules?.FirstOrDefault(s => s.Id == (long)ModulesEnum.Reports);
                            if (reportException != null)
                                context.AddFailure(new ValidationFailure("UserId", $"Você não tem acesso a tela de Meus Relatórios."));
                        });
        }
    }
}
