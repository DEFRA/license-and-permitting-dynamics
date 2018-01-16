namespace Defra.Lp.Core.Workflows.CompaniesHouse
{
    using Base;
    using Microsoft.Xrm.Sdk;
    using System.IO;
    using System.Runtime.Serialization.Json;

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
                else
                {
                    throw new InvalidPluginExecutionException(string.Format("{0} - {1}. Error calling {2}.", ((int)response.StatusCode).ToString(), response.StatusCode.ToString(), URL));
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

                    // Check for nulls
                    if (this.Company.registered_office_address.postal_code == null)
                    {
                        this.Company.registered_office_address.postal_code = string.Empty;
                    }
                    if (this.Company.registered_office_address.address_line_1 == null)
                    {
                        this.Company.registered_office_address.address_line_1 = string.Empty;
                    }
                    if (this.Company.registered_office_address.address_line_2 == null)
                    {
                        this.Company.registered_office_address.address_line_2 = string.Empty;
                    }
                    if (this.Company.registered_office_address.locality == null)
                    {
                        this.Company.registered_office_address.locality = string.Empty;
                    }
                }
            }
        }

        public void GetCompanyDirectors()
        {
            using (this._httpclient)
            {
                // Added register_view=true as docs say this is required to filter on directors. However, didn't seem
                // to work at the time so we need to check befre creating contacts that it is a director
                //string URL = string.Format("{0}/company/{1}/officers?register_type=directors&register_view=true", this.TargetURL, this.CompanyRegistrationNumber);
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