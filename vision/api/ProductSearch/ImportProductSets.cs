using CommandLine;
using Google.Cloud.Vision.V1;
using Google.LongRunning;
using System;
using System.Linq;

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
                ParentAsLocationName = new LocationName(opts.ProjectID,
                                                        opts.ComputeRegion),
                InputConfig = new ImportProductSetsInputConfig
                {
                    GcsSource = new ImportProductSetsGcsSource
                    {
                        CsvFileUri = opts.GcsUri
                    }
                }

            };
            Operation<ImportProductSetsResponse, BatchOperationMetadata> response = 
                client.ImportProductSets(request);

            Operation<ImportProductSetsResponse, BatchOperationMetadata> completedResponse =
                response.PollUntilCompleted();
            
            if (completedResponse.IsCompleted)
            {
                var result = completedResponse.Result;

                foreach (var status in result.Statuses)
                {
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

            return 1;
        }
        // [END vision_product_search_import_product_images]
    }
}
