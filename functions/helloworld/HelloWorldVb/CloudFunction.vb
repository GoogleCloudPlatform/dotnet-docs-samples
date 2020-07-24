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

' [START functions_helloworld_get]
Imports Google.Cloud.Functions.Framework
Imports Microsoft.AspNetCore.Http

Public Class CloudFunction
    Implements IHttpFunction

    Public Async Function HandleAsync(context As HttpContext) As Task Implements IHttpFunction.HandleAsync
        Await context.Response.WriteAsync("Hello World!")
    End Function
End Class
' [END functions_helloworld_get]
