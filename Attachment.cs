using System;
using System.Linq;

namespace Jwpro.Api.Proxy.Gmail
{
    public class Attachment
    {
        public byte[] File { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }
    }
}
