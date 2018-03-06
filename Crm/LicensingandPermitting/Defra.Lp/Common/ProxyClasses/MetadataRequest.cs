namespace Defra.Lp.Common.ProxyClasses
{
    public class MetadataRequest
    {
        public MetadataRequest()
        {
            this.Metadata = new MetadataValues();
        }

        public string RecordURL { get; internal set; }

        public MetadataValues Metadata { get; internal set; }
    }
}