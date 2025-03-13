using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace SwagProject.Locators
{
    internal class Complete
    {
        public static By finishButton = By.XPath("//button[@id='finish']");
        public static By confirmationMessage = By.XPath("//h2[normalize-space(='Thank you for your order!']");
    }
}
