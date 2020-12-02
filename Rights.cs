using System;
using System.Linq;

namespace Jwpro.Api.Proxy.Gmail
{
    public enum Right
    {
        GmailAddonsCurrentActionCompose,
        GmailAddonsCurrentMessageAction,
        GmailAddonsCurrentMessageMetadata,
        GmailAddonsCurrentMessageReadonly,
        GmailCompose,
        GmailInsert,
        GmailLabels,
        GmailMetadata,
        GmailModify,
        GmailReadonly,
        GmailSend,
        GmailSettingsBasic,
        GmailSettingsSharing,
        MailGoogleCom
    }
}
