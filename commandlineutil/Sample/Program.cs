/*
 * Copyright (c) 2017 Google Inc.
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

using CommandLine;
using System;

namespace GoogleCloudSamples
{
    class CommandLineUtilSample
    {
        [Verb("v1")] class Verb1 { };
        [Verb("v2")] class Verb2 { };
        [Verb("v3")] class Verb3 { };
        [Verb("v4")] class Verb4 { };
        [Verb("v5")] class Verb5 { };
        [Verb("v6")] class Verb6 { };
        [Verb("v7")] class Verb7 { };
        [Verb("v8")] class Verb8 { };
        [Verb("v9")] class Verb9 { };
        [Verb("v10")] class Verb10 { };
        [Verb("v11")] class Verb11 { };
        [Verb("v12")] class Verb12 { };
        [Verb("v13")] class Verb13 { };
        [Verb("v14")] class Verb14 { };
        [Verb("v15")] class Verb15 { };
        [Verb("v16")] class Verb16 { };
        [Verb("v17")] class Verb17 { };

        static int Main(string[] args)
        {
            var verbMap = new VerbMap<int>();
            verbMap
                .Add((Verb1 v1) => Console.WriteLine("You invoked Verb1!"))
                .Add((Verb2 v2) => Console.WriteLine("You invoked Verb2!"))
                .Add((Verb3 v3) => Console.WriteLine("You invoked Verb3!"))
                .Add((Verb4 v4) => Console.WriteLine("You invoked Verb4!"))
                .Add((Verb5 v5) =>
                {
                    Console.WriteLine("You invoked Verb5!  It returns 5.");
                    return 5;
                })
                .Add((Verb6 v6) => Console.WriteLine("You invoked Verb6!"))
                .Add((Verb7 v7) => Console.WriteLine("You invoked Verb7!"))
                .Add((Verb8 v8) => Console.WriteLine("You invoked Verb8!"))
                .Add((Verb9 v9) => Console.WriteLine("You invoked Verb9!"))
                .Add((Verb10 v10) => Console.WriteLine("You invoked Verb10!"))
                .Add((Verb11 v11) => Console.WriteLine("You invoked Verb11!"))
                .Add((Verb12 v12) => Console.WriteLine("You invoked Verb12!"))
                .Add((Verb13 v13) => Console.WriteLine("You invoked Verb13!"))
                .Add((Verb14 v14) => Console.WriteLine("You invoked Verb14!"))
                .Add((Verb15 v15) => Console.WriteLine("You invoked Verb15!"))
                .Add((Verb16 v16) => Console.WriteLine("You invoked Verb16!"))
                .Add((Verb17 v17) => Console.WriteLine("You invoked Verb17!"))
                .NotParsedFunc = (err) => 255;
            return verbMap.Run(args);
        }
    }
}
