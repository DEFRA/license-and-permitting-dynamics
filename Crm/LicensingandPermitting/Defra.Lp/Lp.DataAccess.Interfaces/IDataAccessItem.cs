using System;
using Microsoft.Xrm.Sdk;

namespace Lp.DataAccess.Interfaces
{
    public interface IDataAccessItem
    {
        EntityCollection GetAssessmentsForActivity(Guid activity);
    }
}