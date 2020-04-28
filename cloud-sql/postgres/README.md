# Google Cloud SQL with PostgreSQL and Google App Engine Flexible Environment

This sample application demonstrates how to store data in Google Cloud SQL
with a PostgreSQL database when running in Google App Engine Flexible Environment.

## Prerequisites

0.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**
  
1.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=sqladmin.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud SQL API.

2.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

3.  Install the [.NET Core SDK, version 2.0](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.5-download.md)
    or newer.

4.  [Create a second generation Google Cloud SQL instance](
    https://cloud.google.com/sql/docs/postgres/create-instance).

6.  Under the instance's "USERS" tab, create a new user. Note the "User name" and "Password".

7.  Create a new database in your Google Cloud SQL instance.
    
    1.  List your database instances in [Cloud Cloud Console](
        https://console.cloud.google.com/sql/instances/).
    
    2.  Click your Instance Id to see Instance details.

    3.  Click DATABASES.

    4.  Click **Create database**.

    2.  For **Database name**, enter `votes`.

    3.  Click **CREATE**.


8.  Edit [appsettings.json](appsettings.json).  
    1. Update the connection string
       with the "User name" and "Password" you previously created on the Cloud SQL instance.
    2. Replace `your-project-id` with your Google Cloud Project's project id.

## ![PowerShell](../.resources/powershell.png) Using PowerShell

### Run Locally

1.  Download and run the [Google Cloud SQL Proxy](https://cloud.google.com/sql/docs/mysql/sql-proxy).

2.  Edit [appsettings.json](appsettings.json).  Replace `cloudsql` with `127.0.0.1` in the connection string.

3.  Run in powershell:

    ```psm1
    PS > dotnet restore
    PS > dotnet run
    ```

### Deploy to Cloud Run

1.  Edit [appsettings.json](appsettings.json).  Update the connection string to be in the following
    form:
        
        "Server=/cloudsql/your-project-id:us-central1:instance-name;Uid=aspnetuser;Pwd=;Database=votes"
    
    replacing `your-project-id`, `us-central1` and `instance-name` with your Google Cloud Project's
    project id and the [location/region](https://cloud.google.com/compute/docs/regions-zones) and name
    of the Cloud SQL instance you created in a previous step. Also replace the values for
    `Uid` and `Pwd` with the "User name" and "Password" you previously created on the Cloud SQL instance.

2.  Run the following command in powershell to build the container image that you will deploy as a Cloud Run service:

    ```psm1    
        gcloud builds submit --tag gcr.io/your-project-id/postgresql
    ```
    where `your-project-id` is your Google Cloud Project's project id.

3.  In the [Cloud Run](https://console.cloud.google.com/run) section of the Google Cloud Console
    click the "CREATE SERVICE" Button.
    1.  For "Service settings : Deployment platform" select "Cloud Run (fully managed)".

    2.  Set the "Region" to be the same "Region" you chose when you created your Cloud SQL instance.

    3.  Enter a name of your choice for your service in the "Service name" text box.

    4.  For "Authentication" select "Allow unauthenticated invocations".

    5.  Click the "NEXT" Button

    6.  For "Configure the service's first revision" click the "SELECT" button and select the container image
        you just built in a previous  step.

    7.  Click "SHOW ADVANCED SETTINGS" and then select the "CONNECTIONS" tab.

    8.  Click the "ADD CONNECTION" button and choose the Cloud SQL instance you created in a previous step.

    9.  Click the "CREATE" button to create the Cloud Run service.

Once your service is created, visit your running app by clicking the service's URL displayed at the top of the page.

To update your deployed Cloud Run service, update your app then run the following commands in powershell
(replacing `your-project-id` with your Google Cloud Project's project id):
1.  Re-run the `gcloud builds submit` command to update your container image:

    ```psm1    
        gcloud builds submit --tag gcr.io/your-project-id/postgresql
    ```

2. Run the `gcloud run deploy` command to update your deployed Cloud Run service:

    ```psm1
         gcloud run deploy --image gcr.io/your-project-id/postgresql --platform managed
    ```


### Deploy to App Engine Flexible

1.  Edit [app.yaml](app.yaml).  Replace `your-project-id:us-central1:instance-name`
    with your instance connection name, and update the tcp port number to 
    `5432` for a PostgreSQL instance or `3306` for a MySQL instance.

2.  Run in powershell:

    ```psm1
    PS > dotnet restore
    PS > dotnet publish
    PS > gcloud beta app deploy .\bin\Debug\netcoreapp2.1\publish\app.yaml
    ```

## ![Visual Studio](../.resources/visual-studio.png) Using Visual Studio 2017

[Google Cloud Tools for Visual Studio](
https://marketplace.visualstudio.com/items?itemName=GoogleCloudTools.GoogleCloudPlatformExtensionforVisualStudio)
make it easy to deploy to App Engine.  Install them if you are running Visual Studio.

### Run Locally

1.  Download and run the [Google Cloud SQL Proxy](https://cloud.google.com/sql/docs/mysql/sql-proxy).

2.  Edit [appsettings.json](appsettings.json).  Replace `cloudsql` with `127.0.0.1` in the connection string.

3.  Open **CloudSql.csproj**, and Press **F5**.

### Deploy to App Engine Flexible

0.  Edit [app.yaml](app.yaml).  Replace `your-project-id:us-central1:instance-name`
    with your instance connection name.

1.  In Solution Explorer, right-click the **CloudSql** project and choose **Publish CloudSql to Google Cloud**.

2.  Click **App Engine Flex**.

3.  Click **Publish**.
