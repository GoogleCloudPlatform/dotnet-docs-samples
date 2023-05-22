/*
 * Copyright (c) 2019 Google LLC.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

using System.Net;
using System.Net.WebSockets;

public class Program
{
    private WebApplication App { get; set; }

    private Program(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        App = builder.Build();

        // [START gae_flex_dotnet_websocket]
        App.UseDefaultFiles();
        App.UseStaticFiles();
        App.UseWebSockets();
        App.MapGet("/chat", async (HttpContext context) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                await Echo(context, webSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
            }
        });
        // [END gae_flex_dotnet_websocket]
    }

    private async Task Echo(HttpContext context, WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        while (!result.CloseStatus.HasValue)
        {
            await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
        await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
    }

    public static void Main(string[] args)
    {
        new Program(args).App.Run();
    }
}

