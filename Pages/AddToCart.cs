

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using SwagProject.Locators;

namespace SwagProject.Pages
{
    internal class AddToCart
    {
        IWebDriver driver = SwagProject.Hooks.Hooks.driver;

        public void productclick()
        {
            Thread.Sleep(1000);
            driver.FindElement(Addcart.clickproduct).Click();
            Thread.Sleep(2000);

        }
        public void AddCart()
        {
            driver.FindElement(Addcart.addtocartbtn).Click();
            Thread.Sleep(2000);
        }
        public void CartIcon()
        {
            driver.FindElement(Addcart.carticon).Click();
            Thread.Sleep(1000);
        }
    }
}
