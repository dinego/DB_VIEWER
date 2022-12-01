using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.DashBoard.Queries;
using SM.Application.Interactors;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.DashBoard
{
    [TestClass]
    public class GetProposedMovementsTypesTest
    {
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly UserTestDTO userData;

        public GetProposedMovementsTypesTest()
        {
            var unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsGetOk()
        {
            var getProposedMovementsTypesHandler =
            new GetProposedMovementsTypesHandler(permissionUserInteractor);

            var request = new GetProposedMovementsTypesRequest
            {
                UserId =  userData.UserId

            };

            var result = await getProposedMovementsTypesHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetProposedMovementsTypesRequest(), result);

        }

    }
}
