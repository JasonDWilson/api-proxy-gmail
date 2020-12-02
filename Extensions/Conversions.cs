using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Jwpro.Api.Proxy.Gmail.Extensions
{
    static class Conversion
    {
        private static String decodeBase64String(string s)
        {
            var ts = s.Replace("-", "+");
            ts = ts.Replace("_", "/");
            var bc = Convert.FromBase64String(ts);
            var tts = Encoding.UTF8.GetString(bc);

            return tts;
        }

        private static String getNestedBodyParts(IList<MessagePart> part, string curr)
        {
            string str = curr;
            if(part == null)
            {
                return str;
            } else
            {
                foreach(var parts in part)
                {
                    if(parts.Parts == null)
                    {
                        if(parts.Body != null && parts.Body.Data != null)
                        {
                            var ts = decodeBase64String(parts.Body.Data);
                            str += ts;
                        }
                    } else
                    {
                        return getNestedBodyParts(parts.Parts, str);
                    }
                }

                return str;
            }
        }

        private static string stringToBase64url(string data)
        {
            return Convert.ToBase64String(Encoding.ASCII.GetBytes(data))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static Email ToEmail(this Message message, GmailService service)
        {
            var email = new Email();
            if(message != null)
            {
                email.Id = message.Id;
                foreach(var header in message.Payload.Headers)
                {
                    switch(header.Name)
                    {
                        case "Date":
                            email.Date = header.Value != null ? (DateTime?)DateTime.Parse(header.Value) : null;
                            break;
                        case "From":
                            email.From = header.Value;
                            break;
                        case "Subject":
                            email.Subject = header.Value;
                            break;
                    }
                }

                if(email.Date != null && !string.IsNullOrWhiteSpace(email.From))
                {
                    if(message.Payload.Parts == null && message.Payload.Body != null)
                        email.Body = decodeBase64String(message.Payload.Body.Data);
                    else
                        email.Body = getNestedBodyParts(message.Payload.Parts, string.Empty);

                    IList<MessagePart> parts = message.Payload.Parts;
                    foreach(MessagePart part in parts)
                    {
                        if(!String.IsNullOrEmpty(part.Filename))
                        {
                            var attachment = new Attachment();
                            attachment.Id = part.Body.AttachmentId;
                            MessagePartBody attachPart = service.Users.Messages.Attachments
                                .Get("me", message.Id, attachment.Id)
                                .Execute();

                            // Converting from RFC 4648 base64 to base64url encoding
                            // see http://en.wikipedia.org/wiki/Base64#Implementations_and_history
                            String attachData = attachPart.Data.Replace('-', '+');
                            attachData = attachData.Replace('_', '/');

                            attachment.File = Convert.FromBase64String(attachData);
                            attachment.Name = part.Filename;
                            email.Attachments.Add(attachment);
                        }
                    }
                }
                return email;
            }
            return null;
        }

        public static string ToGmailScope(this Right right)
        {
            var fields = typeof(GmailService.Scope).GetFields();
            foreach(var field in fields)
            {
                if(field.Name == right.ToString())
                    return field.GetValue(null).ToString();
            }
            return null;
        }

        public static Message ToMessage(this Email email)
        {
            Message message = new Message();
            var mailMessage = new System.Net.Mail.MailMessage();
            mailMessage.From = new System.Net.Mail.MailAddress(email.From);
            mailMessage.To.Add(email.To);
            mailMessage.ReplyToList.Add(email.From);
            mailMessage.Subject = email.Subject;
            mailMessage.Body = email.Body;
            mailMessage.IsBodyHtml = email.IsHtml;

            foreach(var attachment in email.Attachments)
            {
                System.Net.Mail.Attachment a = new System.Net.Mail.Attachment(
                    new MemoryStream(attachment.File),
                    attachment.Name);
                mailMessage.Attachments.Add(a);
            }

            var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(mailMessage);
            var s = mimeMessage.ToString();
            message.Raw = stringToBase64url(mimeMessage.ToString());
            return message;
        }
    }
}
