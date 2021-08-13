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

casper.test.begin('Fill form with cat image.', 4, function suite(test) {
    casper.start(host + '/', function (response) {
        console.log('Starting ' + host + '/');
        test.assertEquals(response.status, 200);
    });

    casper.thenOpen(host + '/Home/Naughty', function (response) {
        test.assertEquals(response.status, 200);
        this.fill('form', {
            'files': ['cat.jpg']
        }, false);
        console.log('Filled form.');
    });

    casper.thenClick('#submit', function (response) {
        console.log('Submitted.  New location is ' + this.getCurrentUrl());
        test.assertEquals(response.status, 200);
        test.assertEquals(this.fetchText('#adult'),
            'Adult: VeryUnlikely');
    });

    casper.run(function () {
        test.done();
    });
});