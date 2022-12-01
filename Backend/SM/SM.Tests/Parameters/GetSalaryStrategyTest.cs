using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Parameters.Queries;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Parameters
{
    [TestClass]
    public class GetSalaryStrategyTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly UserTestDTO userData;

        public GetSalaryStrategyTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId(); userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }


        [TestMethod]

        public async Task IsGetOk()
        {

            var getSalaryStrategyHandler =
                        new GetSalaryStrategyHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetSalaryStrategyRequest
            {
                UserId =  userData.UserId ,
                CompaniesId = userData.Companies,
                ProjectId = 1,
                IsAsc = true,
                SortColumnId = 1
            };

            var result = await getSalaryStrategyHandler.Handle(request, CancellationToken.None);
                Assert.AreNotSame(new GetSalaryStrategyRequest(), result);

        }
    }
}
