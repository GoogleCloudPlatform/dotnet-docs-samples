/*
 * Copyright (c) 2016 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 */

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
            // [START log4net_config]
            // Configure log4net to use Stackdriver logging from the XML configuration file.
            XmlConfigurator.Configure(new FileInfo(configFile));
            // [END log4net_config]


            // [START log4net_log_entry]
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
