/*
 * Copyright 2021 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Google;
using System;
using System.Net;
using System.IO;
using Xunit;
using Xunit.Sdk;
using GoogleCloudSamples;
using Google.Cloud.Video.Transcoder.V1;
using System.Collections.Generic;

[CollectionDefinition(nameof(TranscoderFixture))]
public class TranscoderFixture : IDisposable, ICollectionFixture<TranscoderFixture>
{
    public string ProjectId { get; }
    public string ProjectNumber { get; }
    public string Location { get; } = "us-central1";

    private readonly RandomBucketFixture _bucketFixture;

    public string BucketName { get; }
    public string TestDataPath { get; } = Path.GetFullPath("../../../testdata/");
    public string TestVideoFileName { get; } = "ChromeCast.mp4";
    public string TestConcatVideo1FileName { get; } = "ForBiggerEscapes.mp4";
    public string TestConcatVideo2FileName { get; } = "ForBiggerJoyrides.mp4";
    public string TestOverlayImageFileName { get; } = "overlay.jpg";
    public string InputUri { get; }
    public string InputConcat1Uri { get; }
    public string InputConcat2Uri { get; }
    public string OverlayImageUri { get; }
    public Job.Types.ProcessingState JobStateSucceeded { get; } = Job.Types.ProcessingState.Succeeded;
    public List<string> JobIds { get; } = new List<string>();
    public List<string> JobTemplateIds { get; } = new List<string>();
    private readonly DeleteJobSample _deleteJobSample = new DeleteJobSample();
    private readonly DeleteJobTemplateSample _deleteJobTemplateSample = new DeleteJobTemplateSample();

    public RetryRobot JobPoller { get; } = new RetryRobot
    {
        FirstRetryDelayMs = 15000,
        DelayMultiplier = 2,
        MaxTryCount = 20,
        ShouldRetry = ex => ex is XunitException ||
            (ex is GoogleApiException gex &&
                gex.HttpStatusCode == HttpStatusCode.ServiceUnavailable)
    };

    public TranscoderFixture()
    {
        ProjectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");
        if (string.IsNullOrEmpty(ProjectId))
        {
            throw new Exception("missing GOOGLE_PROJECT_ID");
        }

        ProjectNumber = Environment.GetEnvironmentVariable("PROJECT_NUMBER");
        if (string.IsNullOrEmpty(ProjectNumber))
        {
            throw new Exception("missing PROJECT_NUMBER");
        }

        _bucketFixture = new RandomBucketFixture();
        BucketName = _bucketFixture.BucketName;

        var bucketCollector = new BucketCollector(BucketName);
        bucketCollector.CopyToBucket(Path.Combine(TestDataPath, TestVideoFileName), TestVideoFileName);
        bucketCollector.CopyToBucket(Path.Combine(TestDataPath, TestConcatVideo1FileName), TestConcatVideo1FileName);
        bucketCollector.CopyToBucket(Path.Combine(TestDataPath, TestConcatVideo2FileName), TestConcatVideo2FileName);
        bucketCollector.CopyToBucket(Path.Combine(TestDataPath, TestOverlayImageFileName), TestOverlayImageFileName);

        InputUri = $"gs://{BucketName}/{TestVideoFileName}";
        InputConcat1Uri = $"gs://{BucketName}/{TestConcatVideo1FileName}";
        InputConcat2Uri = $"gs://{BucketName}/{TestConcatVideo2FileName}";
        OverlayImageUri = $"gs://{BucketName}/{TestOverlayImageFileName}";
    }

    public void Dispose()
    {
        try
        {
            _bucketFixture.Dispose();
        }
        catch (Exception e)
        {
            Console.WriteLine("Cleanup failed: " + e.ToString());
        }

        foreach (string id in JobIds)
        {
            try
            {
                _deleteJobSample.DeleteJob(ProjectId, Location, id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for job: " + id + " with error: " + e.ToString());
            }
        }

        foreach (string id in JobTemplateIds)
        {
            try
            {
                _deleteJobTemplateSample.DeleteJobTemplate(ProjectId, Location, id);
            }
            catch (Exception e)
            {
                Console.WriteLine("Delete failed for job template: " + id + " with error: " + e.ToString());
            }
        }
    }

    public string RandomId()
    {
        return $"csharp-{System.Guid.NewGuid()}";
    }
}
