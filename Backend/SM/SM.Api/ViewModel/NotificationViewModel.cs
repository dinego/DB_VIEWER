using Microsoft.AspNetCore.Http;

namespace SM.Api.ViewModel
{
    public class ContactUsRequestViewModel
    {
        public IFormFile Attachment { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }

    public class SetReadNotificationViewModel
    {
        public long NotificationId { get; set; }
    }

    public class SendLinkViewModel
    {
        public string To { get; set; }
        public string Message { get; set; }
        public string Url { get; set; }
    }
}
