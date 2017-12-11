namespace Defra.Lp.Core.Workflows.OSPlaces
{
    using System.Runtime.Serialization;

    [DataContract]
    public class OSPlacesDPA
    {
        [DataMember(Name = "DPA")]
        public OSPlacesAddress Result { get; set; }
    }
}
