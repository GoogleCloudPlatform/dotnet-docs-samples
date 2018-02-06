/*
 * Copyright (c) 2017 Google Inc.
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

var myApp = angular.module('myApp', []);
myApp.controller('mainCtrl', ['$scope', '$http', function ($scope, $http) {
    $scope.logCount = 3;
    $scope.doLogging = function () {
        var url = '/LoggingSample/Log?count=' + $scope.logCount;

        // HTTP request.
        $http.get(url)
            .then(function (response) {
                //First function handles success
                $scope.submitResult = response.data;
            }, function (response) {
                //Second function handles error
                $scope.submitResult = "Something went wrong";
            });
    }
}]);