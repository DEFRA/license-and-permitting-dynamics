namespace Defra.Lp.Core.CompaniesHouse.OSPlaces
{
    using Base;
    using System.Net.Http;
    using CompaniesHouse;
    using System.IO;
    using System.Runtime.Serialization.Json;

    public class OSPlacesService : APIServiceBase
    {
        public OSPlacesService(string TargetURL, string APIKey)
            :base(TargetURL, APIKey)
        {
        }

        public OSPlacesAddress MatchCompaniesHouseAddress(CompaniesHouseAddress companiesHouseAddress)
        {
            int maxresults = 1;

            string matchedAddress = string.Format("{0}, {1}, {2}, {3}", companiesHouseAddress.address_line_1, companiesHouseAddress.address_line_2, companiesHouseAddress.locality, companiesHouseAddress.postal_code);

            string URL = string.Format("{0}/places/v1/addresses/find?query={1}&maxresults={2}&key={3}", this.TargetURL, matchedAddress, maxresults, this.APIKey);

            var response = this._httpclient.GetAsync(URL).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;

                //Deserialise
                OSPlacesResults OSPlacesResults = new OSPlacesResults();

                //Set up the object...  
                MemoryStream stream1 = new MemoryStream();

                StreamWriter writer = new StreamWriter(stream1);
                writer.Write(responseString);
                writer.Flush();
                stream1.Position = 0;

                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(OSPlacesResults));
                OSPlacesResults = (OSPlacesResults)ser.ReadObject(stream1);

                if (OSPlacesResults != null && OSPlacesResults.DPA != null && OSPlacesResults.DPA.Length > 0)
                    return OSPlacesResults.DPA[0].Result;
            }

            return null;
        }

    }
}
