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
        private static Uri _xrmUri; // = new Uri(System.Configuration.ConfigurationManager.AppSettings["OnlineCrmUrl"].ToString());
        private string statusText;


        #region CRM test user credentials

        private static SecureString pscWasteUserName1 = new SecureString(); 
        private static SecureString pscWastePassword1 = new SecureString();

        private static SecureString pscWasteUserName2 = new SecureString();
        private static SecureString pscWastePassword2 = new SecureString();

        private static SecureString wasteTeamLeadUsername = new SecureString();
        private static SecureString wasteTeamLeadPassword = new SecureString();

        private static SecureString wasteIntelUsername = new SecureString();
        private static SecureString wasteIntelPassword = new SecureString();

        #endregion

        public WastePermitTests()
        {
           

        }

        [ClassInitialize]
        public static void SetupTests(TestContext testContext)
        {
            TestContext TestContext = testContext;

            // Get the usernames and password
            GetSecureStringFromSetting(TestContext, nameof(pscWasteUserName1), pscWasteUserName1);
            GetSecureStringFromSetting(TestContext, nameof(pscWastePassword1), pscWastePassword1);
            GetSecureStringFromSetting(TestContext, nameof(pscWasteUserName2), pscWasteUserName2);
            GetSecureStringFromSetting(TestContext, nameof(pscWastePassword2), pscWastePassword2);
            GetSecureStringFromSetting(TestContext, nameof(wasteTeamLeadUsername), wasteTeamLeadUsername);
            GetSecureStringFromSetting(TestContext, nameof(wasteTeamLeadPassword), wasteTeamLeadPassword);
            GetSecureStringFromSetting(TestContext, nameof(wasteIntelUsername), wasteIntelUsername);
            GetSecureStringFromSetting(TestContext, nameof(wasteIntelPassword), wasteIntelPassword);
            
            // Get the url
            string urlKey = "https://ag-ea-lp-qa-master.crm4.dynamics.com";
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

        private static void GetSecureStringFromSetting(TestContext TestContext, string configKey, SecureString secureString)
        {
            string configValue = null;
            if (TestContext.Properties.Contains(configKey))
            {
                configValue = TestContext.Properties[configKey].ToString();
            }
            else
            {
                configValue = ConfigurationManager.AppSettings[configKey];
            }

            foreach (char c in configValue)
            {
                secureString.AppendChar(c);
            }
        }

        /*(
        [TestMethod]
        public void WastePermitsApplicationGlobalSearchOpenRecord()
        {
            using (var xrmBrowser = new Microsoft.Dynamics365.UIAutomation.Api.Browser(TestSettings.Options))
            {
                xrmBrowser.LoginPage.Login(_xrmUri, pscWasteUserName1, pscWastePassword1);
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
                xrmBrowser.LoginPage.Login(_xrmUri, pscWasteUserName1, pscWastePassword1);
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
                xrmBrowser.LoginPage.Login(_xrmUri, pscWasteUserName2, pscWastePassword2);
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
                xrmBrowser.LoginPage.Login(_xrmUri, pscWasteUserName1, pscWastePassword1);
                
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
