using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CommandLine;
using Google.Cloud.Dialogflow.V2;

namespace GoogleCloudSamples
{
    public class DetectIntentStream
    {
        public static void RegisterCommands(VerbMap<object> verbMap)
        {
            verbMap
                .Add((DetectIntentFromStreamOptions opts) =>
                     DetectIntentFromStream(opts.ProjectId, opts.SessionId, opts.FilePath).Result);
        }

        [Verb("detect-intent:streams", HelpText="Detect intent from stream")]
        public class DetectIntentFromStreamOptions : OptionsWithProjectIdAndSessionId
        {
            [Value(0, MetaName = "file", HelpText = "Path to the audio file", Required = true)]
            public string FilePath { get; set; }
        }

        // [START dialogflow_detect_intent_streaming]
        public static async Task<object> DetectIntentFromStream(string projectId,
                                                 string sessionId,
                                                 string filePath)
        {
            var sessionsClient = SessionsClient.Create();
            var sessionName = SessionName.Format(projectId, sessionId);

            // Initialize streaming call, retrieving the stream object
            var streamingDetectIntent = sessionsClient.StreamingDetectIntent();

            // Define a task to process results from the API
            var responseHandlerTask = Task.Run(async () =>
            {
                var responseStream = streamingDetectIntent.ResponseStream;
                while (await responseStream.MoveNext())
                {
                    var response = responseStream.Current;
                    var queryResult = response.QueryResult;

                    if (queryResult != null)
                    {
                        Console.WriteLine($"Query text: {queryResult.QueryText}");
                        if (queryResult.Intent != null)
                        {
                            Console.Write("Intent detected:");
                            Console.WriteLine(queryResult.Intent.DisplayName);
                        }
                    }
                }
            });

            // Instructs the speech recognizer how to process the audio content.
            // Note: hard coding audioEncoding, sampleRateHertz for simplicity.
            // Audio encoding of the audio content sent in the query request.
            var queryInput = new QueryInput
            {
                AudioConfig = new InputAudioConfig
                {
                    AudioEncoding = AudioEncoding.Linear16,
                    LanguageCode = "en-US",
                    SampleRateHertz = 8000
                }
            };

            // The first request must **only** contain the audio configuration:
            await streamingDetectIntent.WriteAsync(new StreamingDetectIntentRequest
            {
                QueryInput = queryInput,
                Session = sessionName
            });

            using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
            {
                // Subsequent requests must **only** contain the audio data.
                // Following messages: audio chunks. We just read the file in
                // fixed-size chunks. In reality you would split the user input
                // by time.
                var buffer = new byte[32 * 1024];
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(
                    buffer, 0, buffer.Length)) > 0)
                {
                    await streamingDetectIntent.WriteAsync(new StreamingDetectIntentRequest
                    {
                        Session = sessionName,
                        InputAudio = Google.Protobuf.ByteString.CopyFrom(buffer, 0, bytesRead)
                    });
                };
            }

            // Tell the service you are done sending data
            await streamingDetectIntent.WriteCompleteAsync();

            // This will complete once all server responses have been processed.
            await responseHandlerTask;

            return 0;
        }
        // [END dialogflow_detect_intent_streaming]
    }
}
