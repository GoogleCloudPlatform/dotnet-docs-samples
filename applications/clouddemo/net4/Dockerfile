#
# Copyright 2020 Google LLC
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

FROM mcr.microsoft.com/windows/servercore/iis:windowsservercore-ltsc2019
EXPOSE 80
SHELL ["powershell", "-command"]

#------------------------------------------------------------------------------
# Add LogMonitor so that IIS and Windows logs are emitted to STDOUT and can be 
# picked up by Docker/Kubernetes.
#
# See https://github.com/microsoft/windows-container-tools/wiki/Authoring-a-Config-File
# for details.
#------------------------------------------------------------------------------

ADD https://github.com/microsoft/windows-container-tools/releases/download/v1.1/LogMonitor.exe LogMonitor/
ADD LogMonitorConfig.json LogMonitor/

#------------------------------------------------------------------------------
# Copy publishing artifacts to webroot.
#------------------------------------------------------------------------------

ADD CloudDemo.Mvc/bin/Release/PublishOutput/ c:/inetpub/wwwroot/

#------------------------------------------------------------------------------
# Configure IIS using the helper functions from deployment.ps1.
#------------------------------------------------------------------------------

ADD deployment.ps1 /
RUN . /deployment.ps1; \
	Install-Iis; \
	Register-WebApplication -AppName "CloudDemo"; \
	Remove-Item /deployment.ps1

#------------------------------------------------------------------------------
# Run IIS, wrapped by LogMonitor.
#------------------------------------------------------------------------------

ENTRYPOINT ["C:\\LogMonitor\\LogMonitor.exe", "C:\\ServiceMonitor.exe", "w3svc"]