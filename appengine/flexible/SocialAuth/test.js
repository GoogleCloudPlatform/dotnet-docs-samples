// Copyright(c) 2017 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

var system = require('system');
var host = system.env['CASPERJS11_URL'];

casper.test.begin('Home page redirects.', 1, function suite(test) {
    var redirectURLs = [],
        doLog = false;
    // Observing redirects with casperjs is difficult.  See:
    // https://stackoverflow.com/questions/27021176/how-to-prevent-redirects-in-casperjs
    casper.on("resource.requested", function (requestData, networkRequest) {
        if (doLog) console.log('Request (#' + requestData.id + '): ' + JSON.stringify(requestData) + "\n");
        if (redirectURLs.indexOf(requestData.url) !== -1) {
            // this is a redirect url
            networkRequest.abort();
        }
    });

    casper.on("resource.received", function (response) {
        if (doLog) console.log('Response (#' + response.id + ', stage "' + response.stage + '"): ' + JSON.stringify(response) + "\n");
        if (response.status === 302) { // use your status here
            redirectURLs.push(response.redirectURL);
        }
    });

    casper.start(host + '/', function (response) {
        console.log('Starting ' + host + '/');
        if (doLog) console.log(JSON.stringify(response, null, 4));
        test.assertEquals(redirectURLs[0], "https://localhost:44393/");
    });

    casper.run(function () {
        test.done();
    });
});
