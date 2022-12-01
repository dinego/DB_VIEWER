using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.GetSalaryTable;
using SM.Application.Interactors;
using SM.Application.TableSalary.Queries.Response;
using SM.Application.TableSalary.Validators;
using SM.Domain.Enum;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetTableSalary
{
    [TestClass]
    public class GetSalaryTableExcelTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly MultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor;
        private readonly ValidatorResponse validatorResponse;
        private readonly MultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GetTableSalaryPermissionValidators validator;
        private readonly GenerateExcelFileInteractor generateExcelFileInteractor;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;
        private readonly GetLocalLabelsInteractor getLocalLabelsInteractor;

        public GetSalaryTableExcelTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            validatorResponse = new ValidatorResponse();
            multiplierFactorTypeContratcInteractor = new MultiplierFactorTypeContratcInteractor(unitOfWork, validatorResponse);
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            validator = new GetTableSalaryPermissionValidators(permissionUserInteractor);
            generateExcelFileInteractor = new GenerateExcelFileInteractor(new ColorScheme
            {
                Colors = new List<ColorData>
                           {
                               new ColorData
                               {
                                   Color = "#fafafa",
                                   Min = 50,
                                   Max = 90
                               }
                           }
            });
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
            getLocalLabelsInteractor = new GetLocalLabelsInteractor(unitOfWork);

        }

        [TestMethod]
        public async Task GetSalaryExcelTable()
        {
            var infoApp = new InfoApp { Name = "Teste" };
            var getSalaryTableExcelHandler = new
                GetSalaryTableExcelHandler(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                validator,
                permissionUserInteractor,
                validatorResponse,
                infoApp,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getSalaryTableExcelHandler.Handle(
                                              new GetSalaryTableExcelRequest
                                              {
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  GroupId = 1,
                                                  UserId = userData.UserId,
                                                  CompaniesId = userData.Companies,
                                                  UnitId = userData.CompanyId,
                                                  IsAsc = true,
                                                  SortColumnId = (int)TableSalaryColumnEnum.GSM
                                              }, CancellationToken.None);

            Assert.AreNotSame(new GetSalaryTableResponse(), result);
        }

    }
}