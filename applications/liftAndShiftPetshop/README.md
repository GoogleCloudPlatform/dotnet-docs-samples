# Lift and Shift of legacy Pet Shop .NET app to Google Cloud Platform.

=================================================
This is a demo to show how to migrate existing legacy .NET application to Google Cloud Platform.
The demo was shown at Google Cloud Next 2018. [Video](https://www.youtube.com/watch?v=GKGpqLMrFmA&list=PL2N3MR2iGu2E9ECzS-FFQ0ZHOmkhbtD1T&index=6&t=56s)
It consists of two parts - backend (SQL Server) and frontend (ASP.NET Web Pages - legacy)

## Prerequisites

This project requires Windows and MSBuild, including

* .NET Framework 4.5+

* Visual Studio 2017

* SQL Server

* MSBUILD

## Deployment

1. Create a new instance of Sql Server on Google Cloud Platform.
Use [Codelab](https://codelabs.developers.google.com/codelabs/cloud-create-sql-server/index.html?index=..%2F..index#0) for instructions.

2. Create a new instance of Compute with Windows and IIS enabled on Google Cloud Platform.
Use [Codelab](https://codelabs.developers.google.com/codelabs/cloud-compute-engine-aspnet/#0) for instructions.

3. Update web.config with the connection string of the database created in step 1.

4. Deploy the app to compute instance created in previous step. You can use Web Deploy feature from Visual Studio.
More info on [Web Deploy](https://docs.microsoft.com/en-us/visualstudio/deployment/tutorial-import-publish-settings-iis?view=vs-2019).

5. Create a static IP for the compute instance created in step 2.
Use [this](https://cloud.google.com/compute/docs/ip-addresses/reserve-static-external-ip-address) doc for instructions.

## Run

Access the app using the static external IP created in step 5.

This is not an official Google product.
