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

// 1.0 style test script not using the `casperjs test` subcommand
var casper = require('casper').create();
var host = casper.cli.args[0];


casper.start(host + '/', function (response) {
    console.log('Starting ' + host + '/');
    this.test.assertSelectorHasText('H1', 'Google AppEngine Pubsub Sample');
    this.fill('#MessageForm', {
        'Message': 'blobfish 1',
    }, false);
    console.log("Filled form.");
});

// Submit 10 messages to make sure that at least one message arrives
// at each of the 2 running instances.
casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.test.assertSelectorHasText('#PublishedMessage', 'blobfish 1');
    this.fill('#MessageForm', {
        'Message': 'blobfish 2',
    }, false);
});

casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.fill('#MessageForm', {
        'Message': 'blobfish 3',
    }, false);
});

casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.fill('#MessageForm', {
        'Message': 'blobfish 4',
    }, false);
});

casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.fill('#MessageForm', {
        'Message': 'blobfish 5',
    }, false);
});

casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.fill('#MessageForm', {
        'Message': 'blobfish 6',
    }, false);
});

casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.fill('#MessageForm', {
        'Message': 'blobfish 7',
    }, false);
});

casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.fill('#MessageForm', {
        'Message': 'blobfish 8',
    }, false);
});

casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.fill('#MessageForm', {
        'Message': 'blobfish 9',
    }, false);
});

casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.fill('#MessageForm', {
        'Message': 'blobfish 10',
    }, false);
});

casper.thenClick('#Submit', function (response) {
    console.log('Submitted form.');
    this.test.assertEquals(200, response.status);
    this.fill('#MessageForm', {
        'Message': 'cattfish 3',
    }, false);
    this.test.assertSelectorHasText('body', 'blobfish');
});

casper.run(function () {
    this.test.done();
    this.test.renderResults(true);
});
