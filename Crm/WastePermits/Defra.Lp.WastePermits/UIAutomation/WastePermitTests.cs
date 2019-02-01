using System;
using System.Configuration;
using System.Security;
using Microsoft.Dynamics365.UIAutomation.Browser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace WastePermits.UIAutomation
{
    [TestClass]
    public class WastePermitTests
    {
        public TestContext TestContext { get; set; }

        private static SecureString _username = new SecureString(); // = System.Configuration.ConfigurationManager.AppSettings["OnlineUsername"].ToSecureString();
        private static SecureString _password = new SecureString(); //System.Configuration.ConfigurationManager.AppSettings["OnlinePassword"].ToSecureString();
        private static Uri _xrmUri; // = new Uri(System.Configuration.ConfigurationManager.AppSettings["OnlineCrmUrl"].ToString());
        private string statusText;

        public WastePermitTests()
        {
           

        }

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            TestContext TestContext = testContext;
            string usernameKey = "crmUserName";
            string pwdKey = "crmPassword";
            string urlKey = "crmDestinationUrl";

            // Get the username
            string username = null;
            if (TestContext.Properties.Contains(usernameKey))
            {
                username = TestContext.Properties[usernameKey].ToString();
            }
            else
            {
                username = ConfigurationManager.AppSettings[usernameKey];
            }         
            foreach (char c in username)
            {
                _username.AppendChar(c);
            }

            // Get the password
            string pwd = null;
            if (TestContext.Properties.Contains(pwdKey))
            {
                pwd = TestContext.Properties[pwdKey].ToString();
            }
            else
            {
                pwd = ConfigurationManager.AppSettings[pwdKey];
            }
            foreach (char c in pwd)
            {
                _password.AppendChar(c);
            }


            // Get the url
            string url = null;
            if (TestContext.Properties.Contains(urlKey))
            {
                url = TestContext.Properties[urlKey].ToString();
            }
            else
            {
                url = ConfigurationManager.AppSettings[urlKey];
            }

            _xrmUri = new Uri(url);
        }

        /*(
        [TestMethod]
        public void WastePermitsApplicationGlobalSearchOpenRecord()
        {
            using (var xrmBrowser = new Microsoft.Dynamics365.UIAutomation.Api.Browser(TestSettings.Options))
            {
                xrmBrowser.LoginPage.Login(_xrmUri, _username, _password);
                xrmBrowser.GuidedHelp.CloseGuidedHelp();

                xrmBrowser.ThinkTime(500);
                try { xrmBrowser.GuidedHelp.CloseGuidedHelp(); }
                catch { }

                xrmBrowser.ThinkTime(2000);
                try
                { xrmBrowser.Dialogs.CloseWarningDialog(); }
                catch { }

                xrmBrowser.ThinkTime(500);
                xrmBrowser.Navigation.GlobalSearch("");
                xrmBrowser.GlobalSearch.Search("EPR/WE7444QB/A001");
                //xrmBrowser.ThinkTime(4000);
                //xrmBrowser.GlobalSearch.FilterWith("Application");
                xrmBrowser.ThinkTime(4000);
                xrmBrowser.GlobalSearch.OpenRecord("Applications", 0);
                xrmBrowser.ThinkTime(10000);
            }
        }
        */

        [TestMethod]
        public void OpenApplication()
        {
            using (var xrmBrowser = new Microsoft.Dynamics365.UIAutomation.Api.Browser(TestSettings.Options))
            {
                xrmBrowser.LoginPage.Login(_xrmUri, _username, _password);
                xrmBrowser.ThinkTime(500);
                try { xrmBrowser.GuidedHelp.CloseGuidedHelp(); }
                catch { }

                xrmBrowser.ThinkTime(2000);
                try
                { xrmBrowser.Dialogs.CloseWarningDialog(); }
                catch { }

                xrmBrowser.Navigation.OpenSubArea("LP", "Applications");
                xrmBrowser.Grid.SwitchView("Active Applications");
                xrmBrowser.Grid.Search("*WE");
                xrmBrowser.Grid.OpenRecord(0);
                xrmBrowser.ThinkTime(10000);
                // do
                // {
                //    xrmBrowser.ThinkTime(500);
                //     var id = OpenQA.Selenium.By.Id("Application Type_label");
                //    statusText = xrmBrowser.Driver.FindElement(id).Text;
                //  } while (statusText == "New Application");
                //  Assert.IsTrue(String.IsNullOrEmpty(statusText), statusText);
            }
        }

        [TestMethod]
        public void OpenApplicationRecord()
        {
            using (var xrmBrowser = new Microsoft.Dynamics365.UIAutomation.Api.Browser(TestSettings.Options))
            {
                xrmBrowser.LoginPage.Login(_xrmUri, _username, _password);
                xrmBrowser.ThinkTime(500);
                try { xrmBrowser.GuidedHelp.CloseGuidedHelp(); }
                catch { }

                xrmBrowser.ThinkTime(2000);
                try
                { xrmBrowser.Dialogs.CloseWarningDialog(); }
                catch { }

                xrmBrowser.Navigation.OpenSubArea("LP", "Applications");
                xrmBrowser.Grid.SwitchView("Active Applications");
                xrmBrowser.Grid.EnableFilter();

                xrmBrowser.Grid.OpenRecord(0);

            }
        }


        [TestMethod]
        public void ClosePendingEmailReminder()
        {
            using (var xrmBrowser = new Microsoft.Dynamics365.UIAutomation.Api.Browser(TestSettings.Options))
            {
                xrmBrowser.LoginPage.Login(_xrmUri, _username, _password);
                
                try
                {
                    xrmBrowser.GuidedHelp.CloseGuidedHelp();
                }
                catch { }

                xrmBrowser.ThinkTime(2000);
                try
                {
                    xrmBrowser.Dialogs.CloseWarningDialog();
                }
                catch
                {

                }
                
                xrmBrowser.ThinkTime(2000);
                xrmBrowser.Navigation.OpenSubArea("LP", "Applications");
                
                xrmBrowser.ThinkTime(2000);
                xrmBrowser.Grid.SwitchView("Active Applications");
                 
                xrmBrowser.ThinkTime(1000);
                xrmBrowser.Grid.OpenRecord(0);
                xrmBrowser.ThinkTime(1000);
              

            }
        }
    }
}
