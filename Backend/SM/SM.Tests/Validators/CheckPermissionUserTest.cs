using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.TableSalary.Validators;
using SM.Domain.Enum;

namespace SM.Tests.Validators
{
    [TestClass]
    public class CheckPermissionUserTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public CheckPermissionUserTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }
        [TestMethod]
        public void CheckValidateIsOk()
        {
            var permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            //todo fixed user to test 
            //Rita de Cassia de Souza Zaman - companyId : 10861
            var validator = new GetTableSalaryPermissionValidators(permissionUserInteractor);

            var result = validator.Validate(new GetTableSalaryPermissionValidatorsException
            {
                ContractType = ContractTypeEnum.CLT,
                HourType = DataBaseSalaryEnum.HourSalary,
                UserId =  userData.UserId ,
                GroupId = 1,
                TableId = 1
            });


            Assert.AreEqual(result.IsValid,true);
        }

    }
}
