# Google Cloud SQL with MySQL Sample for Cloud Run and Google App Engine Flexible Environment

This sample application demonstrates how to store data in Google Cloud SQL
with a MySQL database when running in Cloud Run or Google App Engine Flexible Environment.

## Prerequisites

1.  **Follow the set-up instructions in [the documentation](https://cloud.google.com/dotnet/docs/setup).**
  
1.  Enable APIs for your project.
    [Click here](https://console.cloud.google.com/flows/enableapi?apiid=sqladmin.googleapis.com&showconfirmation=true)
    to visit Cloud Platform Console and enable the Google Cloud SQL API.

1.  Install the [Google Cloud SDK](https://cloud.google.com/sdk/).  The Google Cloud SDK
    is required to deploy .NET applications to App Engine.

1.  Install the [.NET Core SDK, version 2.0](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.5-download.md)
    or newer.

1.  Create a Cloud SQL for MySQL instance by following these 
    [instructions](https://cloud.google.com/sql/docs/mysql/create-instance).
    Note the instance connection name, database user, and database password that you create.

1.  Create a database for your application by following these 
    [instructions](https://cloud.google.com/sql/docs/mysql/create-manage-databases).
    Note the database name. 

1.  Replace `your-project-id` in [appsettings.json](appsettings.json) with your Google Cloud Project's project id.

## Running locally

To run this application locally, download and install the `cloud_sql_proxy` by
following the instructions
[here](https://cloud.google.com/sql/docs/mysql/sql-proxy#install).

Instructions are provided below for using the proxy with a TCP connection or a Unix Domain Socket.
On Linux or Mac OS you can use either option, but on Windows the proxy currently requires a TCP connection.

### Launch proxy with TCP

To run the sample locally with a TCP connection, set environment variables and launch the proxy as shown below.

#### Linux / Mac OS
Use these terminal commands to initialize environment variables:
```bash
export GOOGLE_APPLICATION_CREDENTIALS=/path/to/service/account/key.json
export INSTANCE_HOST='127.0.0.1'
export DB_USER='<DB_USER_NAME>'
export DB_PASS='<DB_PASSWORD>'
export DB_NAME='<DB_NAME>'
```

Note: Saving credentials in environment variables is convenient, but not secure - consider a more secure solution such as [Cloud Secret Manager](https://cloud.google.com/secret-manager) to help keep secrets safe.


Then use this command to launch the proxy in the background:
```bash
./cloud_sql_proxy -instances=<PROJECT-ID>:<INSTANCE-REGION>:<INSTANCE-NAME>=tcp:3306 -credential_file=$GOOGLE_APPLICATION_CREDENTIALS &
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
$env:INSTANCE_HOST="127.0.0.1"
$env:DB_USER="<DB_USER_NAME>"
$env:DB_PASS="<DB_PASSWORD>"
$env:DB_NAME="<DB_NAME>"
```

Note: Saving credentials in environment variables is convenient, but not secure - consider a more secure solution such as [Cloud Secret Manager](https://cloud.google.com/secret-manager) to help keep secrets safe.


Then use this command to launch the proxy in a separate PowerShell session:
```powershell
Start-Process -filepath "C:\<path to proxy exe>" -ArgumentList "-instances=<PROJECT-ID>:<INSTANCE-REGION>:<INSTANCE-NAME>=tcp:3306 -credential_file=<CREDENTIALS_JSON_FILE>"
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
INSTANCE_HOST : '127.0.0.1' 
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

Use these terminal commands to initialize environment variables:
```bash
export GOOGLE_APPLICATION_CREDENTIALS=/path/to/service/account/key.json
export INSTANCE_UNIX_SOCKET='./cloudsql/<MY-PROJECT>:<INSTANCE-REGION>:<INSTANCE-NAME>'
export DB_USER='<DB_USER_NAME>'
export DB_PASS='<DB_PASSWORD>'
export DB_NAME='<DB_NAME>'
```


Then use this command to launch the proxy in the background:
```bash
./cloud_sql_proxy -dir=./cloudsql --instances=<PROJECT-ID>:<INSTANCE-REGION>:<INSTANCE-NAME> --credential_file=$GOOGLE_APPLICATION_CREDENTIALS &

Finally, run the following commands:
```bash
dotnet restore
dotnet run
```

### Deploy to Cloud Run

See the [Cloud Run documentation](https://cloud.google.com/sql/docs/mysql/connect-run)
for more details on connecting a Cloud Run service to Cloud SQL.

Build the container image:

```sh
gcloud builds submit --tag gcr.io/[YOUR_PROJECT_ID]/run-sql
```

Deploy the service to Cloud Run. Replace environment variables with the correct values for your Cloud SQL instance configuration:

```sh
gcloud run deploy run-sql --image gcr.io/[YOUR_PROJECT_ID]/run-sql \              
  --add-cloudsql-instances '<PROJECT-ID>:<INSTANCE-REGION>:<INSTANCE-NAME>' \
  --set-env-vars INSTANCE_UNIX_SOCKET='/cloudsql/<PROJECT-ID>:<INSTANCE-REGION>:<INSTANCE-NAME>' \
  --set-env-vars DB_USER='<DB_USER_NAME>' \
  --set-env-vars DB_PASS='<DB_PASSWORD>' \
  --set-env-vars DB_NAME='<DB_NAME>'
```

Take note of the URL output at the end of the deployment process and navigate your browser to that URL.

It is recommended to use the [Secret Manager integration](https://cloud.google.com/run/docs/configuring/secrets) for Cloud Run instead
of using environment variables for the SQL configuration. The service injects the SQL credentials from
Secret Manager at runtime via an environment variable.

Create secrets via the command line:
```sh
echo -n $INSTANCE_UNIX_SOCKET | \
    gcloud secrets create [INSTANCE_UNIX_SOCKET_SECRET] --data-file=-
```

Deploy the service to Cloud Run specifying the env var name and secret name:
```sh
gcloud beta run deploy SERVICE --image gcr.io/[YOUR_PROJECT_ID]/run-sql \
    --add-cloudsql-instances <PROJECT-ID>:<INSTANCE-REGION>:<INSTANCE-NAME> \
    --update-secrets INSTANCE_UNIX_SOCKET=[INSTANCE_UNIX_SOCKET_SECRET]:latest, \
      DB_USER=[DB_USER_SECRET]:latest, \
      DB_PASS=[DB_PASS_SECRET]:latest, \
      DB_NAME=[DB_NAME_SECRET]:latest
```

For more details about using Cloud Run see http://cloud.run.

### Deploy to App Engine Flexible

1.  Edit [app.yaml](app.yaml).  Replace `<PROJECT-ID>:<INSTANCE-REGION>:<INSTANCE-NAME>`
    with your instance connection name. Update the values
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

## Configure SSL Certificates
For deployments that connect directly to a Cloud SQL instance with TCP,
without using the Cloud SQL Proxy,
configuring SSL certificates will ensure the connection is encrypted. 
1. Use the gcloud CLI to [download the server certificate](https://cloud.google.com/sql/docs/mysql/configure-ssl-instance#server-certs) for your Cloud SQL instance. Replace INSTANCE_NAME with the name of your Cloud SQL instance.
    - Get information about the service certificate:
        ```
        gcloud beta sql ssl server-ca-certs list --instance=INSTANCE_NAME
        ```
    - Create a server certificate:
        ```
        gcloud beta sql ssl server-ca-certs create --instance=INSTANCE_NAME
        ```
    - Download the certificate information to a local PEM file
        ```
        gcloud beta sql ssl server-ca-certs list --format="value(cert)" --instance=INSTANCE_NAME > certs/server-ca.pem
        ```

1. Use the gcloud CLI to [create and download a client public key certificate and client private key](https://cloud.google.com/sql/docs/mysql/configure-ssl-instance#client-certs).  Replace INSTANCE_NAME with the name of your Cloud SQL instance and replace CERT_NAME with certificate name of your choice.
    - Create a client certificate using the ssl client-certs create command:
        ```
        gcloud sql ssl client-certs create CERT_NAME certs/client-key.pem --instance=INSTANCE_NAME
        ```
    - Retrieve the public key for the certificate you just created and copy it into the client-cert.pem file with the ssl client-certs describe command:
        ```
        gcloud sql ssl client-certs describe CERT_NAME --instance=INSTANCE_NAME --format="value(cert)" > certs/client-cert.pem
        ```

1. Add SSL server certificate on client computer.
    - Linux (Debian)
        ```
        sudo cp certs/server-ca.pem /usr/local/share/ca-certificates/server-ca-mysql.crt
        sudo update-ca-certificates
        ```
    - Windows
        
        1. Click the **Start** menu and type **mmc**.
        2. Select the **mmc** application to open it.
        3. Click the **File** menu and select **Add/Remove Snap-in**.
        4. Select the **Certificates** and click **Add >**.
        5. In the **Certificates Snap-in" dialog window, select **Computer account** and click **Next**.
        6. In the **Select Computer** dialog window with **Local computer** selected, click **Finish**.
        7. Click **OK** to complete the process of adding the **Certificates** Snap-in.
        8. Expand the **Certificates (Local Computer)** Snap-in item and then expand the **Trusted Root Certification Authorities** item. 
        9. Right-click the **Certificates** item (that appears below the **Trusted Root Certification Authorities** item) and select **All Tasks > Import**. 
        10. In the **Certificate Import Wizard** dialog window, click **Next**.
        11. Click the **Browse** button.
        12. In the **Open** file dialog window, change the file type from **X.509 Certificate (*.cer,*.crt)** to be **All Files (*.*)**.
        13. Browse to and select the **server-ca.pem** file that you created in a previous step and then click **Open**.
        14. Click **Next** in the **Certificate Import Wizard > File to Import** dialog window.
        14. With **Place all certificates in the following store** selected and **Trusted Root Certification Authorities** specified as the **Certificate store**, click **Next** in the **Certificate Import Wizard > Certificate Store** dialog window.
        15. Click **Finish** to import the server certificate. 
        16. You will see a dialog window that confirms the certificate import was successful. Click **OK**.


1. Use openssl to combine the client SSL certificate and key into a .pfx file.
   ```
   openssl pkcs12 -export -in certs/client-cert.pem -inkey certs/client-key.pem -certfile certs/server-ca.pem -out certs/client.pfx
   ```

1. Specify the .pfx file as the client certificate via Environment variable:

    #### Linux
    ```
    export DB_CERT='certs/client.pfx'
    ```

    #### Windows/PowerShell
    ```
    $env:DB_CERT="certs/client.pfx"
    ```

