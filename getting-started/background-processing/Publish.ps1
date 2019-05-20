# Build the application locally.
dotnet publish -c Release

# Collect some details about the project that we'll need later.
$projectId = gcloud config get-value project
$projectNumber = gcloud projects describe $projectId --format="get(projectNumber)"

# Use Google Cloud Build to build the worker's container and publish to Google
# Container Registry. 
gcloud builds submit --tag gcr.io/$projectId/translate-worker `
    TranslateWorker/bin/Release/netcoreapp2.2/publish

# Run the container with Google Cloud Run.
$region = "us-central1"
gcloud beta run deploy translate-worker --region $region `
    --image gcr.io/$projectId/translate-worker --no-allow-unauthenticated
$url = gcloud beta run services describe translate-worker `
    --region $region --format="get(status.address.hostname)"

# Enable your project to create pubsub authentication tokens.
gcloud projects add-iam-policy-binding $projectId `
     --member=serviceAccount:service-$projectNumber@gcp-sa-pubsub.iam.gserviceaccount.com `
     --role=roles/iam.serviceAccountTokenCreator

# Create a service account to represent the Cloud Pub/Sub subscription identity.
$serviceAccountExists = gcloud iam service-accounts describe `
    cloud-run-pubsub-invoker@$projectId.iam.gserviceaccount.com 2> $null
if (-not $serviceAccountExists) {
    gcloud iam service-accounts create cloud-run-pubsub-invoker `
        --display-name "Cloud Run Pub/Sub Invoker"
}

# For Cloud Run, give this service account permission to invoke your
# translate-worker service.
gcloud beta run services add-iam-policy-binding translate-worker `
     --member=serviceAccount:cloud-run-pubsub-invoker@$projectId.iam.gserviceaccount.com `
     --role=roles/run.invoker

# Create a pubsub topic and subscription, if they don't already exist.
$topicExists = gcloud pubsub topics describe translate-requests 2> $null 
if (-not $topicExists) {
    gcloud pubsub topics create translate-requests
}
$subscriptionExists = gcloud pubsub subscriptions describe translate-requests 2> $null
if ($subscriptionExists) {
    gcloud beta pubsub subscriptions modify-push-config translate-requests `
        --push-endpoint $url/api/translate `
        --push-auth-service-account cloud-run-pubsub-invoker@$projectId.iam.gserviceaccount.com
} else {
    gcloud beta pubsub subscriptions create translate-requests `
        --topic translate-requests --push-endpoint $url/api/translate `
        --push-auth-service-account cloud-run-pubsub-invoker@$projectId.iam.gserviceaccount.com
}
