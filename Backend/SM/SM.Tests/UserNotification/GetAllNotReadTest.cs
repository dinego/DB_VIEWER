using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.GetAllNotification;
using SM.Application.GetAllNotRead;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetAllNotification
{
    [TestClass]
    public class GetAllNotReadTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GetAllNotReadTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task GetNotificationsNotRead()
        {
            var getAllNotificationHandleTest = new GetAllNotReadHandler(unitOfWork);
            var result = await getAllNotificationHandleTest.Handle(new GetAllNotReadRequest { UserId = userData.UserId }, CancellationToken.None);
            Assert.AreNotSame(new GetAllNotificationResponse(), result);
        }

        [TestMethod]
        public async Task GetNotificationsNotReadZero()
        {
            var getAllNotificationHandleTest = new GetAllNotReadHandler(unitOfWork);
            var result = await getAllNotificationHandleTest.Handle(new GetAllNotReadRequest { UserId = userData.UserId }, CancellationToken.None);
            Assert.AreNotSame(new GetAllNotificationResponse(), result);
        }
    }
}