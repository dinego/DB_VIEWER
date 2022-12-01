using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;
using SM.Application.Interactors;
using SM.Application.TableSalary.Queries;
using SM.Application.PositionDetails.Queries.Response;

namespace SM.Tests.GetTableSalary
{
    [TestClass]
    public class GetEditTableValuesTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;

        public GetEditTableValuesTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task GetEditTableValues()
        {
            var getTableSalaryHandler = new GetEditTableValuesHandler(unitOfWork, permissionUserInteractor);
            var result = await getTableSalaryHandler.Handle(
                                                        new GetEditTableValuesRequest
                                                        {
                                                            UserId = 288,
                                                            ProjectId = 5,
                                                            TableId = 1
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetEditTableValuesResponse(), result);
        }

        [TestMethod]
        public async Task GetTableSalarySalaryTableValuesNull()
        {
            var getTableSalaryHandler = new GetEditTableValuesHandler(unitOfWork, permissionUserInteractor);
            var result = await getTableSalaryHandler.Handle(
                                                        new GetEditTableValuesRequest
                                                        {
                                                            UserId = 288,
                                                            ProjectId = 5,
                                                            TableId = 11111111
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetEditTableValuesResponse(), result);
        }

        [TestMethod]
        public async Task GetTableSalaryWhitoutUserBlocks()
        {
            var getTableSalaryHandler = new GetEditTableValuesHandler(unitOfWork, permissionUserInteractor);
            var result = await getTableSalaryHandler.Handle(
                                                        new GetEditTableValuesRequest
                                                        {
                                                            UserId = 48,
                                                            ProjectId = 5,
                                                            TableId = 1
                                                        }, CancellationToken.None);
            Assert.AreNotSame(new GetEditTableValuesResponse(), result);
        }
    }
}