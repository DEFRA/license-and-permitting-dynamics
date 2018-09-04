using System;
using System.Linq;
using Defra.Lp.Core.CompaniesHouse;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.CompaniesHouse.Api.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private string apiUrl= "https://api.companieshouse.gov.uk";
        private string apiKey = "YPWuQy-0wIwYAtR5FkGC480iEkQZ51PdSdffSuD8";

        [TestMethod]
        public void TestMethod1()
        {

            //Companies House Service
            var chService = new CompaniesHouseService(apiUrl, apiKey, "OC353223");

            chService.GetCompanyMembers(false);

            Assert.IsTrue(chService.CompanyMembers.items.Any(i => i.officer_role == "llp-designated-member"));

        }
    }
    
}
