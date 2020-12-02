using System;
using System.Collections.Generic;
using System.Linq;

namespace Jwpro.Api.Proxy.Gmail
{
    public class Email
    {
        public List<Attachment> Attachments { get; set; } = new List<Attachment>();

        public string Body { get; set; }

        public DateTime? Date { get; set; }

        public string From { get; set; }

        public string Id { get; set; }

        public bool IsHtml { get; set; }

        public string Subject { get; set; }

        public string To { get; set; }
    }
}
