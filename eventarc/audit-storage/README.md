# Eventarc – Cloud Storage Events via Audit Logs

This sample shows how to create a service that processes Cloud Storage events.

## Setup

Configure environment variables:

```sh
MY_RUN_SERVICE=gcs-service
MY_RUN_CONTAINER=gcs-container
MY_GCS_BUCKET=$(gcloud config get-value project)-gcs-bucket
```

## Quickstart

Deploy your Cloud Run service:

```sh
gcloud builds submit \
 --tag gcr.io/$(gcloud config get-value project)/${MY_RUN_CONTAINER}
gcloud run deploy ${MY_RUN_SERVICE} \
 --image gcr.io/$(gcloud config get-value project)/${MY_RUN_CONTAINER} \
 --allow-unauthenticated
```

Create a single region Cloud Storage bucket:

```sh
gsutil mb -p $(gcloud config get-value project) \
    -l us-central1 \
    gs://${MY_GCS_BUCKET}
```

Grant the `eventarc.eventReceiver` role to the Compute Engine service account:

```sh
export PROJECT_NUMBER="$(gcloud projects describe $(gcloud config get-value project) --format='value(projectNumber)')"

gcloud projects add-iam-policy-binding $(gcloud config get-value project) \
    --member=serviceAccount:${PROJECT_NUMBER}-compute@developer.gserviceaccount.com \
    --role='roles/eventarc.eventReceiver'
```

Create Cloud Storage trigger:

```sh
gcloud eventarc triggers create my-gcs-trigger \
  --destination-run-service ${MY_RUN_SERVICE} \
  --event-filters type=google.cloud.audit.log.v1.written \
  --event-filters methodName=storage.buckets.update \
  --event-filters serviceName=storage.googleapis.com \
  --event-filters resourceName=projects/_/buckets/"$MY_GCS_BUCKET" \
  --service-account=${PROJECT_NUMBER}-compute@developer.gserviceaccount.com
```

## Test

Test your Cloud Run service by publishing a message to the topic:

```sh
gsutil defstorageclass set NEARLINE gs://${MY_GCS_BUCKET}
```

Observe the Cloud Run service printing upon receiving an event in
Cloud Logging:

```sh
gcloud logging read "resource.type=cloud_run_revision AND \
resource.labels.service_name=${MY_RUN_SERVICE}" --project \
$(gcloud config get-value project) --limit 30 --format 'value(textPayload)'
```
