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
        private string apiKey = "R63PBUpJhDM5_GrZ0XDN0Fe2aksMjSJU7EDj6DJ4";

        [TestMethod]
        public void TestMethod1()
        {

            //Companies House Service
            var chService = new CompaniesHouseService(apiUrl, apiKey, "OC391037");
            chService.GetCompanyDirectorsAndPartners();

            chService.GetCompanyMembers(false);

            Assert.IsTrue(chService.CompanyMembers.items.Any(i => i.officer_role == "llp-designated-member"));

        }
    }
    
}
