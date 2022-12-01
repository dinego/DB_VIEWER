using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.DashBoard.Queries;
using SM.Application.Interactors;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.DashBoard
{
    [TestClass]
    public class GetPositionsChartTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly UserTestDTO userData;

        public GetPositionsChartTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsGetOk()
        {
            var getPositionsChartHandler =
            new GetPositionsChartHandler(unitOfWork,
                permissionUserInteractor);

            var request = new GetPositionsChartRequest
            {
                UserId =  userData.UserId ,
                ProjectId = userData.ProjectId ,
                Unit = 11674,
                CompaniesId = userData.Companies
            };

            var result = await getPositionsChartHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetPositionsChartRequest(), result);

        }
    }
}
