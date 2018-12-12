using System;
using CommandLine;

namespace GoogleCloudSamples
{
    public class ProductSearch
    {
        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            ProductSetManagement.RegisterCommands(verbMap);
            ProductManagement.RegisterCommands(verbMap);
            ProductInProductSetManagement.RegisterCommands(verbMap);
            verbMap.NotParsedFunc = (err) => 1;
            return (int)verbMap.Run(args);
        }
    }
}
