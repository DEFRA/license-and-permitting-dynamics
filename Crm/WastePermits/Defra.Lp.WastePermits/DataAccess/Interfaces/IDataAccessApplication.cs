using System;
using Microsoft.Xrm.Sdk;

namespace WastePermits.DataAccess
{
    public interface IDataAccessApplication
    {
        Entity GetApplication(Guid applicationId);
    }
}