// Copyright 2020 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

// [START functions_imagemagick_setup]
using CloudNative.CloudEvents;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Google.Cloud.Storage.V1;
using Google.Cloud.Vision.V1;
using Google.Events.Protobuf.Cloud.Storage.V1;
using Grpc.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ImageMagick;

// Dependency injection configuration, executed during server startup.
public class Startup : FunctionsStartup
{
    public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
        services
            .AddSingleton(ImageAnnotatorClient.Create())
            .AddSingleton(StorageClient.Create());
}

[FunctionsStartup(typeof(Startup))]
public class Function : ICloudEventFunction<StorageObjectData>
{
    /// <summary>
    /// The bucket to store blurred images in. An alternative to using environment variables here would be to
    /// fetch it from IConfiguration.
    /// </summary>
    private static readonly string s_blurredBucketName = Environment.GetEnvironmentVariable("BLURRED_BUCKET_NAME");

    private readonly ImageAnnotatorClient _visionClient;
    private readonly StorageClient _storageClient;
    private readonly ILogger _logger;

    public Function(ImageAnnotatorClient visionClient, StorageClient storageClient, ILogger<Function> logger) =>
        (_visionClient, _storageClient, _logger) = (visionClient, storageClient, logger);

    // [END functions_imagemagick_setup]

    // [START functions_imagemagick_analyze]
    public async Task HandleAsync(CloudEvent cloudEvent, StorageObjectData data, CancellationToken cancellationToken)
    {
        // Validate parameters
        if (data.Bucket is null || data.Name is null)
        {
            _logger.LogError("Malformed GCS event.");
            return;
        }

        // Construct URI to GCS bucket and file.
        string gcsUri = $"gs://{data.Bucket}/{data.Name}";
        _logger.LogInformation("Analyzing {uri}", gcsUri);

        // Perform safe search detection using the Vision API.
        Image image = Image.FromUri(gcsUri);
        SafeSearchAnnotation annotation;
        try
        {
            annotation = await _visionClient.DetectSafeSearchAsync(image);
        }
        // If the call to the Vision API fails, log the error but let the function complete normally.
        // If the exceptions weren't caught (and just propagated) the event would be retried.
        // See the "Best Practices" section in the documentation for more details about retry.
        catch (AnnotateImageException e)
        {
            _logger.LogError(e, "Vision API reported an error while performing safe search detection");
            return;
        }
        catch (RpcException e)
        {
            _logger.LogError(e, "Error communicating with the Vision API");
            return;
        }

        if (annotation.Adult == Likelihood.VeryLikely || annotation.Violence == Likelihood.VeryLikely)
        {
            _logger.LogInformation("Detected {uri} as inappropriate.", gcsUri);
            await BlurImageAsync(data, cancellationToken);
        }
        else
        {
            _logger.LogInformation("Detected {uri} as OK.", gcsUri);
        }
    }
    // [END functions_imagemagick_analyze]

    // [START functions_imagemagick_blur]
    /// <summary>
    /// Downloads the Storage object specified by <paramref name="data"/>,
    /// blurs it using ImageMagick, and uploads it to the "blurred" bucket.
    /// </summary>
    private async Task BlurImageAsync(StorageObjectData data, CancellationToken cancellationToken)
    {
        // Download image
        string originalImageFile = Path.GetTempFileName();
        using (Stream output = File.Create(originalImageFile))
        {
            await _storageClient.DownloadObjectAsync(data.Bucket, data.Name, output, cancellationToken: cancellationToken);
        }

        // Construct the ImageMagick command
        string blurredImageFile = Path.GetTempFileName();
        // Command-line arguments for ImageMagick.
        // Paths are wrapped in quotes in case they contain spaces.
        string arguments = $"\"{originalImageFile}\" -blur 0x8, \"{blurredImageFile}\"";

        // Run the ImageMagick command line tool ("convert").
        Process process = Process.Start("convert", arguments);
        // Process doesn't expose a way of asynchronously waiting for completion.
        // See https://stackoverflow.com/questions/470256 for examples of how
        // this can be achieved using events, but for the sake of brevity,
        // this sample just waits synchronously.
        process.WaitForExit();

        // If ImageMagick failed, log the error but complete normally to avoid retrying.
        if (process.ExitCode != 0)
        {
            _logger.LogError("ImageMagick exited with code {exitCode}", process.ExitCode);
            return;
        }

        // Upload image to blurred bucket.
        using (Stream input = File.OpenRead(blurredImageFile))
        {
            await _storageClient.UploadObjectAsync(
                s_blurredBucketName, data.Name, data.ContentType, input, cancellationToken: cancellationToken);
        }

        string uri = $"gs://{s_blurredBucketName}/{data.Name}";
        _logger.LogInformation("Blurred image uploaded to: {uri}", uri);

        // Remove images from the file system.
        File.Delete(originalImageFile);
        File.Delete(blurredImageFile);
    }
    // [END functions_imagemagick_blur]
    // [START functions_imagemagick_setup]
}
// [END functions_imagemagick_setup]
