using CMC.Common.Abstractions.Notification;
using CMC.Common.Repositories;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SM.Application.ContactUs;
using SM.Domain.Options;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static SM.Application.MailNotification.MailNotification;

namespace SM.Tests.GetAllNotification
{
    [TestClass]
    public class ContactUsTest
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;
        private readonly ISenderMail senderMail;
        public ContactUsTest()
        {
            unitOfWork = UnitOfWorkTest.GetUnitOfWork(); 
            userData = UnitOfWorkTest.RetrieveUserId();
            senderMail = new SenderMail();
        }

        [TestMethod]
        public async Task ContactUs()
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
            var contactUsHandler = new ContactUsHandler(unitOfWork, mediator.Object, smtp);
            var result = await contactUsHandler.Handle(new ContactUsRequest
            {
                Attachment = MakeAttachment(),
                Subject = "Assunto",
                Message = "Mensage",
                UserId = userData.UserId
            }, CancellationToken.None);
            Assert.AreNotSame(string.Empty, result);
        }

        [TestMethod]
        public async Task SendMailContactUs()
        {
            var smtp = new SmtpConfiguration
            {
                ContactUs = "demissmartin@hotmail.com.br",
                From = "carreira.noreply@carreira.com.br",
                SmtpHost = "smtplw.com.br",
                SmtpPassword = "cmul1836#",
                SmtpPort = 587,
                SmtpUser = "carreira"
            };

            var mailNotification = new MailNotificationHandler(senderMail, smtp);
            var mediator = new Mock<IMediator>();
            var contactUsHandler = new ContactUsHandler(unitOfWork, mediator.Object, smtp);
            var resultMesage = await contactUsHandler.CreateMessage(new ContactUsRequest { Attachment = MakeAttachment(), Subject = "Assunto", Message = "Mensagem vinda da tela.", UserId = 3 });

            await mailNotification.Handle(new Application.MailNotification.MailNotification
            {
                Message = resultMesage,
                Subject = "Assunto",
                To = smtp.ContactUs
            }, CancellationToken.None);
        }

        private IFormFile MakeAttachment()
        {
            //Or create a IFormFile real
            //Need install package asp.net core http
            //IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("This is a dummy file to test - Demis")), 0, 0, "Data", "dummy-test-file.txt");

            //Setting attachment
            var fileMock = new Mock<IFormFile>();
            //Setup mock file using a memory stream
            var content = "Hello World do meu arquivo fake";
            var fileName = "test-demis.pdf";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            var file =  fileMock.Object;
            return file;
        }
    }
}