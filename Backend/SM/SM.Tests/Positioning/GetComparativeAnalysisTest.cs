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
    public class GetComparativeAnalysisTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;
        public GetComparativeAnalysisTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }

        [TestMethod]

        public async Task TestProfile()
        {

            var getComparativeAnalysisChartHandler =
                        new GetComparativeAnalysisChartHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetComparativeAnalysisChartRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.ProfileId
            };

            var result = await getComparativeAnalysisChartHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }
    }
}
