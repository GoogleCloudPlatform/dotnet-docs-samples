// Copyright (c) 2018 Google LLC.
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

using System;
using Xunit;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit.Abstractions;

namespace Sudokumb
{
    static class Extensions
    {
        public static string InnerText(this IWebElement webElement) =>
            webElement.GetAttribute("innerText");
    }

    public class WebDriverTestFixture : IDisposable
    {
        public IWebDriver WebDriver { get; private set; }
        public WebDriverTestFixture()
        {
            WebDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(),
                new ChromeOptions(), TimeSpan.FromMinutes(3));
        }

        public void Dispose()
        {
            WebDriver.Quit();
        }
    }

    public class WebAppTest : IClassFixture<WebDriverTestFixture>
    {
        private readonly ITestOutputHelper _output;
        private readonly IWebDriver _browser;

        public WebAppTest(ITestOutputHelper output, WebDriverTestFixture fixture)
        {
            _output = output;
            _browser = fixture.WebDriver;
        }

        [Fact]
        public void VisitPages()
        {
            // Visit all the pages in the top navbar.
            _browser.Navigate().GoToUrl("http://localhost:5510");
            IWebElement title = _browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Home Page", title.InnerText());

            IWebElement link = _browser.FindElement(By.Id("nav-solve"));
            link.Click();
            title = _browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Solve", title.InnerText());

            link = _browser.FindElement(By.Id("nav-admin"));
            link.Click();
            title = _browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Log in", title.InnerText());

            link = _browser.FindElement(By.Id("nav-home"));
            link.Click();
            title = _browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Home Page", title.InnerText());

            // Click the Try it! button.
            IWebElement button = _browser.FindElement(By.CssSelector(".btn-primary"));
            button.Click();
            title = _browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Solve", title.InnerText());
        }
    }
}
