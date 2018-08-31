# Social Authentication and Google App Engine Flexible Environment

This is the companion sample for a [medium.com story](
	https://medium.com/@SurferJeff/adding-social-login-to-your-net-app-engine-application-9b7f4149eb73).

This sample application demonstrates how to let users log in into your application
running in Google App Engine Flexible Environment with their Facebook or Google
log in.  

## Prerequisites

Yeah, there's a ton of prerequisites.  But every one of them is necessary.
Hang in there.

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The 
	Google Cloud SDK is required to deploy .NET applications to App Engine.

2.  Install the [.NET Core SDK, version 1.1](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.1.4-download.md).

6.  [Click here](https://console.cloud.google.com/flows/enableapi?apiid=cloudkms.googleapis.com&showconfirmation=true) 
	to enable [Google Cloud Key Management Service](https://cloud.google.com/kms/)
	for your project.

7.  Make the App Engine default service account and Compute Engine default service accounts
	Cloud KMS CryptoKey Encrypter/Decrypters.
	
	1.  Visit [the IAM section of Google Cloud Console](https://console.cloud.google.com/iam-admin/iam/project).

	2.  Click the `Roles` dropdown next to `App Engine default service account`.  

	3.  Under `Cloud KMS`, click **Cloud KMS CryptoKey Encrypter/Decrypter**.

	4.  Click **Save**.

	5.  Click the `Roles` dropdown next to `Compute Engine default service account`.  

	6.  Under `Cloud KMS`, click **Cloud KMS CryptoKey Encrypter/Decrypter**.

	7.  Click **Save**.

8.  Deploy an SQL Server instance on Google Cloud Platform.
	Follow the instructions 
	[here](https://cloud.google.com/dotnet/docs/getting-started/using-sql-server),
	but **stop** when you reach the **Configuring settings** section that talks 
	about `Web.config.`  .NET core applications do not use `Web.config` for
	configuration.

9.  Save your connection string to a local user secret:
    
	```ps1
	PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet restore
	...
	PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet user-secrets set ConnectionStrings:DefaultConnection  'Server=1.2.3.4;Uid=dotnetapp;Pwd=XXXXXXXX'
	```

9.  Initialize your database by running:

	```
	PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet ef migrations add init
	PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet ef database update
	```

10. Edit [appsettings.json](appsettings.json).

	Replace `YOUR-PROJECT-ID` with your Google project id.

11. Configure your social auth provider.

	* [Facebook](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins)
	  instructions. 
	* [Google](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins)
	  instructions.

	  For **Authorized redirect URIs** enter:
	  
	  ```
	  https://localhost:44393/signin-google,https://YOUR-PROJECT-ID.appspot.com/signin-google
	  ```

	  Stop when you reach **Enable Google Middleware**.  The instructions after 
	  **Enable Google Middleware** have already been applied to the SocialAuth
	  project.


## ![PowerShell](../.resources/powershell.png) Using PowerShell

### Run Locally

```ps1
PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet restore
PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet run
```
### Deploy to App Engine

6.  Before deploying to app engine, you must copy your user secrets to your Google
project metadata with this powershell script:

	```psm1
	PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> .\Upload-UserSecrets
	```

7.  Deploy with gcloud:

	```psm1
	PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> gcloud beta app deploy .\bin\Release\PublishOutput\app.yaml
	```


## ![Visual Studio](../.resources/visual-studio.png) Using Visual Studio

### Run Locally

6.  Before deploying to app engine, you must copy your user secrets to your Google
project metadata with this powershell script:

	```psm1
	PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> .\Upload-UserSecrets
	```

Open **SocialAuth.csproj**, and Press **F5**.

### Deploy to App Engine

1.  In Solution Explorer, right-click the **SocialAuth** project and choose **Publish SocialAuth to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.
