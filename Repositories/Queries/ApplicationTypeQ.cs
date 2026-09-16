using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class ApplicationTypeQ
    {
        public const string GetById = @"
                SELECT ApplicationTypeID, TypeName, Description, ApplicationFees
                FROM Apps.ApplicationTypes
                WHERE ApplicationTypeID = @id;";

        public const string GetAll = @"
                SELECT ApplicationTypeID, TypeName, Description, ApplicationFees
                FROM Apps.ApplicationTypes
                ORDER BY ApplicationTypeID ASC;";

        public const string Update = @"
                UPDATE Apps.ApplicationTypes
                SET Description = @Description,
                    ApplicationFees = @ApplicationFees
                WHERE ApplicationTypeID = @ApplicationTypeID;";
    }
}
