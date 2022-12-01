using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Position.Queries;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Position
{
    [TestClass]
    public class GetAllDisplayByTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetAllDisplayByTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
        }

        public IUnitOfWork UnitOfWork => unitOfWork;

        public UserTestDTO UserData => userData;

        [TestMethod]
        public async Task IsOkReturn()
        {

            var getAllDisplayByHandler =
                new GetAllDisplayByHandler(unitOfWork, getGlobalLabelsInteractor);

            var request = new GetAllDisplayByRequest
            {
                ProjectId = userData.ProjectId
            };

            var result = await getAllDisplayByHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetAllDisplayByRequest() , result.Count());

        }

    }
}
