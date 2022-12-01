using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.GetSalaryTable;
using SM.Application.Interactors;
using SM.Application.TableSalary.Queries.Response;
using SM.Application.TableSalary.Validators;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetTableSalary
{
    [TestClass]
    public class GetSalaryTableTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private MultiplierFactorHourlyInteractor multiplierFactorHourlyInteractor;
        private MultiplierFactorTypeContratcInteractor multiplierFactorTypeContratcInteractor;
        private readonly PermissionUserInteractor permissionUserInteractor;
        private readonly GetTableSalaryPermissionValidators validator;
        private readonly GetGlobalLabelsInteractor getGlobalLabelsInteractor;
        private readonly GetLocalLabelsInteractor getLocalLabelsInteractor;
        private readonly ValidatorResponse validatorResponse;


        public GetSalaryTableTest()
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
        public async Task GetSalaryTable()
        {
            var getTableSalaryHandler = new GetSalaryTableHandler(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                validator,
                permissionUserInteractor,
                validatorResponse,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getTableSalaryHandler.Handle(
                                              new GetSalaryTableRequest
                                              {
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  GroupId = 4,
                                                  Page = 1,
                                                  PageSize = 20,
                                                  UserId = userData.UserId,
                                                  CompaniesId = userData.Companies,
                                                  IsAsc = false,
                                                  SortColumnId = (int)TableSalaryColumnEnum.GSM
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTableResponse(), result);
        }

        [TestMethod]
        public async Task GetSalaryTableSecondPage()
        {
            var validatorResponse = new ValidatorResponse();
            var getTableSalaryHandler = new GetSalaryTableHandler(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                validator,
                 permissionUserInteractor,
                validatorResponse,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getTableSalaryHandler.Handle(
                                              new GetSalaryTableRequest
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
            Assert.AreNotSame(new GetSalaryTableResponse(), result);
        }
        [TestMethod]
        public async Task GetSalaryTableEmptyPage()
        {
            var getTableSalaryHandler = new GetSalaryTableHandler(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                validator,
                permissionUserInteractor,
                validatorResponse,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getTableSalaryHandler.Handle(
                                              new GetSalaryTableRequest
                                              {
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  GroupId = 1,
                                                  Page = 4,
                                                  PageSize = 20,
                                                  UserId = userData.UserId,
                                                  CompaniesId = userData.Companies
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTableResponse(), result);
        }

        [TestMethod]
        public async Task GetSalaryTableEmptyUser()
        {
            var getTableSalaryHandler = new GetSalaryTableHandler(unitOfWork,
                multiplierFactorHourlyInteractor,
                multiplierFactorTypeContratcInteractor,
                validator,
                 permissionUserInteractor,
                validatorResponse,
                getGlobalLabelsInteractor,
                getLocalLabelsInteractor);

            var result = await getTableSalaryHandler.Handle(
                                              new GetSalaryTableRequest
                                              {
                                                  ProjectId = userData.ProjectId,
                                                  TableId = 1,
                                                  GroupId = 1,
                                                  Page = 4,
                                                  PageSize = 20,
                                                  UserId = userData.UserId,
                                                  CompaniesId = userData.Companies
                                              }, CancellationToken.None);
            Assert.AreNotSame(new GetSalaryTableResponse(), result);
        }
    }
}