using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Position.Validators;

namespace SM.Tests.Validators
{
    [TestClass]
    public class GetFullInfoPositionTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GetFullInfoPositionTest()
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
            var validator = new GetFullInfoPositionValidators(permissionUserInteractor);

            var result = validator.Validate(new GetFullInfoPositionValidatorsException
            {
                LevelId = 1,
                //Area = "Diretoria Suprimentos",
                UserId = userData.UserId,
                GroupId = 1,

            });

            Assert.AreEqual(result.IsValid, true);
        }

    }
}
