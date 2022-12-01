using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.DashBoard.Queries;
using SM.Application.Interactors;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.DashBoard
{
    [TestClass]
    public class GetFinancialImpactCardsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly UserTestDTO userData;

        public GetFinancialImpactCardsTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsGetOk()
        {
            var getFinancialImpactCardsHandler =
            new GetFinancialImpactCardsHandler(unitOfWork,
                permissionUserInteractor);

            var request = new GetFinancialImpactCardsRequest
            {
                UserId =  userData.UserId ,
                ProjectId = userData.ProjectId ,
                CompaniesId = userData.Companies
            };

            var result = await getFinancialImpactCardsHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetPositionsChartRequest(), result);

        }
    }
}
