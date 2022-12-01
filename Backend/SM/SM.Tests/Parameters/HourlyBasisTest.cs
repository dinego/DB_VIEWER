using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Domain.Entities;
using CMC.Common.Repositories;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Application.Parameters.Command;
using SM.Application.Parameters.Queries;
using SM.Application.Parameters.Validators;
using SM.Domain.Enum;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Parameters
{
    [TestClass]
    public class HourlyBasisTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ValidatorResponse validatorResponse;
        private readonly IPermissionUserInteractor _permissionUserInteractor;
        public HourlyBasisTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            _permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
            validatorResponse = new ValidatorResponse();
        }

        [TestMethod]
        public async Task ValidateIfHourlyBasisHasNotificationsIsFalse()
        {
            var getHourlyBasisHandler = new GetHourlyBasisHandler(unitOfWork, validatorResponse, _permissionUserInteractor);
            if (userData == null) return;
            _ = await getHourlyBasisHandler.Handle(new GetHourlyBasisRequest { IsAdmin = true, CompanyId = userData.ProjectCompanyId.GetValueOrDefault(0) }, CancellationToken.None);

            Assert.IsTrue(!validatorResponse.HasNotifications);
        }

        [TestMethod]
        public void ValidateIfHourlyBasisIsAdmin()
        {

            var validator = new GetHourlyBasisRequestValidator(unitOfWork);
            var result = validator.ShouldHaveValidationErrorFor(x => x.IsAdmin, new GetHourlyBasisRequest { IsAdmin = false, CompanyId = 0 });
            Assert.IsTrue(result.FirstOrDefault(xx => xx.ErrorMessage.Contains("Somente os administradores podem acessar os dados de Base Horária")) != null);
        }

        [TestMethod]
        public void ValidateIfHourlyBasisCompanyHasProject()
        {

            var validator = new GetHourlyBasisRequestValidator(unitOfWork);
            var result = validator.ShouldHaveValidationErrorFor(x => x.CompanyId, new GetHourlyBasisRequest { IsAdmin = true, CompanyId = 0 });
            Assert.IsTrue(result.FirstOrDefault(xx => xx.ErrorMessage.Contains("Não foi encontrado nenhum projeto associado a empresa")) != null);
        }

        [TestMethod]
        public async Task ValidateIfHourlyBasisHasData()
        {
            var getHourlyBasisHandler = new GetHourlyBasisHandler(unitOfWork, validatorResponse, _permissionUserInteractor);
            if (userData == null) return;

            var result = await getHourlyBasisHandler.Handle(new GetHourlyBasisRequest { IsAdmin = true, CompanyId = userData.ProjectCompanyId.GetValueOrDefault(0) }, CancellationToken.None);

            Assert.AreNotEqual(new List<GetHourlyBasisResponse>(), result);
        }

        [TestMethod]
        public void ValidateIfSaveHourlyBasisDataIsAdmin()
        {
            var validator = new SaveHourlyBasisValidators(unitOfWork);
            var result = validator.ShouldHaveValidationErrorFor(x => x.IsAdmin, new SaveHourlyBasisRequest { IsAdmin = false, CompanyId = 0 });
            Assert.IsTrue(result.FirstOrDefault(xx => xx.ErrorMessage.Contains("Somente os administradores podem acessar os dados de Base Horária")) != null);
        }

        [TestMethod]
        public void ValidateIfSaveHourlyBasisCompanyHasProject()
        {
            var validator = new SaveHourlyBasisValidators(unitOfWork);
            var result = validator.ShouldHaveValidationErrorFor(x => x.CompanyId, new SaveHourlyBasisRequest { IsAdmin = true, CompanyId = 0 });
            Assert.IsTrue(result.FirstOrDefault(xx => xx.ErrorMessage.Contains("Não foi encontrado nenhum projeto associado a empresa")) != null);
        }

        [TestMethod]
        public async Task SaveHourlyBasisIfDisplayIsFalseYearSalary()
        {
            var yearSalary = await unitOfWork.GetRepository<SalarioAnualParametro, long>().GetAsync(x => x.Where(xx => xx.Ativo.HasValue && xx.Ativo.Value));
            var saveHandler = new SaveHourlyBasisHandler(unitOfWork);
            var result = await saveHandler.Handle(new SaveHourlyBasisRequest
            {
                HourlyBasis = new List<SaveHourlyBasisItems> { new SaveHourlyBasisItems
                    {
                        Id = (long)DataBaseSalaryEnum.YearSalary,
                        Display = false,
                        SelectedValue = yearSalary != null ? yearSalary.Valor : 0
                    }
                },
                IsAdmin = true,
                CompanyId = userData.ProjectCompanyId.GetValueOrDefault(0),
            }, CancellationToken.None);

            Assert.IsFalse(validatorResponse.HasNotifications);
        }

        [TestMethod]
        public async Task SaveHourlyBasisIfDisplayIsTrueYearSalary()
        {
            var yearSalary = await unitOfWork.GetRepository<SalarioAnualParametro, long>().GetAsync(x => x.Where(xx => xx.Ativo.HasValue && xx.Ativo.Value));
            var saveHandler = new SaveHourlyBasisHandler(unitOfWork);
            var result = await saveHandler.Handle(new SaveHourlyBasisRequest
            {
                HourlyBasis = new List<SaveHourlyBasisItems> { new SaveHourlyBasisItems
                    {
                        Id = (long)DataBaseSalaryEnum.YearSalary,
                        Display = true,
                        SelectedValue = yearSalary != null ? yearSalary.Valor : 0
                    }
                },
                IsAdmin = true,
                CompanyId = userData.ProjectCompanyId.GetValueOrDefault(0)

            }, CancellationToken.None);

            Assert.IsFalse(validatorResponse.HasNotifications);
        }

        [TestMethod]
        public async Task SaveHourlyBasisIfDisplayIsFalseHourSalary()
        {
            var saveHandler = new SaveHourlyBasisHandler(unitOfWork);
            _ = await saveHandler.Handle(new SaveHourlyBasisRequest
            {
                HourlyBasis = new List<SaveHourlyBasisItems> { new SaveHourlyBasisItems
                    {
                        Id = (long)DataBaseSalaryEnum.HourSalary,
                        Display = false,
                        SelectedValue = 0
                    }
                },
                IsAdmin = true,
                CompanyId = userData.CompanyId,
            }, CancellationToken.None);

            Assert.IsFalse(validatorResponse.HasNotifications);
        }

        [TestMethod]
        public async Task SaveHourlyBasisIfDisplayIsTrueHourSalary()
        {
            var saveHandler = new SaveHourlyBasisHandler(unitOfWork);
            _ = await saveHandler.Handle(new SaveHourlyBasisRequest
            {
                HourlyBasis = new List<SaveHourlyBasisItems> { new SaveHourlyBasisItems
                    {
                        Id = (long)DataBaseSalaryEnum.HourSalary,
                        Display = true,
                        SelectedValue = 0
                    }
                },
                IsAdmin = true,
                CompanyId = userData.ProjectCompanyId.GetValueOrDefault(0),
            }, CancellationToken.None);

            Assert.IsFalse(validatorResponse.HasNotifications);
        }
    }
}
