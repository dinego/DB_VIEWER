using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.RemoveProfileImage;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetAllNotification
{
    [TestClass]
    public class RemoveProfileImageTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ValidatorResponse validator;
        private readonly UserTestDTO userData;

        public RemoveProfileImageTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); userData = UnitOfWorkTest.RetrieveUserId();
            validator = new ValidatorResponse();
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task ProfileImage()
        {
            var removeProfileImageHandler = new RemoveProfileImageHandler(unitOfWork, validator);


            var result = await removeProfileImageHandler.Handle(new RemoveProfileImageRequest { UserId = userData.UserId }, CancellationToken.None);
            Assert.AreNotSame(string.Empty, result);
        }

        [TestMethod]
        public async Task ProfileImageExceptionUser()
        {
            var removeProfileImageHandler = new RemoveProfileImageHandler(unitOfWork, validator);
            _ = await removeProfileImageHandler.Handle(new RemoveProfileImageRequest { UserId = userData.UserId }, CancellationToken.None);
            Assert.IsFalse(validator.HasNotifications);
        }
    }
}