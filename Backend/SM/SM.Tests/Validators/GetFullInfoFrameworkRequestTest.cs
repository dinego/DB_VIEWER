using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Queries;
using SM.Application.Positioning.Validators;

namespace SM.Tests.Validators
{
    [TestClass]
    public class GetFullInfoFrameworkRequestTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;

        public GetFullInfoFrameworkRequestTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }
        [TestMethod]
        public void CheckValidateIsOk()
        {
            var validator = new GetFullInfoFrameworkRequestValidators(permissionUserInteractor);

            var result = validator.Validate(new GetFullInfoFrameworkRequest
            {
                UserId =  userData.UserId,
                SalaryBaseId = 16658216,
                CompaniesId = userData.Companies,
                IsMI = true,
                IsMM = true
            });

            Assert.AreEqual(result.IsValid,true);
        }

    }
}
