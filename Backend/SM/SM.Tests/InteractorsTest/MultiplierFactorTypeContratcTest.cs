using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Domain.Enum;
using System.Threading.Tasks;

namespace SM.Tests.InteractorsTest
{
    [TestClass]
    public class MultiplierFactorTypeContratcTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ValidatorResponse validatorResponse;

        public MultiplierFactorTypeContratcTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            validatorResponse = new ValidatorResponse();
        }
        [TestMethod]
        public async Task CLT()
        {
            var multiplierFactorTypeContratcInteractor = new MultiplierFactorTypeContratcInteractor(unitOfWork, validatorResponse);
            var result = await multiplierFactorTypeContratcInteractor.Handler(userData.ProjectId, ContractTypeEnum.CLT);
            Assert.AreNotSame(typeof(long), result);
        }

        [TestMethod]
        public async Task PJ()
        {
            var multiplierFactorTypeContratcInteractor = new MultiplierFactorTypeContratcInteractor(unitOfWork, validatorResponse);
            var result = await multiplierFactorTypeContratcInteractor.Handler(userData.ProjectId, ContractTypeEnum.PJ);
            Assert.AreNotSame(typeof(long), result);
        }
    }
}
