using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Position.Queries;
using SM.Application.Position.Validators;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Position
{
    [TestClass]
    public class GetFullInfoPositionTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GetFullInfoPositionValidators validator;
        private readonly ValidatorResponse validatorResponse;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetFullInfoPositionTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            validator = new GetFullInfoPositionValidators(permissionUserInteractor);
            validatorResponse = new ValidatorResponse();
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);

        }

        [TestMethod]
        public async Task IsOkReturn()
        {

            var getFullInfoPositionHandler =
                new GetFullInfoPositionHandler(unitOfWork,
                validator,
                validatorResponse, 
                permissionUserInteractor,
                getGlobalLabelsInteractor);

            var request = new GetFullInfoPositionRequest
            {
                CompaniesId = new List<long>
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
                },
                UserId =  userData.UserId ,
                PositionSmIdLocal = 320,
                ProjectId = 1
            };

                var result = await getFullInfoPositionHandler.Handle(request, CancellationToken.None);
                Assert.AreNotSame(new GetFullInfoPositionResponse(), result);
        }

    }
}
