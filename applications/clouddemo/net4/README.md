# CloudDemo.Mvc

`CloudDemo.Mvc` is a lightweight sample application that uses 
[ASP.NET MVC 5](https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/getting-started)
on .NET Framework 4.x. 

For further details on how you can deploy the sample application, see
[Creating a CI/CD pipeline with Azure Pipelines and Google Kubernetes Engine](https://cloud.google.com/solutions/creating-cicd-pipeline-vsts-kubernetes-engine).

## Prerequisites

To build the application, you'll need

* [nuget](https://dist.nuget.org/win-x86-commandline/latest/nuget.exe)
* Visual Studio 2019 _or_ the Windows SDK with .NET web development tools installed
* Docker (only required if you plan to deploy the application in a container)

### Build a Windows Docker image

1. Open a developer command prompt 
1. Switch to the `net4` directory:

    `cd net4`

1. Install dependencies:

    `nuget.exe restore`

1. Build the solution and publish the result to a folder:

    `msbuild /t:Clean,Build CloudDemo.Mvc\CloudDemo.Mvc.csproj /p:Configuration=Release   /p:DeployOnBuild=true /p:PublishProfile=FolderProfile`

1. Build a Docker image, using the binaries published to the output folder:

    `docker build -t clouddemo-net4 .`

## Run the container locally

1. Run the container:

    `docker run --rm -it -p 8000:80 clouddemo-net4`

   Notice that Windows event logs and IIS logs are printed to the console.

1. Point your web browser to http://localhost:8000/ and verify that the application is 
   working properly.

## Publish the Docker image

1. Tag and push the image:

    `docker tag clouddemo-net4 gcr.io/[my-project-id]/clouddemo-net4`

1. Push the image to Container Registry:

    `docker push gcr.io/[my-project-id]/clouddemo-net4`

## Deploy to Kubernetes Engine

1. Create a Kubernetes deployment that uses the Docker image:

    kubectl apply -f deployment.yaml`