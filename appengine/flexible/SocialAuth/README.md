# Social Authentication and Google App Engine Flexible Environment

This sample application demonstrates how to let users login into your application
running in Google App Engine Flexible Environment with their Facebook or Google
log in.

## Prerequisites

Yeah, there's a ton of prerequisites.  But every one of them is necessary.
Hang in there.

1.  **Follow the instructions in the [root README](../../../README.md).**
  
2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The 
	Google Cloud SDK is required to deploy .NET applications to App Engine.

3.  Install [.NET Core 1.0.3 SDK Preview 2 build 3156](https://github.com/dotnet/core/blob/master/release-notes/download-archives/1.0.3-preview2-download.md).

4.  Visual Studio is not required, but to build and run .NET *core* applications,
    Visual Studio users need to download and install 
	[.NET Core tools](https://www.microsoft.com/net/core#windowsvs2015).

5.  [Google Cloud Tools for Visual Studio](
	https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
    make it easy to deploy to App Engine.  Install them if you are running 
	Visual Studio.

6.  [Click here](https://console.cloud.google.com/flows/enableapi?apiid=cloudkms.googleapis.com&showconfirmation=true) 
	to enable [Google Cloud Key Management Service](https://cloud.google.com/kms/)
	for your project.

7.  Make the App Engine default service account a 
	Cloud KMS CryptoKey Encrypter/Decrypter.
	
	1.  Visit [the IAM section of Google Cloud Console](https://console.cloud.google.com/iam-admin/iam/project).

	2.  Click the `Roles` dropdown next to `App Engine default service account`.  

	3.  Under `Cloud KMS`, click **Cloud KMS CryptoKey Encrypter/Decrypter**.

	4.  Click **Save**.

8.  Deploy an SQL Server instance on Google Cloud Platform.
	Follow the instructions 
	[here](https://cloud.google.com/dotnet/docs/getting-started/using-sql-server),
	but **stop** when you reach the **Configuration Settings** section that talks 
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

## Run Locally

### ![PowerShell](../.resources/powershell.png)Using PowerShell
```ps1
PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet restore
PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet run
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio
1.  In Solution Explorer, right-click the **SocialAuth** project and choose **Set as StartUp Project**
2.  Press F5.

## Deploy to App Engine

Before deploying to app engine, you must copy your user secrets to your Google
project metadata with commands like this:

```
PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> .\Upload-UserSecrets
```

### ![PowerShell](../.resources/powershell.png)Using PowerShell


```psm1
PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet restore
PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> dotnet publish
PS C:\dotnet-docs-samples\appengine\flexible\SocialAuth> gcloud beta app deploy .\bin\Debug\netcoreapp1.0\publish\app.yaml
```

### ![Visual Studio](../.resources/visual-studio.png)Using Visual Studio


1.  In Solution Explorer, right-click the **SocialAuth** project and choose 
    **Publish SocialAuth to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.