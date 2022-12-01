using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.SetReadNotification;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetAllNotification
{
    [TestClass]
    public class SetReadNotificationTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public SetReadNotificationTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task SetReadNotification()
        {
            var setReadNotificationHandleTest = new SetReadNotificationHandler(unitOfWork);
            var result = await setReadNotificationHandleTest.Handle(new SetReadNotificationRequest { NotificationId = 6 }, CancellationToken.None);
            Assert.IsNotNull( result);
        }

        [TestMethod]
        public async Task SetReadNotificationZero()
        {
            var setReadNotificationHandleTest = new SetReadNotificationHandler(unitOfWork);
            var result = await setReadNotificationHandleTest.Handle(new SetReadNotificationRequest { NotificationId = 0 }, CancellationToken.None);
            Assert.Fail();
        }
    }
}