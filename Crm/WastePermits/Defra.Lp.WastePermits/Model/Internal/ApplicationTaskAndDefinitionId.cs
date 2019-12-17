namespace WastePermits.Model.Internal
{
    using System;

    /// <summary>
    /// Class returns an application task/definition id pair
    /// </summary>
    public class ApplicationTaskAndDefinitionId
    {
        public Guid ApplicationTaskId { get; set; }
        public Guid ApplicationTaskDefinitionId { get; set; }
    }
}
