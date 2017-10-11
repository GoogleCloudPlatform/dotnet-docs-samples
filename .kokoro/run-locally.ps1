Push-Location
try {
    # Build the parent docker image.
    Set-Location ./docker
    docker build -t gcr.io/cloud-devrel-kokoro-resources/dotnet .
} finally {
    Pop-Location
}
Push-Location
try {
    # Copy secrets from google cloud storage to a local directory
    Set-Location ./local-docker
    gsutil cp -r gs://cloud-devrel-kokoro-resources/dotnet-docs-samples/ .

    # Build the local docker image.
    docker build --build-arg KOKORO_GFILE_DIR=dotnet-docs-samples -t dotnet-docs-samples/kokoro .
} finally {
    Pop-Location
}