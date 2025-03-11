//using System;
//using NUnit.Framework;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Firefox;
//using TechTalk.SpecFlow;
//using WebDriverManager;
//using WebDriverManager.DriverConfigs.Impl;

//namespace SwagProject.Hooks
//{
//    [Binding]
//    public class Hooks
//    {
//        private static IWebDriver driver; // ✅ Static driver instance

//        [BeforeScenario("@tag1")]
//        public static void StartBrowserForLogin()
//        {
//            if (driver == null)
//            {
//                new DriverManager().SetUpDriver(new FirefoxConfig());
//                driver = new FirefoxDriver();
//                driver.Manage().Window.Maximize();
//                Console.WriteLine("Browser Launched.");
//            }
//        }

//        [AfterScenario("@uma")]
//        public static void CloseBrowser()
//        {
//            if (driver != null)
//            {
//                driver.Quit();
//                driver = null;
//                Console.WriteLine("Browser Closed.");
//            }
//        }

//        // ✅ Correct method to return WebDriver instance
//        public static IWebDriver GetDriver()
//        {
//            if (driver == null)
//            {
//                throw new Exception("WebDriver is not initialized. Ensure [BeforeScenario] runs first.");
//            }
//            return driver;
//        }
//    }
//}



//using System;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Firefox;
//using TechTalk.SpecFlow;
//using WebDriverManager;
//using WebDriverManager.DriverConfigs.Impl;

//namespace SwagProject.Hooks
//{
//    [Binding]
//    public class Hooks
//    {
//        public static IWebDriver driver;

//        [BeforeScenario]
//        public static void StartBrowser()
//        {
//            if (driver == null)
//            {
//                new DriverManager().SetUpDriver(new FirefoxConfig());
//                driver = new FirefoxDriver();
//                driver.Manage().Window.Maximize();
//                Console.WriteLine("Browser Started...");
//            }
//        }

//        [AfterScenario]
//        public static void CloseBrowser()
//        {
//            if (driver != null)
//            {
//                driver.Quit();
//                driver = null;
//                Console.WriteLine("Browser Closed.");
//            }
//        }

//        public static IWebDriver GetDriver()
//        {
//            return driver;
//        }
//    }
//}


using NUnit.Framework;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using TechTalk.SpecFlow;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports;
using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace SwagProject.Hooks
{
    [Binding]
    public class Hooks
    {
        public static IWebDriver driver;
        private readonly ScenarioContext _scenarioContext;
        private static ExtentReports _extent;
        private static ExtentTest _feature;
        private ExtentTest _scenario;
        private static ExtentSparkReporter _sparkReporter;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
            string reportPath = Path.Combine(reportDirectory, "ExtentReport.html");

            // ✅ Ensure Reports Directory Exists
            Directory.CreateDirectory(reportDirectory);

            _sparkReporter = new ExtentSparkReporter(reportPath);
            _extent = new ExtentReports();
            _extent.AttachReporter(_sparkReporter);
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            _feature = _extent.CreateTest(featureContext.FeatureInfo.Title);
        }

        [BeforeScenario]
        public void Setup()
        {
            TestContext.Progress.WriteLine("Initializing WebDriver...");

            if (driver == null)
            {
                //driver = new FirefoxDriver();
                ChromeOptions options = new ChromeOptions();

        // ✅ Specify the Chrome binary path (for CI environments)
        options.BinaryLocation = "/usr/bin/google-chrome";

        // ✅ Run Chrome in headless mode (CI/CD friendly)
        options.AddArgument("--headless");

        // ✅ Other recommended arguments
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");

        driver = new ChromeDriver(options);
                
            }

            _scenarioContext["WebDriver"] = driver;
            _scenario = _feature.CreateNode(_scenarioContext.ScenarioInfo.Title);
        }

        [AfterStep]
        public void InsertReportingSteps()
        {
            string stepText = _scenarioContext.StepContext.StepInfo.Text;
            string screenshotPath = CaptureScreenshot(_scenarioContext.ScenarioInfo.Title, stepText);

            if (_scenarioContext.TestError == null)
            {
                if (screenshotPath != null)
                {
                    _scenario.Log(Status.Pass, stepText,
                        MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
                }
                else
                {
                    _scenario.Log(Status.Pass, stepText);
                }
            }
            else
            {
                if (screenshotPath != null)
                {
                    _scenario.Log(Status.Fail, stepText,
                        MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
                }
                else
                {
                    _scenario.Log(Status.Fail, stepText);
                }

                _scenario.Log(Status.Fail, _scenarioContext.TestError.Message);
            }
        }

        [AfterScenario]
        public void TearDown()
        {
            if (driver != null)
            {
                driver.Quit();
                driver = null;
            }
        }

        [AfterTestRun]
        public static void AfterTestRun()
        {
            _extent.Flush();
        }

        private string CaptureScreenshot(string scenarioName, string stepName)
        {
            try
            {
                if (driver == null || driver.WindowHandles.Count == 0)
                {
                    TestContext.Progress.WriteLine("WebDriver is null or browser is closed. Skipping screenshot.");
                    return null;
                }

                Thread.Sleep(500);  // ✅ Small Wait Before Capturing Screenshot
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                string reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
                string screenshotDirectory = Path.Combine(reportDirectory, "Screenshots");

                Directory.CreateDirectory(screenshotDirectory);  // ✅ Ensure Folder Exists

                // ✅ Generate a Safe Filename
                string sanitizedStepName = string.Join("_", stepName.Split(Path.GetInvalidFileNameChars()));
                string fileName = $"{scenarioName}_{sanitizedStepName}.png";
                string filePath = Path.Combine(screenshotDirectory, fileName);

                screenshot.SaveAsFile(filePath);
                TestContext.Progress.WriteLine($"Screenshot saved: {filePath}");

                return Path.Combine("Screenshots", fileName);  // ✅ Return Relative Path for Extent Report
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return null;
            }
        }
    }
}


//[Binding]
//public class Hooks
//{

//        public static IWebDriver driver;

//    [BeforeTestRun]
//    public void BeforeTestRun()
//    {
//        //if (driver == null)
//        //{
//        //    var chromeOptions = new ChromeOptions();
//        //    chromeOptions.DebuggerAddress = "127.0.0.1:9222"; // Attach to running Chrome instance
//        //    driver = new ChromeDriver(chromeOptions);
//        //}
//    }
//[BeforeScenario("@Login")]
//    public static void StartBrowserForLogin()
//    {
//        if (driver == null)
//        {
//            var chromeOptions = new ChromeOptions();
//            chromeOptions.DebuggerAddress = "127.0.0.1:9222"; // Attach to running Chrome instance
//            driver = new ChromeDriver(chromeOptions);
//            //new WebDriverManager.DriverManager().SetUpDriver(new FirefoxConfig());
//            //driver = new FirefoxDriver(); // Assigning to static driver
//            Console.WriteLine("Executing Login First...");

//        }
//    }
//    /*[BeforeFeature("@tag1")]
//    public static void EnsureBrowserIsOpen()
//    {
//        if (driver == null)
//        {
//            driver = new ChromeDriver();
//            driver.Manage().Window.Maximize();
//            Console.WriteLine("Re-opening Browser for Shopping/Checkout...");
//        }
//    }*/


//    [AfterScenario("@Overview")]
//        public static void CloseBrowser()
//        {
//            if (driver != null)
//            {
//                driver.Quit();
//                driver = null;
//                Console.WriteLine("Browser Closed after Checkout.");
//            }
//        }


//        public IWebDriver GetDriver()
//        {
//            return driver;
//        }
//    }
//}
