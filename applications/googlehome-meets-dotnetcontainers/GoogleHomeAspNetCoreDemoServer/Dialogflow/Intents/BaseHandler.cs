// Copyright(c) 2018 Google Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.
using System.Threading.Tasks;

namespace GoogleHomeAspNetCoreDemoServer.Dialogflow.Intents
{
    /// <summary>
    /// Base class for intent handlers. 
    /// </summary>
    public abstract class BaseHandler
    {
        protected readonly Conversation _conversation;

        /// <summary>
        /// Initializes class with the conversation.
        /// </summary>
        /// <param name="conversation">Conversation</param>
        public BaseHandler(Conversation conversation)
        {
            _conversation = conversation;
        }

        /// <summary>
        /// Base async method that simply returns a Task with null. 
        /// Subclasses can override to provide an implementation.
        /// </summary>
        /// <param name="req">Conversation request</param>
        /// <returns>Task with null</returns>

        public virtual Task<string> HandleAsync(ConvRequest req)
        {
            return Task.FromResult<string>(null);
        }

        /// <summary>
        /// Base method that simply returns null.
        /// Sublasses can override to provide an implementation.
        /// </summary>
        /// <param name="req">Conversation request</param>
        /// <returns>null</returns>
        public virtual string Handle(ConvRequest req)
        {
            return null;
        }
    }
}
