using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Positioning.Queries;
using SM.Domain.Enum.Positioning;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Positioning
{
    [TestClass]
    public class GetFinancialImpactTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly PermissionUserInteractor permissionUserInteractor;

        public GetFinancialImpactTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);

        }

        [TestMethod]
        
        public async Task TestProfile()
        {

            var getFinancialImpactHandler =
                        new GetFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor);

                var request = new GetFinancialImpactRequest
                {
                    UserId =  userData.UserId ,
                    ProjectId =  userData.ProjectId ,
                    CompaniesId = userData.Companies,
                    DisplayBy = DisplayByPositioningEnum.ProfileId
                };

                var result = await getFinancialImpactHandler.Handle(request, CancellationToken.None);

                Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task TestArea()
        {

            var getFinancialImpactHandler =
                        new GetFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetFinancialImpactRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.Area
            };

            var result = await getFinancialImpactHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task TestLevel()
        {

            var getFinancialImpactHandler =
                        new GetFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetFinancialImpactRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.LevelId
            };

            var result = await getFinancialImpactHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task TestParameter01()
        {

            var getFinancialImpactHandler =
                        new GetFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetFinancialImpactRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.Parameter01
            };

            var result = await getFinancialImpactHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task TestParameter02()
        {

            var getFinancialImpactHandler =
                        new GetFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetFinancialImpactRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.Parameter02
            };

            var result = await getFinancialImpactHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task TestParameter03()
        {

            var getFinancialImpactHandler =
                        new GetFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetFinancialImpactRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.Parameter03
            };

            var result = await getFinancialImpactHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task TestUnit11674()
        {

            var getFinancialImpactHandler =
                        new GetFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetFinancialImpactRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                UnitId =  userData.CompanyId ,
                CompaniesId = userData.Companies,
            };

            var result = await getFinancialImpactHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task TestScenario()
        {

            var getFinancialImpactHandler =
                        new GetFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetFinancialImpactRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                Scenario = DisplayMMMIEnum.MI,
                CompaniesId = userData.Companies
            };

            var result = await getFinancialImpactHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

        [TestMethod]
        public async Task TestAnalyseExp()
        {

            var getFinancialImpactHandler =
                        new GetFinancialImpactHandler(unitOfWork,
                        permissionUserInteractor);

            var request = new GetFinancialImpactRequest
            {
                UserId =  userData.UserId ,
                ProjectId =  userData.ProjectId ,
                CompaniesId = userData.Companies,
                DisplayBy = DisplayByPositioningEnum.Area
            };

            var result = await getFinancialImpactHandler.Handle(request, CancellationToken.None);

            Assert.AreNotSame(new GetFrameworkRequest(), result);

        }

    }
}
