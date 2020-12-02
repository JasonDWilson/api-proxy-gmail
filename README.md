# api-proxy-gmail:  A .Net proxy wrapper for the Gmail API

*Please note that this is not intended to be a comprehensive coverage of the Gmail API
I only included what I needed when I wrote it.*

### If there are things you need feel free to contribute

## Authentication: ##
Gmail uses oAuth2 for authentication.  To configure a gmail account to be accessed through the API you will need to go to the Google API [documententation](https://developers.google.com/gmail/api/auth/about-auth) and configure it in the [Google API Console](https://developers.google.com/identity).  You can also use this [Quickstart Guide](https://developers.google.com/gmail/api/quickstart/dotnet).  In either case you will generate a *credential file* for your application.  When you application is launched if a token has not already been generated, the user will be prompted to log into the gmail account and authorize the application for access and then a token file is generated and stored for the application.

## Usage: ##
- All usage is governed through the **DataManager** class
- DataManager expects a **GmailApiConfiguration** object to be injected that contains the path to the *credential file* generated above, the application name, and the rights to the gmail account that you are requesting.

```csharp
var dm = new DataManager(new GmailApiConfiguration
{ 
  ApplicationName=*your application*, 
  PathToCredentialFile=*path to credentials.json*,
  Rights= new List<Right>{ *rights you wish to grant* }
});
var courses = dm.GetInboxEmails();
```

*please let me know if you have questions
Jason D Wilson*
