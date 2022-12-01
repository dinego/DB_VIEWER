using AgileObjects.AgileMapper.Extensions;
using CMC.Common.Abstractions.Notification;
using MediatR;
using SM.Domain.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SM.Application.MailNotification
{
    public class MailNotification : INotification
    {
        public string To { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }
        public List<FileUpload> Files { get; set; }
        public class MailNotificationHandler : INotificationHandler<MailNotification>
        {
            private readonly ISenderMail _senderMail;
            private readonly SmtpConfiguration _smtp;
            public MailNotificationHandler(ISenderMail senderMail, SmtpConfiguration smtp)
            {
                _senderMail = senderMail;
                _smtp = smtp;
            }
            public async Task Handle(MailNotification notification, CancellationToken cancellationToken)
            {
                var senderConfig = _smtp.Map().ToANew<SenderMailConfig>();
                senderConfig.Message = notification.Message;
                senderConfig.Subject = notification.Subject;
                senderConfig.To = notification.To;
                if (notification.Files != null)
                    senderConfig.Files = notification.Files;
                await _senderMail.Send(senderConfig);
            }
        }
    }
}
