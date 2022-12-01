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
    public class GetFrameworkTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly ValidatorResponse validatorResponse;
        private readonly MultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor;
        private readonly MultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor;
        private readonly GetPositionProjectSMAndSalaryTableInteractor getPositionProjectSMAndSalaryTableInteractor;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;

        public GetFrameworkTest()
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

                var getFrameworkHandler =
                            new GetFrameworkHandler(unitOfWork,
                            permissionUserInteractor,
                            getPositionProjectSMAndSalaryTableInteractor,
                            getGlobalLabelsInteractor);

                var request = new GetFrameworkRequest
                {
                    UserId =  userData.UserId ,
                    ProjectId =  userData.ProjectId ,
                    CompaniesId = userData.Companies,
                    IsMM = false,
                    IsMI = true,
                    IsAsc = true,
                    SortColumnId = 1
                };

                var result = await getFrameworkHandler.Handle(request, CancellationToken.None);

                Assert.AreNotSame(new GetFrameworkRequest(), result);

        }


        [TestMethod]
        public async Task IsGetOkMMMI()
        {

            var getFrameworkHandler =
                        new GetFrameworkHandler(unitOfWork,
                        permissionUserInteractor,
                        getPositionProjectSMAndSalaryTableInteractor,
                        getGlobalLabelsInteractor);

            var request = new GetFrameworkRequest
            {
                UserId =  userData.UserId ,
                CompaniesId = userData.Companies,
                IsMI = true,
                IsMM = false
            };

            var result = await getFrameworkHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task IsGetOkUnit()
        {

            var getFrameworkHandler =
                        new GetFrameworkHandler(unitOfWork,
                        permissionUserInteractor,
                        getPositionProjectSMAndSalaryTableInteractor,
                        getGlobalLabelsInteractor);

            var request = new GetFrameworkRequest
            {
                UserId =  userData.UserId ,
                CompaniesId = userData.Companies,
                UnitId =  userData.CompanyId ,
                IsMI = true,
                IsMM = false
            };

            var result = await getFrameworkHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task IsGetOkTerm()
        {

            var getFrameworkHandler =
                        new GetFrameworkHandler(unitOfWork,
                        permissionUserInteractor,
                        getPositionProjectSMAndSalaryTableInteractor,
                        getGlobalLabelsInteractor);

            var request = new GetFrameworkRequest
            {
                UserId =  userData.UserId ,
                CompaniesId = userData.Companies,
                Term = "AJUDANTE",
                IsMI = true,
                IsMM = false
            };

            var result = await getFrameworkHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task IsGetOkContractType()
        {

            var getFrameworkHandler =
                        new GetFrameworkHandler(unitOfWork,
                        permissionUserInteractor,
                        getPositionProjectSMAndSalaryTableInteractor,
                        getGlobalLabelsInteractor);

            var request = new GetFrameworkRequest
            {
                UserId =  userData.UserId ,
                CompaniesId = userData.Companies,
                ContractType = ContractTypeEnum.PJ,
                IsMI = true,
                IsMM = false
            };

            var result = await getFrameworkHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task IsGetOkHoursType()
        {

            var getFrameworkHandler =
                        new GetFrameworkHandler(unitOfWork,
                        permissionUserInteractor,
                        getPositionProjectSMAndSalaryTableInteractor,
                        getGlobalLabelsInteractor);

            var request = new GetFrameworkRequest
            {
                UserId =  userData.UserId ,
                CompaniesId = userData.Companies,
                HoursType = DataBaseSalaryEnum.MonthSalary,
                IsMI = true,
                IsMM = false
            };

            var result = await getFrameworkHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task IsGetOkColumns()
        {

            var getFrameworkHandler =
                        new GetFrameworkHandler(unitOfWork,
                        permissionUserInteractor,
                        getPositionProjectSMAndSalaryTableInteractor,
                        getGlobalLabelsInteractor);

            var request = new GetFrameworkRequest
            {
                UserId =  userData.UserId ,
                CompaniesId = userData.Companies,
                Columns = new List<int> { 1 }
            };

            var result = await getFrameworkHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }
    }
}
