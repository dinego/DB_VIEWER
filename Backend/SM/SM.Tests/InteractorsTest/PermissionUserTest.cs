using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using System.Threading.Tasks;

namespace SM.Tests.InteractorsTest
{
    [TestClass]
    public class PermissionUserTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public PermissionUserTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
        }
        [TestMethod]
        public async Task CheckUserPermission()
        {
            var permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            //todo fixed user to test 
            //Rita de Cassia de Souza Zaman - companyId : 10861
            long userId = 520; 
            var result = await permissionUserInteractor.Handler(userId);

            Assert.IsNotNull(result);
        }

    }
}
