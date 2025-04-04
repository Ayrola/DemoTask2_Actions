﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using NUnit.Framework;
using System;

namespace TestProject1
{
    [TestFixture]
    public class TestCalculator
    {
        IWebDriver driver;
        IWebElement textBoxFirstNum;
        IWebElement textBoxSecondNum;
        IWebElement dropDownOperation;
        IWebElement calcBtn;
        IWebElement resetBtn;
        IWebElement divResult;

        [OneTimeSetUp]
        public void SetUp()
        {
            ChromeOptions options = new ChromeOptions();

            // ✅ Ensure Chrome runs in headless mode (no UI)
            options.AddArguments("headless");

            // ✅ Bypass OS security model (needed in some CI/CD and Docker environments)
            options.AddArguments("no-sandbox");

            // ✅ Overcome limited shared memory problems (useful in Docker/Linux)
            options.AddArguments("disable-dev-shm-usage");

            // ✅ Disable GPU hardware acceleration (not needed in headless mode)
            // Mostly useful on Windows systems
            options.AddArguments("disable-gpu");

            // ✅ Set fixed window size so that page elements are correctly visible
            options.AddArguments("window-size=1920x1080");

            // ✅ Disable all Chrome extensions
            options.AddArguments("disable-extensions");

            // ✅ Set remote debugging port (useful for debugging with DevTools)
            options.AddArguments("remote-debugging-port=9222");

            //--user-data-dir is a Chrome command-line flag that tells Chrome where to store user data like:

            //Chrome uses a fresh, isolated profile every time,

            //you avoid file locks/ conflicts, and

            //tests run reliably in any environment.
            string userDataDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(userDataDir);
            options.AddArguments($"--user-data-dir={userDataDir}");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Url = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com/number-calculator/";

            textBoxFirstNum = driver.FindElement(By.Id("number1"));
            dropDownOperation = driver.FindElement(By.Id("operation"));
            textBoxSecondNum = driver.FindElement(By.Id("number2"));
            calcBtn = driver.FindElement(By.Id("calcButton"));
            resetBtn = driver.FindElement(By.Id("resetButton"));
            divResult = driver.FindElement(By.Id("result"));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }

        public void PerformCalculation(string firstNumber, string operation,
                                        string secondNumber, string expectedResult)
        {
            // Click the [Reset] button
            resetBtn.Click();

            // Send values to the corresponding fields if they are not empty
            if (!string.IsNullOrEmpty(firstNumber))
            {
                textBoxFirstNum.SendKeys(firstNumber);
            }

            if (!string.IsNullOrEmpty(secondNumber))
            {
                textBoxSecondNum.SendKeys(secondNumber);
            }

            if (!string.IsNullOrEmpty(operation))
            {
                new SelectElement(dropDownOperation).SelectByText(operation);
            }

            // Click the [Calculate] button
            calcBtn.Click();

            // Assert the expected and actual result text are equal
            Assert.That(divResult.Text, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase("5", "+ (sum)", "10", "Result: 15")]
        [TestCase("3.5", "- (subtract)", "1.2", "Result: 2.3")]
        [TestCase("2e2", "* (multiply)", "1.5", "Result: 300")]
        [TestCase("5", "/ (divide)", "0", "Result: Infinity")]
        [TestCase("invalid", "+ (sum)", "10", "Result: invalid input")]
        public void TestNumberCalculator(string firstNumber, string operation,
                                            string secondNumber, string expectedResult)
        {
            PerformCalculation(firstNumber, operation, secondNumber, expectedResult);
        }
    }
}
