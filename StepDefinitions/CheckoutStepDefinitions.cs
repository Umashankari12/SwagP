

using System;
using OpenQA.Selenium;
using SwagProject.Pages;
using TechTalk.SpecFlow;

namespace SwagProject.StepDefinitions
{
    [Binding]
    public class CheckoutStepDefinitions
    {
        //private readonly ScenarioContext scenarioContext;
        //private IWebDriver driver=;
        /*private LoginPage lp;
        private AddToCart cart;*/
        Checkout check;
        

        public CheckoutStepDefinitions()
        {
            /*scenarioContext = scenarioContext;
            driver = scenarioContext["WebDriver"] as IWebDriver;
            lp = new LoginPage(driver);
            cart = new AddToCart(driver);*/
            check = new Checkout();

        }

        [Given(@"User is on the checkout page")]
        public void GivenUserIsOnTheCheckoutPage()
        {
            /* lp.launchbrowser();
             lp.enterusernamepass("standard_user", "secret_sauce");
             lp.login();
             cart.productclick();
             cart.AddCart();
             cart.CartIcon();

             Thread.Sleep(1000);*/
            Console.WriteLine("");
            Thread.Sleep(1000);
        }

        [When(@"User clicks checkout button")]
        public void WhenUserClicksCheckoutButton()
        {
            check.checkout();
            Thread.Sleep(1000);

        }

        [When(@"User enters ""([^""]*)"", ""([^""]*)"", and ""([^""]*)""")]
        public void WhenUserEntersFirstNameLastNameAndZipCode(string firstname, string lastname, string zipcode)
        {
            check.details(firstname, lastname, zipcode);
            Thread.Sleep(1000);
        }

        [Then(@"Clicks on Continue")]
        public void ThenThenClicksOnContinue()
        {
            check.continued();
            Thread.Sleep(1000);
        }
    }
}
