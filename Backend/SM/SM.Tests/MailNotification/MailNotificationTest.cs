using CMC.Common.Abstractions.Notification;
using CMC.Common.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SM.Domain.Options;
using System.Threading;
using System.Threading.Tasks;
using static SM.Application.MailNotification.MailNotification;

namespace SM.Tests.MailNotification
{
    [TestClass]
    public class MailNotificationTest
    {
        private readonly ISenderMail senderMail;
        private readonly IUnitOfWork unitOfWork;
        private readonly UserTestDTO userData;

        public MailNotificationTest()
        {
            senderMail = new SenderMail();
            unitOfWork = UnitOfWorkTest.GetUnitOfWork();
            userData = UnitOfWorkTest.RetrieveUserId();
        }
        [TestMethod]
        public async Task SendMailContactUs()
        {
            var smtp = new SmtpConfiguration
            {
                ContactUs = "thiagosilverio@doubleit.com.br",
                From = "carreira.noreply@carreira.com.br",
                SmtpHost = "smtplw.com.br",
                SmtpPassword = "cmul1836#",
                SmtpPort = 587,
                SmtpUser = "carreira"
            };

            var mailNotification = new MailNotificationHandler(senderMail, smtp);
            await mailNotification.Handle(new Application.MailNotification.MailNotification
            {
                Message = "Teste de envio de e-mail",
                Subject = "Meu Teste",
                To = smtp.ContactUs
            }, CancellationToken.None);
        }
    }
}
