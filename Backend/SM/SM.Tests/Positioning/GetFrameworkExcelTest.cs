using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Queries;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class GetFrameworkExcelTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GetPositionProjectSMAndSalaryTableInteractor getPositionProjectSMAndSalaryTableInteractor;
        private readonly GenerateExcelFileInteractor generateExcelFileInteractor;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetFrameworkExcelTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            var validatorResponse = new ValidatorResponse();
            var multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            var multiplierFactorTypeContratcInteractor = new MultiplierFactorTypeContratcInteractor(unitOfWork, validatorResponse);
            getPositionProjectSMAndSalaryTableInteractor = new GetPositionProjectSMAndSalaryTableInteractor(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor);
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


        }

        [TestMethod]
        
        public async Task IsGetOk()
        {

                var getFrameworkExcelHandler =
                            new GetFrameworkExcelHandler(unitOfWork,
                            permissionUserInteractor,
                            getPositionProjectSMAndSalaryTableInteractor,
                            generateExcelFileInteractor,
                            new InfoApp
                            {
                               Name = "SalaryMark"
                            },
                            getGlobalLabelsInteractor
                            );

                var request = new GetFrameworkExcelRequest
                {
                    UserId =  userData.UserId ,
                    ProjectId =  userData.ProjectId ,
                    CompaniesId = userData.Companies,
                    IsMM = false,
                    IsMI = true,
                    IsAsc = true,
                    SortColumnId =1
                };

                var result = await getFrameworkExcelHandler.Handle(request, CancellationToken.None);

                Assert.AreNotSame(new GetFrameworkRequest(), result);

        }
 
    }
}
