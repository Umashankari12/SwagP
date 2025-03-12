using System;
using System.Threading;
using OpenQA.Selenium;
using SwagProject.Pages;
using SwagProject.Hooks;  // Ensure this is included
using TechTalk.SpecFlow;

namespace SwagProject.StepDefinitions
{
    [Binding]
    public class OverviewStepDefinitions
    {
        private readonly IWebDriver driver;
        private readonly Overview over;

        public OverviewStepDefinitions(ScenarioContext scenarioContext)
        {
            // Get the WebDriver instance from Hooks
            driver = Hooks.driver; 

            // If using ScenarioContext:
            // driver = scenarioContext["WebDriver"] as IWebDriver;

            over = new Overview();
        }

        [Given(@"User is on the Checkout Overview page")]
        public void GivenUserIsOnTheCheckoutOverviewPage()
        {
            Console.WriteLine("User is on the Checkout Overview page.");
        }

        [When(@"User clicks on Finish")]
        public void WhenUserClicksOnFinish()
        {
            driver.FindElement(By.XPath("//button[@id='finish']")).Click();
            Thread.Sleep(1000);
        }

        [Then(@"Order status should be visible")]
        public void ThenOrderStatusShouldBeVisible()
        {
            Console.WriteLine("Thank you for your order.");
            Thread.Sleep(1000);
        }
    }
}


// using System;
// using System.Reflection.Emit;
// using OpenQA.Selenium;
// using SwagProject.Pages;
// using TechTalk.SpecFlow;

// namespace SwagProject.StepDefinitions
// {
//     [Binding]
//     public class OverviewStepDefinitions
//     {
//         /*private readonly ScenarioContext scenarioContext;
//         private IWebDriver driver;
//         private LoginPage lp;
//         private AddToCart cart;
//         private Checkout check;*/
//          Overview over;
//         public OverviewStepDefinitions()
//         {
//            /* scenarioContext = scenarioContext;
//             driver = scenarioContext["WebDriver"] as IWebDriver;
//             lp = new LoginPage(driver);
//             cart = new AddToCart(driver);
//             check = new Checkout(driver);*/
//             over = new Overview();

//         }

//         [Given(@"User is on the Checkout Overview page")]
//         public void GivenUserIsOnTheCheckoutOverviewPage()
//         {
//             /*lp.launchbrowser();
//             lp.enterusernamepass("standard_user", "secret_sauce");
//             lp.login();
//             cart.productclick();
//             cart.AddCart();
//             cart.CartIcon();
//             check.checkout();
//             check.details("uma", "shankari", "123456");
//             check.continued();*/
//             Console.WriteLine(" ");
//         }

//         [When(@"User clicks on Finish")]
//         public void WhenUserClicksOnFinish()
//         {
//             driver.FindElement(By.XPath("//button[@id='finish']")).Click();
//             Thread.Sleep(1000);
//         }


//         // [When(@"User clicks on Finish")]
//         // public void WhenUserClicksOnFinish()
//         // {
//         //     over.finish();
//         //     Thread.Sleep(1000);
//         // }

//         [Then(@"Order status should be visible")]
//         public void ThenOrderStatusShouldBeVisible()
//         {
//             Console.WriteLine("Thank you for your order");
//             Thread.Sleep(1000);
//         }
//     }
// }
