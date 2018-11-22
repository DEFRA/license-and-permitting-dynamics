using System;
using Microsoft.Xrm.Sdk;

namespace WastePermits.DataAccess.Interfaces
{
    public interface IDataAccessItem
    {
        EntityCollection GetAssessmentsForActivity(Guid activity);
    }
}