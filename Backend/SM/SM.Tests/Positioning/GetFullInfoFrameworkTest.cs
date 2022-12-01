using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class GetFullInfoFrameworkTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly ValidatorResponse validatorResponse;
        private readonly MultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor;
        private readonly MultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor;
        private readonly GetPositionProjectSMAndSalaryTableInteractor getPositionProjectSMAndSalaryTableInteractor;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetFullInfoFrameworkTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            validatorResponse = new ValidatorResponse();
            multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            multiplierFactorTypeContratcInteractor = new MultiplierFactorTypeContratcInteractor(unitOfWork, validatorResponse);
            getPositionProjectSMAndSalaryTableInteractor = new GetPositionProjectSMAndSalaryTableInteractor(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor);
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);

        }

        [TestMethod]
        
        public async Task IsGetOk()
        {

                var getFullInfoFrameworkHandler =
                            new GetFullInfoFrameworkHandler(unitOfWork,
                            permissionUserInteractor,
                            getPositionProjectSMAndSalaryTableInteractor,
                            getGlobalLabelsInteractor);

                var request = new GetFullInfoFrameworkRequest
                {
                    UserId =  userData.UserId ,
                    SalaryBaseId = 17208163,
                    CompaniesId = userData.Companies,
                    IsMI = true,
                    IsMM = true,
                    ProjectId = userData.ProjectId
                };

                var result = await getFullInfoFrameworkHandler.Handle(request, CancellationToken.None);

                Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

    }
}
