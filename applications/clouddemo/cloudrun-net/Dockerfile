#
# Copyright 2025 Google LLC
#
# Licensed to the Apache Software Foundation (ASF) under one
# or more contributor license agreements.  See the NOTICE file
# distributed with this work for additional information
# regarding copyright ownership.  The ASF licenses this file
# to you under the Apache License, Version 2.0 (the
# "License"); you may not use this file except in compliance
# with the License.  You may obtain a copy of the License at
#
#   http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing,
# software distributed under the License is distributed on an
# "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
# KIND, either express or implied.  See the License for the
# specific language governing permissions and limitations
# under the License.
#

#------------------------------------------------------------------------------
# Build the web application
#------------------------------------------------------------------------------

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY helloworld-csharp/*.csproj ./
RUN dotnet restore

#------------------------------------------------------------------------------
# Create publishing artifacts.
#------------------------------------------------------------------------------

# copy everything else and build app
COPY helloworld-csharp/. ./
RUN dotnet publish ./helloworld-csharp.csproj -c release -o /app --no-restore

#------------------------------------------------------------------------------
# Copy publishing artifacts.
#------------------------------------------------------------------------------

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 8080

WORKDIR /app
COPY --from=build /app ./

#------------------------------------------------------------------------------
# Run application in Kestrel.
#------------------------------------------------------------------------------

ENTRYPOINT ["dotnet", "helloworld-csharp.dll"]
