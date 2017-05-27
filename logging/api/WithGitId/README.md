# Stackdriver Logging WithGitId Sample

A sample demonstrating how to add source context git id to Stackdriver log entries.

## Links

- [Cloud Logging Reference Docs](https://cloud.google.com/logging/docs/)

## Download install [Google Cloud Tools for Visual Studio](https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio) 
* You need Microsoft Visual Studio version 2015 or later.
* Restart Visual Studio after installation

## Deploy an [ASP.NET image](https://console.cloud.google.com/launcher/details/click-to-deploy-images/aspnet?_ga=1.162497535.236660576.1482928062)

## Build and Run

1.  **Follow the instructions in the [root README](../../../README.md)**.

2.  Open [WithGitId project](WithGitId.csproj) 

3.  Edit `StackdriverLogWriter.cs`, and replace YOUR-PROJECT-ID with id
    of the project you created in step 1.  
	Replace YOUR-LOG-ID with whatever log id name you like.

4.  Build and deploy the solution
* In Solution explorer, select project WithGitId. 
* Right click and select "Set as default project"
* Right click the project again, select "Publish to Google Cloud"  
  Make sure to select "launch web page after deployment"
* Follow the steps and deploy it to the VM you just created using ASP.NET image.

5. By the end of deployment, you should be able to see the test web page. 
* Click the button on the page to generate log entries.
* In Visaul Studio, choose Tools | Google Cloud Tools | Browse Stackdriver Logs 
* In the logs viewer, select Globla resource.  You should be able to see the log entires just created.
  
	
## Contributing changes

* See [CONTRIBUTING.md](../../../CONTRIBUTING.md)

## Licensing

* See [LICENSE](../../../LICENSE)
