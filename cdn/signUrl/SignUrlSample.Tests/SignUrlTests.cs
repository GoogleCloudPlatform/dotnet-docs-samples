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
using Xunit;

namespace SignUrlSample.Tests
{
    public static class DataForTest
    {
        public static TheoryData<string, string, DateTime, string> TestData
        {
            get
            {
                return new TheoryData<string, string, DateTime, string>
                {
                   {
                        "http://35.186.234.33/index.html",
                        "my-key",
                        new DateTime(2019, 5, 17, 22, 15, 50),
                        "http://35.186.234.33/index.html?Expires=1558131350&KeyName=my-key&Signature=fm6JZSmKNsB5sys8VGr-JE4LiiE="
                   },
                   {
                        "https://www.google.com/",
                        "my-key",
                        new DateTime(2019, 2, 9, 22, 30, 1),
                        "https://www.google.com/?Expires=1549751401&KeyName=my-key&Signature=M_QO7BGHi2sGqrJO-MDr0uhDFuc="
                   },
                   {
                        "https://www.example.com/some/path?some=query&another=param",
                        "my-key",
                        new DateTime(2019, 2, 9, 22, 31, 1),
                        "https://www.example.com/some/path?some=query&another=param&Expires=1549751461&KeyName=my-key&Signature=sTqqGX5hUJmlRJ84koAIhWW_c3M="
                   },
                };
            }
        }
    }

    /// <summary>
    /// Tests signing Urls
    /// </summary>
    public class SignUrlTests
    {
        /// <summary>
        /// Key used to sign all test Urls
        /// </summary>
        private static readonly string s_signingKey = "nZtRohdNF9m3cKM24IcK4w==";

        [Theory]
        [MemberData(nameof(SignUrlSample.Tests.DataForTest.TestData), MemberType = typeof(DataForTest))]
        public void SignUrls(string url, string keyName, DateTime time, string expected)
        {
            string actual = Program.CreateSignedUrl(url, keyName, s_signingKey, time);

            Assert.Equal(expected, actual);
        }
    }
}
