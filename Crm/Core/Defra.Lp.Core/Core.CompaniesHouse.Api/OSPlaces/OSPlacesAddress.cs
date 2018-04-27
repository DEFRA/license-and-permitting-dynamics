namespace Defra.Lp.Core.CompaniesHouse.OSPlaces
{
    using System.Runtime.Serialization;

    [DataContract]
    public class OSPlacesAddress
    {
        [DataMember]
        public string UPRN;

        [DataMember]
        public string UDPRN;

        [DataMember]
        public string ADDRESS;

        [DataMember]
        public string SUB_BUILDING_NAME;

        [DataMember]
        public string BUILDING_NAME;

        [DataMember]
        public string THOROUGHFARE_NAME;

        [DataMember]
        public string POST_TOWN;

        [DataMember]
        public string POSTCODE;

        [DataMember]
        public string X_COORDINATE;

        [DataMember]
        public string Y_COORDINATE;
    }
}
