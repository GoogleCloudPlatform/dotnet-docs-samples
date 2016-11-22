using System;
using System.IO;
using log4net;
using log4net.Config;
using System.Threading;

namespace GoogleCloudSamples
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFile = "log4net.config.xml";
            // Add custom path to config file, if passed in via args
            if (args.Length != 0)
            {
                configFile = Path.Combine(args[0], configFile);
            }
            // [START log4net_log_entry]
            // Configure log4net to use Stackdriver logging from the XML configuration file.
            XmlConfigurator.Configure(new FileInfo(configFile));

            // Retrieve a logger for this context.
            ILog log = LogManager.GetLogger(typeof(Program));

            // Log some information to Google Stackdriver Logging.
            log.Info("Hello World!");
            // [END log4net_log_entry]
            // Pause for 5 seconds to ensure log entry is completed
            Thread.Sleep(5000);
            Console.WriteLine("Log Entry created.");
        }
    }
}
