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

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TwelveFactor.Services {
    /// Queues log messages until Logger property is set.
    public class DelayedLogger : ILogger
    {
        private ILogger _logger;
        public ILogger InnerLogger
        {
            get { return _logger;}
            set { 
                _logger = value;
                foreach (var logAction in _logActions) {
                    logAction(_logger);
                }
                _logActions.Clear();
            }
        }
        
        List<Action<ILogger>> _logActions = new List<Action<ILogger>>();
        IDisposable ILogger.BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }

        bool ILogger.IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, 
            TState state, Exception exception, 
            Func<TState, Exception, string> formatter)
        {
            if (_logger == null) {
                _logActions.Add((ILogger logger) => 
                    logger.Log(logLevel, eventId, state, exception, formatter));
            } else {
                _logger.Log(logLevel, eventId, state, exception, formatter);
            }
        }
    }
}