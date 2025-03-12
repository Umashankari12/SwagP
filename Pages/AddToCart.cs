using System;
using System.Threading;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using SwagProject.Locators;

namespace SwagProject.Pages
{
    internal class AddToCart
    {
        IWebDriver driver = SwagProject.Hooks.Hooks.driver;

        // Method to click on the product image (or link)
        public void productclick()
        {
            // Wait for the product to be clickable and then click it
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var product = wait.Until(ExpectedConditions.ElementToBeClickable(Addcart.clickproduct));

            // Scroll the product into view
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", product);

            // Click the product
            product.Click();
            Thread.Sleep(2000); // Optional: Consider replacing Thread.Sleep with explicit waits
        }

        // Method to click on the Add to Cart button
        public void AddCart()
        {
            // Wait for the Add to Cart button to be clickable
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var addToCartButton = wait.Until(ExpectedConditions.ElementToBeClickable(Addcart.addtocartbtn));

            // Click the Add to Cart button
            addToCartButton.Click();
            Thread.Sleep(2000); // Optional: Consider replacing Thread.Sleep with explicit waits
        }

        // Method to click on the Cart icon
        public void CartIcon()
        {
            // Wait for the Cart icon to be clickable
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var cartIcon = wait.Until(ExpectedConditions.ElementToBeClickable(Addcart.carticon));

            // Click the Cart icon
            cartIcon.Click();
            Thread.Sleep(1000); // Optional: Consider replacing Thread.Sleep with explicit waits
        }
    }
}

// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
// using OpenQA.Selenium;
// using SwagProject.Locators;

// namespace SwagProject.Pages
// {
//     internal class AddToCart
//     {
//         IWebDriver driver = SwagProject.Hooks.Hooks.driver;

//         public void productclick()
//         {
//             driver.FindElement(Addcart.clickproduct).Click();
//             Thread.Sleep(2000);

//         }
//         public void AddCart()
//         {
//             driver.FindElement(Addcart.addtocartbtn).Click();
//             Thread.Sleep(2000);
//         }
//         public void CartIcon()
//         {
//             driver.FindElement(Addcart.carticon).Click();
//             Thread.Sleep(1000);
//         }
//     }
// }
