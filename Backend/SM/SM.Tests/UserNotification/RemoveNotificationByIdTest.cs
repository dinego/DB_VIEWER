using CMC.Common.Abstractions.Behaviours;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.RemoveNotificationById;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetAllNotification
{
    [TestClass]
    public class RemoveNotificationByIdTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ValidatorResponse validator;

        public RemoveNotificationByIdTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            validator = new ValidatorResponse();
        }

        [TestMethod]
        public async Task RemoveNotificationNotification()
        {
            var removeNotificationByIdHandler = new RemoveNotificationByIdHandler(unitOfWork, validator);
            var result = await removeNotificationByIdHandler.Handle(new RemoveNotificationByIdRequest { NotificationId = 6 }, CancellationToken.None);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task RemoveNotificationNotificationZero()
        {
            var removeNotificationByIdHandler = new RemoveNotificationByIdHandler(unitOfWork, validator);
            _ = await removeNotificationByIdHandler.Handle(new RemoveNotificationByIdRequest { NotificationId = 0 }, CancellationToken.None);
            Assert.IsTrue(validator.HasNotifications);
        }
    }
}