# Eventarc – Pub/Sub

This sample shows how to create a service that processes Pub/Sub events.

## Quickstart

Deploy your Cloud Run service:

```sh
gcloud builds submit \
 --tag gcr.io/$(gcloud config get-value project)/eventarc-pubsub
gcloud run deploy eventarc-pubsub \
 --image gcr.io/$(gcloud config get-value project)/eventarc-pubsub
```

Create a Cloud Pub/Sub trigger:

```sh
gcloud eventarc triggers create pubsub-trigger \
  --destination-run-service eventarc-pubsub \
  --event-filters "type=google.cloud.pubsub.topic.v1.messagePublished"
```

## Test

Test your Cloud Run service by publishing a message to the topic:

```sh
TOPIC=$(gcloud eventarc triggers describe pubsub-trigger \
--format="value(transport.pubsub.topic)")

echo "Listening to events on topic: $TOPIC"

gcloud pubsub topics publish $TOPIC --message="Events"
```

You may observe the Run service receiving an event in Cloud Logging:

```sh
gcloud logging read "resource.type=cloud_run_revision AND resource.labels.service_name=eventarc-pubsub" --limit 10
```
