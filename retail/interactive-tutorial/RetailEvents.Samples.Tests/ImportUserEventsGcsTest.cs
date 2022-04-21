﻿// Copyright 2021 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Xunit;

namespace RetailEvents.Samples.Tests
{
    public class ImportUserEventsGcsTest
    {
        [Fact]
        public void TestImportUserEventsGcs()
        {
            string createdBucketName = null;
            string projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

            try
            {
                UpdateUserEventsJson.PerformUpdateEventsTimeStamp();
                EventsDeleteGcsBucket.PerformDeletionOfEventsBucketName(createdBucketName);
                createdBucketName = EventsCreateGcsBucket.PerformCreationOfEventsGcsBucket();

                int expectedSuccessfullyImportedEvents = 4;
                int expectedFailures = 0;

                var sample = new ImportUserEventsGcsSample();
                var result = sample.ImportUserEventsFromGcs(projectId, createdBucketName);

                Assert.Equal(expectedSuccessfullyImportedEvents, result.Metadata.SuccessCount);
                Assert.Equal(expectedFailures, result.Metadata.FailureCount);
            }
            finally
            {
                EventsDeleteGcsBucket.PerformDeletionOfEventsBucketName(createdBucketName);
            }
        }
    }
}