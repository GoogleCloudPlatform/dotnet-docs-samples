# Build the application locally.
dotnet publish -c Release
# Use Google Cloud Build to build the container and publish to Google
# Container Registry. 
Copy-Item Dockerfile bin/Release/netcoreapp2.2/publish
$projectId = gcloud config get-value project
gcloud builds submit --tag gcr.io/$projectId/translator-worker bin/Release/netcoreapp2.2/publish
gcloud beta run deploy translator-worker --image gcr.io/$projectId/translator-worker --no-allow-unauthenticated