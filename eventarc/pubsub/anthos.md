# Events for Cloud Run on Anthos – Pub/Sub tutorial

This sample shows how to create a service that processes Pub/Sub events on Anthos.
It is assumed that you already created a GKE cluster with Events for Cloud Run
already installed.

## Quickstart

Set cluster name, location and platform:

```sh
gcloud config set run/cluster events-cluster
gcloud config set run/cluster_location europe-west1-b
gcloud config set run/platform gke
```

Create a Cloud Pub/Sub topic:

```sh
gcloud pubsub topics create my-topic
```

Create a Cloud Pub/Sub trigger:

```sh
gcloud beta events triggers create pubsub-trigger \
  --target-service cloudrun-events-pubsub \
  --type google.cloud.pubsub.topic.v1.messagePublished \
  --parameters topic=my-topic
```

Deploy your Cloud Run service:

```sh
gcloud builds submit \
 --tag gcr.io/$(gcloud config get-value project)/cloudrun-events-pubsub
gcloud run deploy cloudrun-events-pubsub \
 --image gcr.io/$(gcloud config get-value project)/cloudrun-events-pubsub
```

## Test

Test your Cloud Run service by publishing a message to the topic:

```sh
gcloud pubsub topics publish ${MY_TOPIC} --message="John Doe"
```

You may observe the Run service receiving an event in Cloud Logging.
