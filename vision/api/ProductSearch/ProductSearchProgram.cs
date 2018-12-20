using CommandLine;

namespace GoogleCloudSamples
{
    class BaseOptions
    {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }

        [Value(1, HelpText = "Region name")]
        public string ComputeRegion { get; set; }
    }

    public class ProductSearchProgram
    {
        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            ProductSetManagement.RegisterCommands(verbMap);
            ProductManagement.RegisterCommands(verbMap);
            ProductInProductSetManagement.RegisterCommands(verbMap);
            ReferenceImageManagement.RegisterCommands(verbMap);
            ImportProductSets.RegisterCommands(verbMap);
            ProductSearch.RegisterCommands(verbMap);
            verbMap.NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);
        }
    }
}
