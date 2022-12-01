using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.DashBoard.Queries;
using SM.Application.Interactors;
using SM.Domain.Enum.Positioning;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.DashBoard
{
    [TestClass]
    public class GetDistributionAnalysisChartTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly UserTestDTO userData;

        public GetDistributionAnalysisChartTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsGetOk()
        {
            var getDistributionAnalysisChartHandler =
            new GetDistributionAnalysisChartHandler(unitOfWork,
                permissionUserInteractor);

            var request = new GetDistributionAnalysisChartRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                Scenario = DisplayMMMIEnum.MI,
                CompaniesId = userData.Companies
            };

            var result = await getDistributionAnalysisChartHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetPositionsChartRequest(), result);

      }

    }
}
