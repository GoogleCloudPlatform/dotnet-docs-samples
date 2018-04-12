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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Rewrite;

namespace Sudokumb
{
    public enum HttpsPolicy { Required, Optional };

    public class RewriteHttpsOnAppEngine : IRule
    {

        readonly HttpsPolicy _httpsPolicy;

        public RewriteHttpsOnAppEngine(HttpsPolicy httpsPolicy)
        {
            _httpsPolicy = httpsPolicy;
        }

        /// <summary>
        /// A path that we should ignore because App Engine hits it multiple
        /// times per second, and it doesn't need to be https.
        /// </summary>
        static PathString s_healthCheckPathString =
            new PathString("/_ah/health");

        /// <summary>
        /// Https requests that arrived via App Engine look like http
        /// (no ssl) requests.  Rewrite them so they look like https requests.
        /// </summary>
        /// <returns>
        /// True if the request was secure.
        /// </returns>
        public static bool Rewrite(HttpRequest request)
        {
            if (request.Scheme == "https")
            {
                return true;  // Already https.
            }
            string proto = request.Headers["X-Forwarded-Proto"]
                .FirstOrDefault();
            if (proto == "https")
            {
                // This request was sent via https from the browser to the
                // App Engine load balancer.  So it's good, but we need to
                // modify the request so that Controllers know it
                // was sent via https.
                request.IsHttps = true;
                request.Scheme = "https";
                return true;
            }
            if (request.Path.StartsWithSegments(s_healthCheckPathString))
            {
                // Accept health checks from non-ssl connections.
                return true;
            }

            return false;
        }

        void IRule.ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;
            bool wasSecure = Rewrite(request);
            if (!wasSecure && _httpsPolicy == HttpsPolicy.Required)
            {
                // Redirect to https.
                var newUrl = string.Concat(
                                    "https://",
                                    request.Host.ToUriComponent(),
                                    request.PathBase.ToUriComponent(),
                                    request.Path.ToUriComponent(),
                                    request.QueryString.ToUriComponent());
                var action = new RedirectResult(newUrl);
                // Execute the redirect.
                ActionContext actionContext = new ActionContext()
                {
                    HttpContext = context.HttpContext
                };
                action.ExecuteResult(actionContext);
                context.Result = RuleResult.EndResponse;
            }
        }
    }
}
