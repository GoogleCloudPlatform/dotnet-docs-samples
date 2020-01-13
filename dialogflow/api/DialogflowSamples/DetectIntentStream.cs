using System;
using CommandLine;

namespace GoogleCloudSamples
{
    public class DetectIntentStream
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DetectIntentFromStreamOptions opts) =>
                     DetectIntentFromStream(opts.ProjectId, opts.SessionId, opts.FilePath));
        }

        [Verb("detect-intent:streams", HelpText="Detect intent from stream")]
        public class DetectIntentFromStreamOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "file", HelpText = "Path to the audio file", Required = true)]
            public string FilePath { get; set; }
        }

        public static int DetectIntentFromStream(string projectId,
                                                 string sessionId,
                                                 string filePath)
        {
            Console.WriteLine(projectId);
            return 0;
        }
    }
}
