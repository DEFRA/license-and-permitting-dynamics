namespace Defra.Lp.Core.CompaniesHouse.OSPlaces
{
    using System.Runtime.Serialization;

    [DataContract]
    public class OSPlacesResults
    {
        [DataMember(Name = "results")]
        public OSPlacesDPA[] DPA { get; set; }
    }
}
