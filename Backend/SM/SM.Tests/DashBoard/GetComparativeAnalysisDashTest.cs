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
    public class GetComparativeAnalysisDashTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly UserTestDTO userData;

        public GetComparativeAnalysisDashTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsGetOk()
        {
            var getComparativeAnalysisDashHandler =
            new GetComparativeAnalysisDashHandler(unitOfWork,
                permissionUserInteractor);

            var request = new GetComparativeAnalysisDashRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId,
                Scenario = DisplayMMMIEnum.MI,
                CompaniesId = userData.Companies,
            };

            var result = await getComparativeAnalysisDashHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetPositionsChartRequest(), result);

        }
    }
}
