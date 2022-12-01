using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Share.Command;
using SM.Application.Share.Validators;
using SM.Domain.Enum;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Share
{
    [TestClass]
    public class GenerateKeySaveTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GenerateKeySaveTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task IsSaveOk()
        {

            var permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            var validator = new SharePemissionValidators(permissionUserInteractor);
            var signingConfiguration = new SigningConfiguration
            {
                SecretKey = "CMC.0lwem3xbf366wyop05pj0zbpx.AUTH"
            };

            var validatorResponse = new ValidatorResponse();

            var generateKeySaveHandler =
                    new GenerateKeySaveHandler(unitOfWork,
                    signingConfiguration,
                    validator,
                    validatorResponse);


            var result = await generateKeySaveHandler.Handle(new GenerateKeySaveRequest
            {
                ModuleId = (long)ModulesEnum.TableSalary,
                UserId = userData.UserId,
                Parameters = new
                {
                    ContractType = ContractTypeEnum.CLT,
                    ProfileId = 1,
                    TableSalaryId = 1
                },
                ColumnsExcluded = new List<object> { 1, 2, 3 }
            }, CancellationToken.None);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(result));

        }

    }
}
