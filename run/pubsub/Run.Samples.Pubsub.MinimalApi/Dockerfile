# Copyright 2022 Google Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

# [START cloudrun_pubsub_dockerfile_csharp]
# Build in SDK base image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -r linux-x64 --no-self-contained -p:PublishReadyToRun=true -c Release -o out

# Copy to runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

# Port passed in by Cloud Run via environment variable PORT.  Default 8080.
ENV PORT=8080

ENTRYPOINT ["dotnet", "Run.Samples.Pubsub.MinimalApi.dll"]
# [END cloudrun_pubsub_dockerfile_csharp]