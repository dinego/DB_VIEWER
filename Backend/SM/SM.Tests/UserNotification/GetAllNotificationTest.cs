using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Application.GetAllNotification;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.GetAllNotification
{
    [TestClass]
    public class GetAllNotificationTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public GetAllNotificationTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
        }

        [TestMethod]
        public async Task GetNotifications()
        {
            var getAllNotificationHandleTest = new GetAllNotificationHandler(unitOfWork);
            var result = await getAllNotificationHandleTest.Handle(new GetAllNotificationRequest { UserId =  userData.UserId , Page = 1, PageSize = 20 }, CancellationToken.None);
            Assert.AreNotSame(new GetAllNotificationResponse(), result);
        }

        [TestMethod]
        public async Task GetNotificationsOtherPage()
        {
            var getAllNotificationHandleTest = new GetAllNotificationHandler(unitOfWork);
            var result = await getAllNotificationHandleTest.Handle(new GetAllNotificationRequest { UserId =  userData.UserId , Page = 2, PageSize = 20 }, CancellationToken.None);
            Assert.AreNotSame(new GetAllNotificationResponse(), result);
        }

        [TestMethod]
        public async Task GetNotificationsShortPageSize()
        {
            var getAllNotificationHandleTest = new GetAllNotificationHandler(unitOfWork);
            var result = await getAllNotificationHandleTest.Handle(new GetAllNotificationRequest { UserId =  userData.UserId , Page = 1, PageSize = 2 }, CancellationToken.None);
            Assert.AreNotSame(new GetAllNotificationResponse(), result);
        }
    }
}