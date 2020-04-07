using CommandLine;
using Google.Api.Gax.ResourceNames;
using Google.Cloud.Vision.V1;
using Google.LongRunning;
using System;

namespace GoogleCloudSamples
{
    [Verb("import_product_set", HelpText = "Import a product set from GCS")]
    class ImportProductSetOptions : BaseOptions
    {
        [Value(2, HelpText = "Google Cloud Storage URI. Target files must be in Product Search CSV format.")]
        public string GcsUri { get; set; }
    }

    public class ImportProductSets
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((ImportProductSetOptions opts) => ImportProductSet(opts));
        }

        // [START vision_product_search_import_product_images]
        private static int ImportProductSet(ImportProductSetOptions opts)
        {
            var client = ProductSearchClient.Create();
            var request = new ImportProductSetsRequest
            {
                // A resource that represents Google Cloud Platform location.
                ParentAsLocationName = new LocationName(opts.ProjectID,
                                                        opts.ComputeRegion),

                // Set the input configuration along with Google Cloud Storage URI
                InputConfig = new ImportProductSetsInputConfig
                {
                    GcsSource = new ImportProductSetsGcsSource
                    {
                        CsvFileUri = opts.GcsUri
                    }
                }
            };
            var response = client.ImportProductSets(request);

            // Synchronous check of operation status
            var completedResponse = response.PollUntilCompleted();

            if (completedResponse.IsCompleted)
            {
                var result = completedResponse.Result;

                foreach (var status in result.Statuses)
                {
                    // Check status of reference image.
                    // `0` is the code for OK in google.rpc.Code.
                    if (status.Code == 0)
                    {
                        Console.WriteLine(result.ReferenceImages);
                    }
                    else
                    {
                        Console.WriteLine("No reference images.");
                    }
                }
            }

            return 0;
        }
        // [END vision_product_search_import_product_images]
    }
}
