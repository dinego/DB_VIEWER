using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SM.Tests.InteractorsTest
{
    [TestClass]
    public class GetPositionProjectSMAndSalaryTableTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ValidatorResponse validatorResponse;
        private readonly MultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor;
        private readonly MultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor;
        private readonly PermissionUserInteractor permissionUserInteractor;

        public GetPositionProjectSMAndSalaryTableTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            validatorResponse = new ValidatorResponse();
            multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            multiplierFactorTypeContratcInteractor = new MultiplierFactorTypeContratcInteractor(unitOfWork, validatorResponse);
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);

        }
        [TestMethod]
        public async Task MM()
        {
            
            var getPositionProjectSMAndSalaryTableInteractor = new GetPositionProjectSMAndSalaryTableInteractor(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor);

            var mock = await UnitOfWorkTest.PositionProjectSMAndSalaryTableTest();

            var request = mock.Map().ToANew<List<DataPositionProjectSMRequest>>();

            var permissionUser = await permissionUserInteractor.Handler(userData.UserId); 

                var result = await getPositionProjectSMAndSalaryTableInteractor
                .Handler(request, userData.ProjectId, permissionUser, DataBaseSalaryEnum.MonthSalary, ContractTypeEnum.CLT) ;

            Assert.AreNotSame(new DataPositionProjectSMRequest(), result);
        }
     
    }
}
