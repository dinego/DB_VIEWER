using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.GetAllContractTypes;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetTableSalary
{
    [TestClass]
    public class GetAllContractTypesTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GetAllContractTypesTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task GetAllContractTypes()
        {
            var getAllContractTypesHandler = new GetAllContractTypesHandler(unitOfWork);
            var result = await getAllContractTypesHandler.Handle(
                                                        new GetAllContractTypesRequest
                                                        {
                                                            UserId = userData.UserId
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllContractTypesResponse(), result);
        }

        [TestMethod]
        public async Task GetAllContractTyesWrongUserId()
        {
            var getAllContractTypesHandler = new GetAllContractTypesHandler(unitOfWork);
            var result = await getAllContractTypesHandler.Handle(
                                                        new GetAllContractTypesRequest
                                                        {
                                                            UserId = userData.UserId
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllContractTypesResponse(), result);
        }

        [TestMethod]
        public async Task GetTableSalaryWhitoutUserBlocks()
        {
            var getAllContractTypesHandler = new GetAllContractTypesHandler(unitOfWork);
            var result = await getAllContractTypesHandler.Handle(
                                                        new GetAllContractTypesRequest
                                                        {
                                                            UserId = userData.UserId
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetAllContractTypesResponse(), result);
        }
    }
}