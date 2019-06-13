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

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chat").build();

document.getElementById("chat-send").disabled = true;

connection.on("ReceiveMessage", function (message) {
    var li = document.createElement("li");
    li.textContent = "Message received";
    document.getElementById("chat-status").appendChild(li);

    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    li = document.createElement("li");
    li.textContent = msg;
    document.getElementById("chat-response").appendChild(li);
});

connection.start().then(function() {
    document.getElementById("chat-send").disabled = false;

    var li = document.createElement("li")
    li.textContent = "Connected";
    document.getElementById("chat-status").appendChild(li);
}).catch(function (err) {
    var li = document.createElement("li")
    li.textContent = "Not connected";
    document.getElementById("chat-status").appendChild(li);

    return console.error(err.toString());
});

document.getElementById("chat-send").addEventListener("click", function (event) {
    var message = document.getElementById("chat-text").value;
    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});
