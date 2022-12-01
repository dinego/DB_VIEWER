using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Domain.Enum;
using System.Threading.Tasks;

namespace SM.Tests.InteractorsTest
{
    [TestClass]
    public class MultiplierFactorHourlyTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public MultiplierFactorHourlyTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }
        [TestMethod]
        public async Task MonthSalary()
        {
            var multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            var result = await multiplierFactorHourlyInteractor.Handler(1, DataBaseSalaryEnum.MonthSalary);
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public async Task HourSalary()
        {
            var multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            var result = await multiplierFactorHourlyInteractor.Handler(1, DataBaseSalaryEnum.HourSalary);
            Assert.AreEqual(result, (double)220/100);

        }

        [TestMethod]
        public async Task YearSalary()
        {
            var multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            var result = await multiplierFactorHourlyInteractor.Handler(1, DataBaseSalaryEnum.YearSalary);
            Assert.AreEqual(result, 13,33);
        }
    }
}
