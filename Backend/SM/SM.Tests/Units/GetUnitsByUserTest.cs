using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Units.Queries;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Units
{
    [TestClass]
    public class GetUnitsByUserTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GetUnitsByUserTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsOkReturn()
        {

            var getUnitsByUserHandler =
                new GetUnitsByUserHandler(unitOfWork);

            var request = new GetUnitsByUserRequest
            {
                CompanyIds = new List<long>
                {
                    11674,
                    11675,
                    11679,
                    11696,
                    11681,
                    11682,
                    11687,
                    11688,
                    11689,
                    11691,
                    11692,
                    11683,
                    11693,
                    11685,
                    11694,
                    11695,
                    10861
                }
            };

            var result = await getUnitsByUserHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(17, result.Count());

        }

    }
}
