using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Parameters.Queries;
using SM.Application.Parameters.Validators;

namespace SM.Tests.Validators
{
    [TestClass]
    public class GetLevelsConfigurationTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;

        public GetLevelsConfigurationTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }

        [TestMethod]
        public void CheckValidateIsOk()
        {
            
            var validator = 
                new GetLevelsConfigurationRequestValidators(unitOfWork,
                permissionUserInteractor);

            if (userData == null) return;
            var result = validator.Validate(new GetLevelsConfigurationRequest
            {
                UserId =  userData.UserId ,
                CompanyId = userData.CompanyId,
                IsAdmin = false
            });

            Assert.AreEqual(result.IsValid,false);
        }

    }
}
