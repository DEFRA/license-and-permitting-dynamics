namespace Defra.Lp.Core.Workflows.CompaniesHouse
{
    using System;
    using System.Text;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.IO;
    using Base;

    public class CompaniesHouseService : APIServiceBase
    {
        protected readonly string CompanyRegistrationNumber;

        public CompaniesHouseCompany Company;

        public CompaniesHouseResultsDirectors Directors;

        public CompaniesHouseService(string TargetURL, string APIKey, string CompanyRegistrationNumber)
            :base(TargetURL, APIKey)
        {
            this.CompanyRegistrationNumber = CompanyRegistrationNumber;
        }

        

        public void GetCompany()
        {
            using (this._httpclient)
            {
                string URL = string.Format("{0}/company/{1}", this.TargetURL, this.CompanyRegistrationNumber);

                var response = this._httpclient.GetAsync(URL).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    //Deserialise
                    this.Company = new CompaniesHouseCompany();

                    //Set up the object...  
                    MemoryStream stream1 = new MemoryStream();

                    StreamWriter writer = new StreamWriter(stream1);
                    writer.Write(responseString);
                    writer.Flush();
                    stream1.Position = 0;

                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(CompaniesHouseCompany));
                    this.Company = (CompaniesHouseCompany)ser.ReadObject(stream1);
                }
            }
        }

        public void GetRegisteredAddress()
        {
            using (this._httpclient)
            {
                string URL = string.Format("{0}/company/{1}/registered-office-address", this.TargetURL, this.CompanyRegistrationNumber);

                var response = this._httpclient.GetAsync(URL).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    //Deserialise
                    this.Company.registered_office_address = new CompaniesHouseAddress();

                    //Set up the object...  
                    MemoryStream stream1 = new MemoryStream();

                    StreamWriter writer = new StreamWriter(stream1);
                    writer.Write(responseString);
                    writer.Flush();
                    stream1.Position = 0;

                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(CompaniesHouseAddress));
                    this.Company.registered_office_address = (CompaniesHouseAddress)ser.ReadObject(stream1);
                }
            }
        }

        public void GetCompanyDirectors()
        {
            using (this._httpclient)
            {
                string URL = string.Format("{0}/company/{1}/officers?register_type=directors", this.TargetURL, this.CompanyRegistrationNumber);

                var response = this._httpclient.GetAsync(URL).Result;

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    //Deserialise
                    this.Directors = new CompaniesHouseResultsDirectors();

                    //Set up the object...  
                    MemoryStream stream1 = new MemoryStream();

                    StreamWriter writer = new StreamWriter(stream1);
                    writer.Write(responseString);
                    writer.Flush();
                    stream1.Position = 0;

                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(CompaniesHouseResultsDirectors));
                    this.Directors = (CompaniesHouseResultsDirectors)ser.ReadObject(stream1);
                }
            }
        }
    }
}
