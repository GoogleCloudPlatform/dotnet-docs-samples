# Build the application locally.
dotnet publish -c Release

# Use Google Cloud Build to build the worker's container and publish to Google
# Container Registry. 
$projectId = gcloud config get-value project
gcloud builds submit --tag gcr.io/$projectId/translate-worker `
    TranslateWorker/bin/Release/netcoreapp2.2/publish

# Run the container with Google Cloud Run.
$region = "us-central1"
gcloud beta run deploy translate-worker --region $region `
    --image gcr.io/$projectId/translate-worker --no-allow-unauthenticated
$url = gcloud beta run services describe translate-worker `
    --region $region --format="get(status.address.hostname)"

# Create a pubsub topic and subscription, if they don't already exist.
$topicExists = gcloud pubsub topics describe translate-requests 2> $null 
if (-not $topicExists) {
    gcloud pubsub topics create translate-requests
}
$subscriptionExists = gcloud pubsub subscriptions describe translate-requests 2> $null
if ($subscriptionExists) {
    gcloud pubsub subscriptions modify-push-config translate-requests `
        --push-endpoint $url/api/translate
} else {
    gcloud pubsub subscriptions create translate-requests `
        --topic translate-requests --push-endpoint $url/api/translate
}
