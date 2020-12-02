
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Jwpro.Api.Proxy.Gmail.Configuration;
using Jwpro.Api.Proxy.Gmail.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Jwpro.Api.Proxy.Gmail
{
    public class DataManager
    {
        private GmailApiConfiguration _config;
        private UserCredential _credential;
        private GmailService _service;

        public DataManager(GmailApiConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            string[] scopes = _config.Rights.Select(x => x.ToGmailScope()).ToArray();
            _credential = authorize(_config.PathToCredentialFile, scopes);

            // Create Gmail API service.
            _service = new GmailService(
                new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = _config.ApplicationName,
                });
        }

        private UserCredential authorize(string pathToCredentials, string[] scopes)
        {
            using(var stream =
                new FileStream(pathToCredentials, FileMode.Open, FileAccess.Read))
            {
                return GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true))
                    .Result;
            }
        }

        public void DeleteEmail(string emailId) => _service.Users.Messages.Delete("me", emailId).Execute();

        public List<Email> GetInboxEmails()
        {
            var results = new List<Email>();


            var inboxRequest = _service.Users.Messages.List("me");
            inboxRequest.LabelIds = "INBOX";
            //re.Q = "is:unread"; //only get unread;
            var inboxResponse = inboxRequest.Execute();

            if(inboxResponse != null && inboxResponse.Messages != null)
            {
                foreach(var message in inboxResponse.Messages)
                {
                    var emailResponse = _service.Users.Messages.Get("me", message.Id).Execute();
                    var email = emailResponse.ToEmail(_service);
                    if(email != null)
                        results.Add(email);
                }
            }

            return results;
        }

        public void SendEmail(Email email) => _service.Users.Messages.Send(email.ToMessage(), "me").Execute();

        public void TrashEmail(string emailId) => _service.Users.Messages.Trash("me", emailId).Execute();
    }
}
