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

using GoogleCloudSamples;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Xunit;
using Xunit.Abstractions;

namespace Sudokumb
{
    internal static class Extensions
    {
        public static string InnerText(this IWebElement webElement) =>
            webElement.GetAttribute("innerText");
    }

    public class WebDriverTestFixture : IDisposable
    {
        public IWebDriver WebDriver { get; private set; }

        public WebDriverTestFixture()
        {
            WebDriver = new ChromeDriver(
                ChromeDriverService.CreateDefaultService(),
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

        private readonly RetryRobot _retryRobot = new RetryRobot()
        {
            RetryWhenExceptions = new[] { typeof(Xunit.Sdk.XunitException) }
        };

        public WebAppTest(ITestOutputHelper output,
            WebDriverTestFixture fixture)
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
            IWebElement button = _browser.FindElement(
                By.CssSelector(".btn-primary"));
            button.Click();
            title = _browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Solve", title.InnerText());
        }

        [Fact]
        public void SubmitPuzzle()
        {
            _browser.Navigate().GoToUrl("http://localhost:5510");
            IWebElement link = _browser.FindElement(By.Id("nav-solve"));
            link.Click();

            IWebElement button = _browser.FindElement(
                By.CssSelector(".btn-primary"));
            button.Click();

            IWebElement solvingMessage = _browser.FindElement(
                By.CssSelector("#solvingMessage"));
            // After a second or two, message should say "Examined N boards."
            _retryRobot.Eventually(() =>
                Assert.Contains("Examined", solvingMessage.InnerText()));

            // And it should eventually find a solution.
            IWebElement solutionPre = _browser.FindElement(
                By.CssSelector("#solutionPre"));
            _retryRobot.Eventually(() => Assert.True(solutionPre.Displayed));
        }

        [Fact]
        public void RegisterUserAndLogin()
        {
            try
            {
                _browser.Navigate().GoToUrl("http://localhost:5510");
                IWebElement link = _browser.FindElement(By.Id("nav-register"));
                link.Click();

                Assert.Contains("Register", _browser.Title);
                string email = "joe@example.com";
                string password = ",byc;sC3";

                _browser.FindElement(By.Name("Email")).SendKeys(email);
                _browser.FindElement(By.Name("Password")).SendKeys(password);
                _browser.FindElement(
                    By.Name("ConfirmPassword")).SendKeys(password);
                _browser.FindElement(
                    By.CssSelector("button[type=\"submit\"]")).Click();

                // Make sure we see hello joe!
                IWebElement manage = _browser.FindElement(
                    By.CssSelector("a[title=\"Manage\"]"));
                Assert.Contains($"Hello {email}!", manage.InnerText());

                // Logout.
                _browser.FindElement(By.CssSelector("#nav-logout")).Click();
                try
                {
                    manage = _browser.FindElement(
                        By.CssSelector("a[title=\"Manage\"]"));
                    Assert.DoesNotContain(email, manage.InnerText());
                }
                catch (NoSuchElementException)
                {
                    // The "Hello joe" element does not exist.  Good.
                }

                // Try logging in with the wrong password.
                _browser.FindElement(By.CssSelector("#nav-login")).Click();
                _browser.FindElement(By.Name("Email")).SendKeys(email);
                _browser.FindElement(
                    By.Name("Password")).SendKeys("badpassword");
                _browser.FindElement(
                    By.CssSelector("button[type=\"submit\"]")).Click();
                IWebElement errors = _browser.FindElement(
                    By.CssSelector("div.validation-summary-errors li"));
                Assert.Contains(errors.InnerText(), "Invalid login attempt.");

                // Try logging in with the correct password.
                _browser.FindElement(By.Name("Password")).SendKeys(password);
                _browser.FindElement
                (By.CssSelector("button[type=\"submit\"]")).Click();
                manage = _browser.FindElement(
                    By.CssSelector("a[title=\"Manage\"]"));
                Assert.Contains($"Hello {email}!", manage.InnerText());
            }
            finally
            {
                // Logout so we don't break other tests.
                try
                {
                    _browser.FindElement(By.CssSelector("#nav-logout")).Click();
                }
                catch (NoSuchElementException)
                {
                    // Not logged in.  Ok.
                }
            }
        }
    }
}