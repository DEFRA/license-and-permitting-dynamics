namespace Defra.Lp.Core.CompaniesHouse.OSPlaces
{
    using System.Runtime.Serialization;

    [DataContract]
    public class OSPlacesDPA
    {
        [DataMember(Name = "DPA")]
        public OSPlacesAddress Result { get; set; }
    }
}
