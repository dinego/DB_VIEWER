using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Parameters.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Parameters
{
    [TestClass]
    public class GetPJSettingsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GetPJSettingsTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsOK()
        {

            var getPJSettingsHandler =
                        new GetPJSettingsHandler(unitOfWork);

            var request = new GetPJSettingsRequest
            {
                ProjectId =  userData.ProjectId ,
                IsAdmin = true,
                ContractTypeId = 1,
                UserId = 520
            };

            var result = await getPJSettingsHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetPJSettingsRequest(), result);

        }
    }
}
