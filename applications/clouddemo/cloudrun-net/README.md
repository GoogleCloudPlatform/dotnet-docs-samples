# Cloud Run - .NET Demo

This walkthrough creates a basic [ASP.NET MVC Core](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc) web application.

## Prerequisites

To create and build the application, you'll need

* [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
* [Google Cloud account and project](https://cloud.google.com/resource-manager/docs/creating-managing-projects)

## Build a Linux Docker image
1. Open a developer command prompt on your machine. Alternatively, open [Cloud Shell](https://cloud.google.com/shell/docs/using-cloud-shell).
1. Build a Docker image, using the binaries published to the output folder:

    `docker build -t cloudrun-net .`

## Run the container locally
1. Run the container:

    `docker run --rm -it -p 8080:8080 cloudrun-net`

1. Point your web browser to http://localhost:8080/ and verify that the application is
   working properly.

## Push the container image to Artifact Registry

1. If you haven't done so already, [create an Artifact Registry Docker repository](https://cloud.google.com/artifact-registry/docs/docker/store-docker-container-images#create).
1. Retrieve the URI of your Artifact Registry Docker repository:
    <pre>gcloud artifacts repositories describe <b>REPO-NAME</b> \
        --location=<b>LOCATION</b> \
        --format="value(registryUri)"</pre>
    Replace:
    - `REPO-NAME` with the name of your Docker repository
    - `LOCATION` with the location (region / multi-region) where the Docker repository was created

1. Create a environment variable to hold the Docker image's URI
    <pre>export IMAGE_URI=<b>REGISTRY-URI</b>/cloudrun-net</pre>
    Replace `REGISTRY-URI` with the URI you retrieved in the previous step.
1. Tag the image:
    ```bash
    docker tag cloudrun-net $IMAGE_URI
    ```
1. Push the image to Artifact Registry:
    ```bash
    docker push $IMAGE_URI
    ```

## Deploy to Cloud Run
1. Deploy the container image to Cloud Run
    <pre>gcloud run deploy cloudrun-net \
        --region=<b>REGION</b> \
        --image=$IMAGE_URI \
        --allow-unauthenticated</pre>
    Replace `REGION` with the region you want to deploy the Cloud Run service to.

    If you are prompted to enable additional APIs on the project, for example, the Cloud Run API, respond by pressing y.

1. Point your web browser to the Service URL returned by the command and verify that the   application is working properly.
