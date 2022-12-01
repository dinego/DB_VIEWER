using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Movements.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Movements
{
    [TestClass]
    public class GetMovementsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly UserTestDTO userData;

        public GetMovementsTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsGetOk()
        {
            var getMovementsHandler =
            new GetMovementsHandler(permissionUserInteractor, unitOfWork);

            var user = UnitOfWorkTest.RetrieveUserId();
            if (user == null)
                return;

            var request = new GetMovementsRequest
            {
                UserId =  userData.UserId,
                Companies = userData.Companies
            };

            var result = await getMovementsHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetMovementsRequest(), result);

        }
    }
}
