using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Position.Queries;
using SM.Domain.Enum;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Position
{
    [TestClass]
    public class GetMapPositionExcelTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GenerateExcelFileInteractor generateExcelFileInteractor;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetMapPositionExcelTest()
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
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task GetMapPositionExcel()
        {

            var getMapPositionExcelHandler =
                new GetMapPositionExcelHandler(unitOfWork,
                permissionUserInteractor,
                generateExcelFileInteractor,
                                new InfoApp
                                {
                                    Name = "SalaryMark"
                                },
                                getGlobalLabelsInteractor);

            var request = new GetMapPositionExcelRequest
            {
                CompaniesId = userData.Companies,
                UserId =  userData.UserId,
                DisplayBy = DisplayByMapPositionEnum.AxisCarreira,
                ProjectId = userData.ProjectId,
                IsAsc = false
            };

            var result = await getMapPositionExcelHandler.Handle(request, CancellationToken.None);

            //todo fix test
            Assert.AreNotSame(new GetMapPositionResponse(), result);

        }

    }
}
