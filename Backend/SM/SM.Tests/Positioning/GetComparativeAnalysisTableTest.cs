using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum.Positioning;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class GetComparativeAnalysisTableTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly IGetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetComparativeAnalysisTableTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
        }

        [TestMethod]

        public async Task TestProfile()
        {

            var getComparativeAnalysisTableHandler =
                        new GetComparativeAnalysisTableHandler(unitOfWork,
                        permissionUserInteractor,
                        getGlobalLabelsInteractor);

            var request = new GetComparativeAnalysisTableRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.ProfileId
            };

            var result = await getComparativeAnalysisTableHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }
    }
}
