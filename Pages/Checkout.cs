//using OpenQA.Selenium;

//namespace SwagProject.Pages
//{
//    internal class Checkout
//    {
//        private readonly IWebDriver driver;

//        public Checkout(IWebDriver driver)
//        {
//            this.driver = driver;
//        }

//        // Locators
//        private readonly By checkoutBtn = By.XPath("//button[@id='checkout']");
//        private readonly By firstNameField = By.XPath("//input[@id='first-name']");
//        private readonly By lastNameField = By.XPath("//input[@id='last-name']");
//        private readonly By zipCodeField = By.XPath("//input[@id='postal-code']");
//        private readonly By continueButton = By.XPath("//input[@id='continue']");

//        public Checkout ClickCheckout()
//        {
//            driver.FindElement(checkoutBtn).Click();
//            return this;
//        }

//        public Checkout EnterDetails(string firstName, string lastName, string zipCode)
//        {
//            driver.FindElement(firstNameField).SendKeys(firstName);
//            driver.FindElement(lastNameField).SendKeys(lastName);
//            driver.FindElement(zipCodeField).SendKeys(zipCode);
//            return this;
//        }

//        public Overview ClickContinue()
//        {
//            driver.FindElement(continueButton).Click();
//            return new Overview(driver);
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using SwagProject.Locators;

namespace SwagProject.Pages
{
    internal class Checkout
    {
        IWebDriver driver = SwagProject.Hooks.Hooks.driver;

        /* public Checkout(IWebDriver driver)
         {

             this.driver = driver;
         }*/
        //locators on checkout page

        //By checkoutbtn = By.XPath("//button[@id='checkout']");
        //By checkOutFirstName = By.XPath("//input[@id='first-name']");
        //By checkOutLastName = By.XPath("//input[@id='last-name']");
        //By checkOutPostalCode = By.XPath("//input[@id='postal-code']");
        //By continueCheckOut = By.XPath("//input[@id='continue']");
        public void checkout()
        {
            driver.FindElement(check.checkoutbtn).Click();
        }
        public void details(String firstname, String lastname, String zipcode)
        {
            driver.FindElement(check.checkOutFirstName).SendKeys(firstname);
            driver.FindElement(check.checkOutLastName).SendKeys(lastname);
            driver.FindElement(check.checkOutPostalCode).SendKeys(zipcode);
        }
        public void continued()
        {
            driver.FindElement(check.continueCheckOut).Click();
        }
    }
}
