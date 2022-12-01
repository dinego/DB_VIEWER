using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum.Positioning;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class GetDistributionAnalysisTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;

        public GetDistributionAnalysisTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task TestProfile()
        {

            var getProposedMovementsHandler =
                        new GetDistributionAnalysisHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetDistributionAnalysisRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.ProfileId
            };

            var result = await getProposedMovementsHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }
    }
}
