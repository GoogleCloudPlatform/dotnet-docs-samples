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
            WebDriver.Close();
        }
    }

    public class WebAppTest : IClassFixture<WebDriverTestFixture>
    {
        private readonly ITestOutputHelper output;
        private readonly IWebDriver browser;

        public WebAppTest(ITestOutputHelper output, WebDriverTestFixture fixture)
        {
            this.output = output;
            this.browser = fixture.WebDriver;
        }

        [Fact]
        public void VisitPages()
        {
            // Visit all the pages in the top navbar.
            browser.Navigate().GoToUrl("http://localhost:5510");
            IWebElement title = browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Home Page", title.InnerText());

            IWebElement link = browser.FindElement(By.Id("nav-solve"));
            link.Click();
            title = browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Solve", title.InnerText());

            link = browser.FindElement(By.Id("nav-admin"));
            link.Click();
            title = browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Log in", title.InnerText());

            link = browser.FindElement(By.Id("nav-home"));
            link.Click();
            title = browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Home Page", title.InnerText());

            // Click the Try it! button.
            IWebElement button = browser.FindElement(By.CssSelector(".btn-primary"));
            button.Click();
            title = browser.FindElement(By.CssSelector("title"));
            Assert.Contains("Solve", title.InnerText());            
        }
    }
}
