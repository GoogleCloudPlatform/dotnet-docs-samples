# C# Google Cloud Tasks Samples

This sample demonstrates how to use the [Google Cloud Tasks](https://cloud.google.com/cloud-tasks/)
client library.

The `api/TasksSample` project contains simple programs:
- `CreateHttpTask.cs` constructs a task with an HTTP target and pushes it
to your queue.
- `CreateTask.cs` constructs a task with an App Engine target and pushes it
to your queue.

`appengine` is a task handler. This example app that has an endpoint to
receive App Engine task attempts.

## Setup

Before you can run or deploy the sample, you need to do the following:

1.  Enable the Cloud Tasks API in the [Google Cloud Console](https://console.cloud.google.com/apis/api/tasks.googleapis.com).
1.  Set up [Google Application Credentials](https://cloud.google.com/docs/authentication/getting-started).

## Creating a queue

To create a queue using the Cloud SDK, use the following gcloud command:

    gcloud tasks queues create <QUEUE_NAME>

## Run the Sample Using the Command Line

Set environment variables:

First, your project ID:

    export GOOGLE_PROJECT_ID=my-project-id

Then the queue ID, as specified at queue creation time. Queue IDs already
created can be listed with `gcloud tasks queues list`.

    export GCP_QUEUE=<QUEUE_NAME>

And finally the location ID, which can be discovered with
`gcloud tasks queues describe $GCP_QUEUE`, with the location embedded in
the "name" value (for instance, if the name is
"projects/my-project/locations/us-central1/queues/my-queue", then the
location is "us-central1").

    export LOCATION_ID=us-central1

Move into the Tasks Sample folder:

    cd ../api/TasksSample

Install dependencies:

    dotnet restore

Run the tests to run the samples:

    dotnet test


## Using an App Engine Task Handler

Move into the App Engine folder:

  cd ../../appengine

Deploy the App Engine app with `gcloud`:

    ```psm1
    PS > dotnet restore
    PS > dotnet publish
    PS > gcloud beta app deploy .\bin\Debug\netcoreapp2.1\publish\app.yaml
    ```

Verify the index page is serving:

    gcloud app browse

The App Engine app serves as a target for the push requests. It has an
endpoint `/log_payload` that reads the payload (i.e., the request body) of the
HTTP POST request and logs it. The log output can be viewed with:

    gcloud app logs read


[readme]: https://github.com/GoogleCloudPlatform/dotnet-docs-samples/blob/master/appengine/flexible/README.md
[appengine]: https://cloud.google.com/appengine/docs/flexible/dotnet
