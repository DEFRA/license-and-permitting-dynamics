namespace Defra.Lp.Core.Workflows.OSPlaces
{
    using System.Runtime.Serialization;

    [DataContract]
    public class OSPlacesResults
    {
        [DataMember(Name = "results")]
        public OSPlacesDPA[] DPA { get; set; }
    }
}
