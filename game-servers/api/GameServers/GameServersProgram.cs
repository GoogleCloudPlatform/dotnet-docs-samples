using CommandLine;

namespace GoogleCloudSamples
{
    public class BaseOptions
    {
        [Value(0, HelpText = "Your project ID")]
        public string ProjectID { get; set; }

        [Value(1, HelpText = "Location name")]
        public string Location { get; set; }
    }

    public class GameServersProgram
    {
        public static int Main(string[] args)
        {
            var verbMap = new VerbMap<object>();
            Realms.RegisterCommands(verbMap);
            return (int)verbMap.Run(args);
        }
    }
}
