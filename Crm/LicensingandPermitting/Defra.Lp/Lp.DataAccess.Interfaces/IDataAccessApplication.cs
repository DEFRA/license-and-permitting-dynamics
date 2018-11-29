using System;
using Microsoft.Xrm.Sdk;

namespace Lp.DataAccess.Interfaces
{
    public interface IDataAccessApplication
    {
        Entity GetApplication(Guid applicationId);
    }
}