using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum.Positioning;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class GetComparativeAnalysisTableExcelTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GenerateExcelFileInteractor generateExcelFileInteractor;

        public GetComparativeAnalysisTableExcelTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
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
        }

        [TestMethod]

        public async Task TestProfile()
        {

            var getComparativeAnalysisTableExcelHandler =
                        new GetComparativeAnalysisTableExcelHandler(unitOfWork,
                        permissionUserInteractor,
                        generateExcelFileInteractor,
                        new InfoApp {Name = "SalaryMark"});

            var request = new GetComparativeAnalysisTableExcelRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.ProfileId
            };

            var result = await getComparativeAnalysisTableExcelHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }
    }
}
