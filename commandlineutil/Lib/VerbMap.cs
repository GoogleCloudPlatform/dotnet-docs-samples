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
using System.Collections.Generic;
using System.Linq;

namespace GoogleCloudSamples
{
    /// <summary>
    /// A fluent interface to CommandLineParser library that can handle more
    /// 16 verbs.
    /// 
    /// <param name="ResultType">The return type of your verb delegates.
    /// Usually an int.
    /// </param>
    /// </summary>
    public class VerbMap<ResultType>
    {
        protected readonly Dictionary<Type, Func<object, ResultType>> _verbs =
                new Dictionary<Type, Func<object, ResultType>>();

        /// <summary>
        /// When an Action (with no return value) is invoked by Run(), this
        /// value will be returned.
        /// </summary>
        public ResultType DefaultResult { get; set; }

        /// <summary>
        /// Invoked when the command line arguments failed to parse.
        /// </summary>        
        public Func<NotParsed<object>, ResultType> NotParsedFunc { get; set; }

        /// <summary>
        /// Map an OptionsType to its handler function.
        /// </summary>
        public VerbMap<ResultType> Add<OptionsType>(Func<OptionsType, ResultType> f)
        {
            _verbs.Add(typeof(OptionsType), (object a) => f((OptionsType)a));
            return this;
        }

        /// <summary>
        /// Map an OptionsType to its handler action.
        /// </summary>
        public VerbMap<ResultType> Add<ArgType>(Action<ArgType> f)
        {
            _verbs.Add(typeof(ArgType), (object a) =>
            {
                f((ArgType)a);
                return DefaultResult;
            });
            return this;
        }

        /// <summary>
        /// Invoke a verb based on the arguments.
        /// </summary>
        public ResultType Run(string[] args, Parser parser = null)
        {
            parser = parser ?? Parser.Default;
            ParserResult<object> result =
                parser.ParseArguments(args, _verbs.Keys.ToArray());
            var parsed = result as Parsed<object>;
            if (parsed != null)
            {
                return _verbs[parsed.Value.GetType()](parsed.Value);
            }
            if (NotParsedFunc == null)
            {
                return DefaultResult;
            }
            return NotParsedFunc((NotParsed<object>)result);
        }
    }
}
