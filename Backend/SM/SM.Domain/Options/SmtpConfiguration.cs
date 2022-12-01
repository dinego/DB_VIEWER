﻿namespace SM.Domain.Options
{
    public class SmtpConfiguration
    {
        public string ContactUs { get; set; }
        public string From { get; set; }
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPassword { get; set; }
    }
}
