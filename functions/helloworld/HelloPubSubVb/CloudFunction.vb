' Copyright 2020, Google LLC
'
' Licensed under the Apache License, Version 2.0 (the "License");
' you may not use this file except in compliance with the License.
' You may obtain a copy of the License at
'
'     https://www.apache.org/licenses/LICENSE-2.0
'
' Unless required by applicable law or agreed to in writing, software
' distributed under the License is distributed on an "AS IS" BASIS,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the License for the specific language governing permissions and
' limitations under the License.

' [START functions_helloworld_pubsub]
Imports System.Threading
Imports CloudNative.CloudEvents
Imports Google.Cloud.Functions.Framework
Imports Google.Events.Protobuf.Cloud.PubSub.V1
Imports Microsoft.Extensions.Logging

Public Class CloudFunction
    Implements ICloudEventFunction(Of MessagePublishedData)

    Private _logger As ILogger

    Public Sub New(ByVal logger As ILogger(Of CloudFunction))
        _logger = logger
    End Sub


    Public Function HandleAsync(cloudEvent As CloudEvent, data As MessagePublishedData, cancellationToken As CancellationToken) As Task _
        Implements ICloudEventFunction(Of MessagePublishedData).HandleAsync

        Dim nameFromMessage = data.Message?.TextData
        Dim name = If(String.IsNullOrEmpty(nameFromMessage), "world", nameFromMessage)
        _logger.LogInformation("Hello {name}", name)

        Return Task.CompletedTask
    End Function
End Class
' [END functions_helloworld_pubsub]