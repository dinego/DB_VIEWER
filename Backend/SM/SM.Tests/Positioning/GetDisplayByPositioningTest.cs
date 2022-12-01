using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class GetDisplayByPositioningTest
    {
        private UserTestDTO userData;
        private readonly IUnitOfWork unitOfWork;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetDisplayByPositioningTest()
        {
            userData = UnitOfWorkTest.RetrieveUserId();
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task IsGetOk()
        {
            var getDisplayByPositioningHandler =
            new GetDisplayByPositioningHandler(unitOfWork,
            getGlobalLabelsInteractor);

            var request = new GetDisplayByPositioningRequest
            {
                UserId =  userData.UserId ,
                ProjectId = userData.ProjectId
            };

            var result = await getDisplayByPositioningHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetDisplayByPositioningRequest(), result);

        }
    }
}
