using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using FluentValidation.TestHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.Interactors;
using SM.Application.Interactors.Interfaces;
using SM.Application.UserParameters.Command;
using SM.Application.UserParameters.Queries;
using SM.Application.UserParameters.Validators;
using SM.Domain.Enum.Positioning;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.UserParameters
{
    [TestClass]
    public class UserParametersTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ValidatorResponse validator;
        private readonly IPermissionUserInteractor permissionUserInteractor;
        public UserParametersTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
            validator = new ValidatorResponse();
            permissionUserInteractor = new PermissionUserInteractor(unitOfWork);
        }

        [TestMethod]
        public async Task GetAllUsers()
        {
            if (userData == null) return;
            var userHandler = new GetUsersHandler(unitOfWork);
            var result = await userHandler.Handle(new GetUsersRequest { UserCompanies = userData.Companies.ToList(), UserId = userData.UserId }, CancellationToken.None);
            Assert.AreNotEqual(new List<GetUsersResponse>(), result);
        }

        [TestMethod]
        public void ValidateGetUserInfoWhenAdminIsFalse()
        {
            var userValidator = new UserValidators();
            var result = userValidator.ShouldHaveValidationErrorFor(x => x.IsAdmin, new GetUserInfoPermissionsRequest { IsAdmin = false });
            Assert.IsTrue(result.FirstOrDefault(xx => xx.ErrorMessage.Contains("Somente os administradores podem acessar os dados de Usuários")) != null);
        }

        [TestMethod]
        public async Task ValidateGetUserInfo()
        {

            if (userData == null) return;
            var userInfoHandler = new GetUserInfoPermissionsHandler(unitOfWork, validator, permissionUserInteractor);
            var result = await userInfoHandler.Handle(new GetUserInfoPermissionsRequest
            {
                CompanyId = userData.CompanyId,
                IsAdmin = true,
                UserId = userData.UserId
            }, CancellationToken.None);

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void ValidateSaveUserInfoWhenAdminIsFalse()
        {
            var userValidator = new SaveUserInfoPermissionValidators();
            var result = userValidator.ShouldHaveValidationErrorFor(x => x.IsAdmin, new SaveUserInformationRequest { IsAdmin = false });
            Assert.IsTrue(result.FirstOrDefault(xx => xx.ErrorMessage.Contains("Somente os administradores podem acessar os dados de Usuários")) != null);
        }

        [TestMethod]
        public async Task ValidateSaveUserInfoWhenUserNotFound()
        {

            if (userData == null) return;
            var userInfoHandler = new SaveUserInformationHandler(unitOfWork, validator);
            var result = await userInfoHandler.Handle(new SaveUserInformationRequest
            {
                IsAdmin = false
            }, CancellationToken.None);

            Assert.IsTrue(validator.HasNotifications);
        }
        [TestMethod]
        public async Task ValidateSaveUserInfoAddPermission()
        {

            if (userData == null) return;
            var userInfoHandler = new SaveUserInformationHandler(unitOfWork, validator);


            var getUserInfoHandler = new GetUserInfoPermissionsHandler(unitOfWork, validator, permissionUserInteractor);
            var getUserResult = await getUserInfoHandler.Handle(new GetUserInfoPermissionsRequest
            {
                CompanyId = userData.CompanyId,
                IsAdmin = true,
                UserId = userData.UserId
            }, CancellationToken.None);

            Assert.IsTrue(getUserResult != null);

            var request = getUserResult.Map().ToANew<SaveUserInformationRequest>();
            request.Active = false;
            request.UserPermissions.Levels = new List<SubFieldCheckedUser> { new SubFieldCheckedUser { Id = 19, IsChecked = false, Name = "Test" } };
            Assert.IsNotNull(request);

            request.IsAdmin = true;
            _ = await userInfoHandler.Handle(request, CancellationToken.None);
            Assert.IsTrue(!validator.HasNotifications);
        }

        [TestMethod]
        public async Task ValidateSaveUserInfo()
        {

            if (userData == null) return;
            var userInfoHandler = new SaveUserInformationHandler(unitOfWork, validator);


            var getUserInfoHandler = new GetUserInfoPermissionsHandler(unitOfWork, validator, permissionUserInteractor);
            var getUserResult = await getUserInfoHandler.Handle(new GetUserInfoPermissionsRequest
            {
                CompanyId = userData.CompanyId,
                IsAdmin = true,
                UserId = userData.UserId
            }, CancellationToken.None);

            Assert.IsTrue(getUserResult != null);

            var request = getUserResult.Map().ToANew<SaveUserInformationRequest>();
            request.Active = true;
            request.UserPermissions.Levels = request.UserPermissions.Levels.Select(x => { x.IsChecked = true; return x; });
            Assert.IsNotNull(request);

            request.IsAdmin = true;
            _ = await userInfoHandler.Handle(request, CancellationToken.None);
            Assert.IsTrue(!validator.HasNotifications);
        }

        [TestMethod]
        public async Task ValidateSaveUserInfoWhenDisplayHasException()
        {

            if (userData == null) return;
            var userInfoHandler = new SaveUserInformationHandler(unitOfWork, validator);


            var getUserInfoHandler = new GetUserInfoPermissionsHandler(unitOfWork, validator,permissionUserInteractor);
            var getUserResult = await getUserInfoHandler.Handle(new GetUserInfoPermissionsRequest
            {
                CompanyId = userData.CompanyId,
                IsAdmin = true,
                UserId = userData.UserId
            }, CancellationToken.None);

            Assert.IsTrue(getUserResult != null);

            var request = getUserResult.Map().ToANew<SaveUserInformationRequest>();
            request.Active = false;
            request.UserPermissions.Contents.Where(x=> x.Id == (long)DisplayEnum.DisplayId)
                                                      .ForEach(x => { x.SubItems.ForEach(sb => { sb.IsChecked = false;});});
            Assert.IsNotNull(request);

            request.IsAdmin = true;
            _ = await userInfoHandler.Handle(request, CancellationToken.None);
            Assert.IsTrue(!validator.HasNotifications);
        }

        [TestMethod]
        public async Task ValidateSaveUserInfoWhenDisplayNotHasException()
        {

            if (userData == null) return;
            var userInfoHandler = new SaveUserInformationHandler(unitOfWork, validator);


            var getUserInfoHandler = new GetUserInfoPermissionsHandler(unitOfWork, validator, permissionUserInteractor);
            var getUserResult = await getUserInfoHandler.Handle(new GetUserInfoPermissionsRequest
            {
                CompanyId = userData.CompanyId,
                IsAdmin = true,
                UserId = userData.UserId
            }, CancellationToken.None);

            Assert.IsTrue(getUserResult != null);

            var request = getUserResult.Map().ToANew<SaveUserInformationRequest>();
            request.Active = true;
            request.UserPermissions.Contents.Where(x => x.Id == (long)DisplayEnum.DisplayId)
                                                      .ForEach(x => { x.IsChecked = true;  x.SubItems.ForEach(sb => { sb.IsChecked = true; }); });
            Assert.IsNotNull(request);

            request.IsAdmin = true;
            _ = await userInfoHandler.Handle(request, CancellationToken.None);
            Assert.IsTrue(!validator.HasNotifications);
        }
    }
}
