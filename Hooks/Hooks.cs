using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;

namespace SwagProject.Hooks
{
    [Binding]
    public class Hooks
    {
        public static IWebDriver? driver; // Made public for accessibility
        private readonly ScenarioContext _scenarioContext;
        public List<string> screenshotPaths = new List<string>(); // Made public if needed

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            if (driver == null)
            {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--headless");
                options.AddArgument("--disable-gpu");
                options.AddArgument("--window-size=1920,1080");
                driver = new ChromeDriver(options);
            }
            _scenarioContext["WebDriver"] = driver; // Store driver in ScenarioContext for access
        }

        public static IWebDriver? GetDriver()
        {
            return driver;
        }

        [AfterStep]
        public void TakeScreenshotAfterStep()
        {
            if (_scenarioContext.TestError != null && driver != null)
            {
                string screenshotDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");
                Directory.CreateDirectory(screenshotDirectory);
                string screenshotFile = Path.Combine(screenshotDirectory, $"{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMddHHmmss}.png");
                ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(screenshotFile);
                screenshotPaths.Add(screenshotFile);
            }
        }

        [AfterScenario]
        public void AfterScenario()
        {
            if (driver != null)
            {
                driver.Quit();
                driver = null;
            }
            SendEmailReport();
        }

        public void SendEmailReport() // Made public for better accessibility
        {
            string senderEmail = "shankariu804@gmail.com";
            string senderPassword = "exry tjbv yrxb ctnu";
            string receiverEmail = "shankariu8@gmail.com";
            string smtpServer = "smtp.gmail.com";
            int smtpPort = 587;

            using (MailMessage mail = new MailMessage())
            using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
            {
                mail.From = new MailAddress(senderEmail);
                mail.To.Add(receiverEmail);
                mail.Subject = "Test Execution Report";
                mail.Body = "Please find the test execution report and screenshots attached.";

                foreach (var screenshotPath in screenshotPaths)
                {
                    if (File.Exists(screenshotPath))
                    {
                        mail.Attachments.Add(new Attachment(screenshotPath));
                    }
                }

                smtpClient.Credentials = new NetworkCredential(senderEmail, senderPassword);
                smtpClient.EnableSsl = true;

                try
                {
                    smtpClient.Send(mail);
                    Console.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email: {ex.Message}");
                }
            }
        }
    }
}



/*using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;

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
        private static string reportPath;
        private static string screenshotsDir;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
            reportPath = Path.Combine(reportDirectory, "ExtentReport.html");
            screenshotsDir = Path.Combine(reportDirectory, "Screenshots");

            Directory.CreateDirectory(reportDirectory);
            Directory.CreateDirectory(screenshotsDir);

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
                ChromeOptions options = new ChromeOptions();
                options.BinaryLocation = "/usr/bin/chromium-browser"; // Set Chrome binary path for Linux
                options.AddArguments("--headless", "--disable-gpu", "--window-size=1920,1080");
                options.AddArguments("--disable-dev-shm-usage", "--no-sandbox");

                string driverPath = "/usr/local/bin/";
                driver = new ChromeDriver(driverPath, options);
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
                _scenario.Log(Status.Pass, stepText,
                    MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
            }
            else
            {
                _scenario.Log(Status.Fail, stepText,
                    MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
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
            SendEmailWithGmail();
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

                Thread.Sleep(500);
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                string sanitizedStepName = string.Join("_", stepName.Split(Path.GetInvalidFileNameChars()));
                string fileName = $"{scenarioName}_{sanitizedStepName}.png";
                string filePath = Path.Combine(screenshotsDir, fileName);

                screenshot.SaveAsFile(filePath);
                return Path.Combine("Screenshots", fileName);
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return null;
            }
        }

        private static void SendEmailWithGmail()
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string senderEmail = "shankariu804@gmail.com";
                string senderPassword = "exry tjbv yrxb ctnu"; // Replace with an environment variable for security
                string recipientEmail = "shankariu8@gmail.com";

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "SpecFlow Test Report & Screenshots",
                    Body = "Attached are the Extent Report and failure screenshots from the latest test execution.",
                    IsBodyHtml = false
                };

                mail.To.Add(recipientEmail);

                if (File.Exists(reportPath))
                {
                    mail.Attachments.Add(new Attachment(reportPath));
                }
                else
                {
                    TestContext.Progress.WriteLine("⚠️ Report file not found: " + reportPath);
                }

                string[] screenshotFiles = Directory.GetFiles(screenshotsDir, "*.png");
                if (screenshotFiles.Length == 0)
                {
                    TestContext.Progress.WriteLine("⚠️ No screenshots found to attach.");
                }

                foreach (string screenshot in screenshotFiles)
                {
                    mail.Attachments.Add(new Attachment(screenshot));
                }

                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                smtp.Send(mail);
                TestContext.Progress.WriteLine("✅ Email sent successfully via Gmail SMTP!");
            }
            catch (SmtpException smtpEx)
            {
                TestContext.Progress.WriteLine($"❌ SMTP Error: {smtpEx.StatusCode} - {smtpEx.Message}");
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"❌ Failed to send email via Gmail SMTP: {ex.Message}");
            }
        }
    }
}
*/


/*using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;

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
        private static string reportPath;
        private static string screenshotsDir;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
            reportPath = Path.Combine(reportDirectory, "ExtentReport.html");
            screenshotsDir = Path.Combine(reportDirectory, "Screenshots");

            Directory.CreateDirectory(reportDirectory);
            Directory.CreateDirectory(screenshotsDir);

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
                ChromeOptions options = new ChromeOptions();

                // Set Chrome binary path for Linux runners
                options.BinaryLocation = "/usr/bin/chromium-browser";

                // Set necessary Chrome options for CI/CD
                options.AddArguments("--headless", "--disable-gpu", "--window-size=1920,1080");
                options.AddArguments("--disable-dev-shm-usage", "--no-sandbox");

                // Ensure ChromeDriver has execution permissions
                string driverPath = "/usr/local/bin/";
                driver = new ChromeDriver(driverPath, options);
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
                _scenario.Log(Status.Pass, stepText,
                    MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
            }
            else
            {
                _scenario.Log(Status.Fail, stepText,
                    MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
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
            SendEmailWithGmail();
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

                Thread.Sleep(500);
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                string sanitizedStepName = string.Join("_", stepName.Split(Path.GetInvalidFileNameChars()));
                string fileName = $"{scenarioName}_{sanitizedStepName}.png";
                string filePath = Path.Combine(screenshotsDir, fileName);

                screenshot.SaveAsFile(filePath);
                return Path.Combine("Screenshots", fileName);
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return null;
            }
        }

        private static void SendEmailWithGmail()
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string senderEmail = "shankariu804@gmail.com";
                string senderPassword = "exry tjbv yrxb ctnu"; // Use environment variable for security
                string recipientEmail = "shankariu8@gmail.com";

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "SpecFlow Test Report & Screenshots",
                    Body = "Attached are the Extent Report and failure screenshots from the latest test execution.",
                    IsBodyHtml = false
                };

                mail.To.Add(recipientEmail);

                if (File.Exists(reportPath))
                    mail.Attachments.Add(new Attachment(reportPath));

                foreach (string screenshot in Directory.GetFiles(screenshotsDir, "*.png"))
                {
                    mail.Attachments.Add(new Attachment(screenshot));
                }

                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                smtp.Send(mail);
                TestContext.Progress.WriteLine("✅ Email sent successfully via Gmail SMTP!");
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"❌ Failed to send email via Gmail SMTP: {ex.Message}");
            }
        }
    }
}
*/



/*using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using TechTalk.SpecFlow;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;

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
        private static string reportPath;
        private static string screenshotsDir;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            string reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
            reportPath = Path.Combine(reportDirectory, "ExtentReport.html");
            screenshotsDir = Path.Combine(reportDirectory, "Screenshots");

            Directory.CreateDirectory(reportDirectory);
            Directory.CreateDirectory(screenshotsDir);

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
                ChromeOptions options = new ChromeOptions();
                
                // Explicitly set Chrome binary path for Linux CI
                options.BinaryLocation = "/usr/bin/google-chrome"; 
                
                // Set necessary Chrome options for CI/CD
                options.AddArgument("--headless");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");

                // Ensure ChromeDriver has execution permissions
                string driverPath = "/usr/local/bin/";
                driver = new ChromeDriver(driverPath, options);
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
                _scenario.Log(Status.Pass, stepText,
                    MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
            }
            else
            {
                _scenario.Log(Status.Fail, stepText,
                    MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath).Build());
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
            SendEmailWithGmail();  
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

                Thread.Sleep(500);
                Screenshot screenshot = ((ITakesScreenshot)driver).GetScreenshot();

                string sanitizedStepName = string.Join("_", stepName.Split(Path.GetInvalidFileNameChars()));
                string fileName = $"{scenarioName}_{sanitizedStepName}.png";
                string filePath = Path.Combine(screenshotsDir, fileName);

                screenshot.SaveAsFile(filePath);
                return Path.Combine("Screenshots", fileName);
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"Failed to capture screenshot: {ex.Message}");
                return null;
            }
        }

        private static void SendEmailWithGmail()
        {
            try
            {
                string smtpServer = "smtp.gmail.com";
                int smtpPort = 587;
                string senderEmail = "shankariu804@gmail.com"; 
                string senderPassword = "exry tjbv yrxb ctnu"; 
                string recipientEmail = "shankariu8@gmail.com";

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = "SpecFlow Test Report & Screenshots",
                    Body = "Attached are the Extent Report and failure screenshots from the latest test execution.",
                    IsBodyHtml = false
                };

                mail.To.Add(recipientEmail);

                if (File.Exists(reportPath))
                    mail.Attachments.Add(new Attachment(reportPath));

                foreach (string screenshot in Directory.GetFiles(screenshotsDir, "*.png"))
                {
                    mail.Attachments.Add(new Attachment(screenshot));
                }

                SmtpClient smtp = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                smtp.Send(mail);
                TestContext.Progress.WriteLine("✅ Email sent successfully via Gmail SMTP!");
            }
            catch (Exception ex)
            {
                TestContext.Progress.WriteLine($"❌ Failed to send email via Gmail SMTP: {ex.Message}");
            }
        }
    }
}
*/
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
