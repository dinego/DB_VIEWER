using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Position.Querie;
using SM.Application.TableSalary.Queries.Response;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetAllPositionsTest
{
    [TestClass]
    public class GetAllPositionsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly MultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor;
        private readonly ValidatorResponse validatorResponse;
        private readonly MultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;
        private readonly GetLocalLabelsInteractor getLocalLabelsInteractor;

        public GetAllPositionsTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            validatorResponse = new ValidatorResponse();
            multiplierFactorTypeContratcInteractor = new MultiplierFactorTypeContratcInteractor(unitOfWork, validatorResponse);
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
            getLocalLabelsInteractor = new GetLocalLabelsInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task GetAllPrositionsProjectsTableId()
        {

            var getAllPositionsHandler = new GetAllPositionsHandler(unitOfWork,
                permissionUserInteractor,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getAllPositionsHandler.Handle(
                                              new GetAllPositionsRequest
                                              {
                                                  Page = 1,
                                                  PageSize = 20,
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  UserId = userData.UserId,
                                                  CompaniesId = userData.Companies,
                                                  IsAsc = true,
                                                  SortColumnId = 1,
                                                  HoursType = Domain.Enum.DataBaseSalaryEnum.HourSalary,
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTableResponse(), result);
        }

        [TestMethod]
        public async Task GetAllPositionsProjectsShowJustWithOccupants()
        {

            var getAllPositionsHandler = new GetAllPositionsHandler(unitOfWork,
                permissionUserInteractor,
                                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getAllPositionsHandler.Handle(
                                              new GetAllPositionsRequest
                                              {
                                                  Page = 1,
                                                  PageSize = 20,
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  UserId = userData.UserId,
                                                  ShowJustWithOccupants = false,
                                                  CompaniesId = userData.Companies
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTableResponse(), result);
        }

        [TestMethod]
        public async Task GetAllPrositionsProjectsTerm()
        {
            var getAllPositionsHandler = new GetAllPositionsHandler(unitOfWork,
                permissionUserInteractor,
                                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getAllPositionsHandler.Handle(
                                              new GetAllPositionsRequest
                                              {
                                                  Page = 1,
                                                  PageSize = 20,
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  Term = "Assist",
                                                  UserId = userData.UserId,
                                                  CompaniesId = userData.Companies
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTableResponse(), result);
        }

        [TestMethod]
        public async Task GetAllPrositionsProjectsColumns()
        {
            var getAllPositionsHandler = new GetAllPositionsHandler(unitOfWork,
                permissionUserInteractor,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);


            var result = await getAllPositionsHandler.Handle(
                                              new GetAllPositionsRequest
                                              {
                                                  Page = 1,
                                                  PageSize = 20,
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  UserId = userData.UserId,
                                                  ColumnsExcluded = new List<int> { 1 },
                                                  CompaniesId = userData.Companies,
                                                  IsAsc = false,
                                                  SortColumnId = 1000
                                              }, CancellationToken.None);

            Assert.AreNotSame(new GetSalaryTableResponse(), result);
        }
    }
}