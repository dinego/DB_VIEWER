using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.GetSalaryPositionTable;
using SM.Application.Interactors;
using SM.Application.TableSalary.Validators;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SM.Application.TableSalary.Queries.Response;

namespace SM.Tests.GetTableSalary
{
    [TestClass]
    public class GetSalaryPositionTableTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private MultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor;
        private MultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GetTableSalaryPermissionValidators validator;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;
        private readonly ValidatorResponse validatorResponse;
        private readonly GetLocalLabelsInteractor getLocalLabelsInteractor;


        public GetSalaryPositionTableTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            multiplierFactorHourlyInteractor = new MultiplierFactorHourlyInteractor(unitOfWork);
            validatorResponse = new ValidatorResponse();
            multiplierFactorTypeContratcInteractor = new MultiplierFactorTypeContratcInteractor(unitOfWork, validatorResponse);
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            validator = new GetTableSalaryPermissionValidators(permissionUserInteractor);
            getGlobalLabelsInteractor = new GetGlobalLabelsInteractor(unitOfWork);
            getLocalLabelsInteractor = new GetLocalLabelsInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task GetSalaryPositionTable()
        {

            var getTableSalaryHandler = new GetSalaryTablePositionHandler(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                validator,
                permissionUserInteractor,
                validatorResponse,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getTableSalaryHandler.Handle(
                                              new GetSalaryTablePositionRequest
                                              {
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  GroupId = 1,
                                                  Page = 1,
                                                  PageSize = 20,
                                                  UserId = userData.UserId,
                                                  CompaniesId = userData.Companies,
                                                  IsAsc = false
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTablePositionResponse(), result);
        }

        [TestMethod]
        public async Task GetSalaryTablePositionSecondPage()
        {
            var validatorResponse = new ValidatorResponse();
            var getTableSalaryHandler = new GetSalaryTablePositionHandler(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                validator,
                 permissionUserInteractor,
                validatorResponse,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getTableSalaryHandler.Handle(
                                              new GetSalaryTablePositionRequest
                                              {
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  GroupId = 1,
                                                  Page = 2,
                                                  CompaniesId = userData.Companies,
                                                  PageSize = 20,
                                                  UserId = userData.UserId,
                                                  ColumnsExcluded = new List<int> { 1 }
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTablePositionResponse(), result);
        }
        [TestMethod]
        public async Task GetSalaryTablePositionEmptyPage()
        {
            var getTableSalaryHandler = new GetSalaryTablePositionHandler(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                validator,
                permissionUserInteractor,
                validatorResponse,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getTableSalaryHandler.Handle(
                                              new GetSalaryTablePositionRequest
                                              {
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  GroupId = 1,
                                                  Page = 4,
                                                  PageSize = 20,
                                                  UserId = userData.UserId,
                                                  CompaniesId = userData.Companies
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTablePositionResponse(), result);
        }

        [TestMethod]
        public async Task GetSalaryTablePositionEmptyUser()
        {
            var getTableSalaryHandler = new GetSalaryTablePositionHandler(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                validator,
                 permissionUserInteractor,
                validatorResponse,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getTableSalaryHandler.Handle(
                                              new GetSalaryTablePositionRequest
                                              {
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  GroupId = 1,
                                                  Page = 4,
                                                  PageSize = 20,
                                                  UserId = userData.UserId,
                                                  CompaniesId = userData.Companies
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTablePositionResponse(), result);
        }
    }
}