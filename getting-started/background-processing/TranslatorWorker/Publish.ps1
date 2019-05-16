# Build the application locally.
dotnet publish -c Release
# Use Google Cloud Build to build the container and publish to Google
# Container Registry. 
Copy-Item Dockerfile bin/Release/netcoreapp2.2/publish
$projectId = gcloud config get-value project
gcloud builds submit --tag gcr.io/$projectId/translator-worker bin/Release/netcoreapp2.2/publish
@"
Deploy to your Kubernetes cluster with the following commmand:
kubectl run translator-worker --image=gcr.io/$projectId/translator-worker --replicas=2
"@ | Write-Host