using CMC.Common.Abstractions.Notification;
using CMC.Common.Repositories;
using MediatR;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SM.Application.Share.Command;
using SM.Domain.Options;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Tests.Share
{
    [TestClass]
    public class ShareLinkByEmailHandlerTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ISenderMail senderMail;

        public ShareLinkByEmailHandlerTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            senderMail = new SenderMail();
        }

        [TestMethod]
        public async Task SendEmail()
        {
            var smtp = new SmtpConfiguration
            {
                ContactUs = "demissmartin@hotmail.com",
                From = "carreira.noreply@carreira.com.br",
                SmtpHost = "smtplw.com.br",
                SmtpPassword = "cmul1836#",
                SmtpPort = 587,
                SmtpUser = "carreira"
            };
            var mediator = new Mock<IMediator>();
            var shareLink = new ShareLinkByEmailHandler(unitOfWork, mediator.Object, smtp);
            var result =await shareLink.Handle(new ShareLinkByEmailRequest { UserId =  userData.UserId , To = "demissmartin@hotmail.com", URL = "teste.com.br" }, CancellationToken.None);
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public async Task SendEmailEmptyURL()
        {
            var smtp = new SmtpConfiguration
            {
                ContactUs = "demissmartin@hotmail.com",
                From = "carreira.noreply@carreira.com.br",
                SmtpHost = "smtplw.com.br",
                SmtpPassword = "cmul1836#",
                SmtpPort = 587,
                SmtpUser = "carreira"
            };
            var mediator = new Mock<IMediator>();
            var shareLink = new ShareLinkByEmailHandler(unitOfWork, mediator.Object, smtp);
            var result = await shareLink.Handle(new ShareLinkByEmailRequest { UserId =  userData.UserId , To = "demissmartin@hotmail.com", URL = "" }, CancellationToken.None);
            Assert.IsNotNull(result);

        }
    }
}
