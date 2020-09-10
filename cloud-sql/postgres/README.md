# Google Cloud SQL with PostgreSQL and Google App Engine Flexible Environment

This sample application demonstrates how to store data in Google Cloud SQL
with a PostgreSQL database when running in Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**
  
1.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=sqladmin.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud SQL API.

1.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

1.  Install the [.NET Core SDK, version 2.0](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.5-download.md)
    or newer.

1.  Create a Cloud SQL for Postgres instance by following these 
    [instructions](https://cloud.google.com/sql/docs/postgres/create-instance).
    Note the connection string, database user, and database password that you create.

1.  Create a database for your application by following these 
    [instructions](https://cloud.google.com/sql/docs/postgres/create-manage-databases).
    Note the database name. 

1.  Replace `your-project-id` in [appsettings.json](appsettings.json) with your Google Cloud Project's project id.

## Running locally

To run this application locally, download and install the `cloud_sql_proxy` by
following the instructions
[here](https://cloud.google.com/sql/docs/postgres/sql-proxy#install).

Instructions are provided below for using the proxy with a TCP connection or a Unix Domain Socket.
On Linux or Mac OS you can use either option, but on Windows the proxy currently requires a TCP connection.

### Launch proxy with TCP

To run the sample locally with a TCP connection, set environment variables and launch the proxy as shown below.

#### Linux / Mac OS
Use these terminal commands to initialize environment variables:
```bash
export GOOGLE_APPLICATION_CREDENTIALS=/path/to/service/account/key.json
export DB_HOST='127.0.0.1'
export DB_USER='<DB_USER_NAME>'
export DB_PASS='<DB_PASSWORD>'
export DB_NAME='<DB_NAME>'
```

Note: Saving credentials in environment variables is convenient, but not secure - consider a more secure solution such as [Cloud Secret Manager](https://cloud.google.com/secret-manager) to help keep secrets safe.


Then use this command to launch the proxy in the background:
```bash
./cloud_sql_proxy -instances=<project-id>:<region>:<instance-name>=tcp:5432 -credential_file=$GOOGLE_APPLICATION_CREDENTIALS &
```

Finally, run the following commands:
```bash
dotnet restore
dotnet run
```

#### Windows/PowerShell
Use these PowerShell commands to initialize environment variables:
```powershell
$env:GOOGLE_APPLICATION_CREDENTIALS="<CREDENTIALS_JSON_FILE>"
$env:DB_HOST="127.0.0.1"
$env:DB_USER="<DB_USER_NAME>"
$env:DB_PASS="<DB_PASSWORD>"
$env:DB_NAME="<DB_NAME>"
```

Note: Saving credentials in environment variables is convenient, but not secure - consider a more secure solution such as [Cloud Secret Manager](https://cloud.google.com/secret-manager) to help keep secrets safe.


Then use this command to launch the proxy in a separate PowerShell session:
```powershell
Start-Process -filepath "C:\<path to proxy exe>" -ArgumentList "-instances=<project-id>:<region>:<instance-name>=tcp:5432 -credential_file=<CREDENTIALS_JSON_FILE>"
```
Finally, run the following commands:
```psm1
PS > dotnet restore
PS > dotnet run
```
#### Visual Studio 2017

Download and run the [Google Cloud SQL Proxy](https://cloud.google.com/sql/docs/mysql/sql-proxy).

Then, navigate to the project options menu. Select the default configuration, then set the following environment variables:

```
DB_HOST : '127.0.0.1' 
DB_USER : '<DB_USER_NAME>' 
DB_PASS : '<DB_PASSWORD>'
DB_NAME : '<DB_NAME>'
```

Note: Saving credentials in environment variables is convenient, but not secure - consider a more secure solution such as [Cloud Secret Manager](https://cloud.google.com/secret-manager) to help keep secrets safe.


Finally, run the application by clicking the arrow button in the main toolbar, or by pressing F5.

### Launch proxy with Unix Domain Socket
NOTE: this option is currently only supported on Linux and Mac OS. Windows users should use the [Launch proxy with TCP](#launch-proxy-with-tcp) option.

To use a Unix socket, you'll need to create a directory and give write access to the user running the proxy. For example:
```bash
sudo mkdir ./cloudsql
sudo chown -R $USER ./cloudsql
```

You'll also need to initialize an environment variable containing the directory you just created:

```bash
export DB_SOCKET_DIR=./cloudsql
```

Use these terminal commands to initialize environment variables:
```bash
export GOOGLE_APPLICATION_CREDENTIALS=/path/to/service/account/key.json
export INSTANCE_CONNECTION_NAME='<MY-PROJECT>:<INSTANCE-REGION>:<INSTANCE-NAME>'
export DB_USER='<DB_USER_NAME>'
export DB_PASS='<DB_PASSWORD>'
export DB_NAME='<DB_NAME>'
```

Then use this command to launch the proxy in the background:
```bash
./cloud_sql_proxy -dir=$DB_SOCKET_DIR --instances=$INSTANCE_CONNECTION_NAME --credential_file=$GOOGLE_APPLICATION_CREDENTIALS &

Finally, run the following commands:
```bash
dotnet restore
dotnet run
```

### Deploy to Cloud Run

See the [Cloud Run documentation](https://cloud.google.com/sql/docs/postgres/connect-run)
for more details on connecting a Cloud Run service to Cloud SQL.

Build the container image:

```sh
gcloud builds submit --tag gcr.io/[YOUR_PROJECT_ID]/run-sql
```

Deploy the service to Cloud Run. Replace environment variables with the correct values for your Cloud SQL
instance configuration:

```sh
gcloud run deploy run-sql --image gcr.io/[YOUR_PROJECT_ID]/run-sql \              
  --add-cloudsql-instances '<MY-PROJECT>:<INSTANCE-REGION>:<INSTANCE-NAME>' \
  --set-env-vars INSTANCE_CONNECTION_NAME='<MY-PROJECT>:<INSTANCE-REGION>:<INSTANCE-NAME>' \
  --set-env-vars DB_USER='<DB_USER_NAME>' \
  --set-env-vars DB_PASS='<DB_PASSWORD>' \
  --set-env-vars DB_NAME='<DB_NAME>'
```

Take note of the URL output at the end of the deployment process and navigate your browser to that URL.

For more details about using Cloud Run see http://cloud.run.




### Deploy to App Engine Flexible

1.  Edit [app.yaml](app.yaml).  Replace `your-project-id:us-central1:instance-name`
    with your instance connection name, and update the tcp port number to 
    `5432` for a PostgreSQL instance or `3306` for a MySQL instance. Update the values
    for `DB_USER`, `DB_PASS`, and `DB_NAME`

2.  Build and deploy the application:

    #### Powershell
    ```psm1
    PS > dotnet restore
    PS > dotnet publish
    PS > gcloud beta app deploy .\bin\Debug\netcoreapp2.1\publish\app.yaml
    ```
        #### Bash
    ```bash
    dotnet restore
    dotnet publish
    gcloud beta app deploy ./bin/Debug/netcoreapp2.1/publish/app.yaml
    ```
