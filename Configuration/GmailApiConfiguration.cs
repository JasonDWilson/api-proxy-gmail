using System;
using System.Collections.Generic;
using System.Linq;

namespace Jwpro.Api.Proxy.Gmail.Configuration
{
    public class GmailApiConfiguration
    {
        public string ApplicationName { get; set; }

        public string PathToCredentialFile { get; set; }

        public List<Right> Rights { get; set; }
    }
}
