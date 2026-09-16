using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries.QueriesValiditon
{
    public static class PersonValidationQ
    {
        public const string CheckConflicts = @"
        SELECT 'NationalId' FROM Core.Persons
        WHERE NationalId = @NationalId AND (@PersonId IS NULL OR Id != @PersonId)
        UNION ALL
        SELECT 'Email' FROM Core.Persons
        WHERE Email = @Email AND (@PersonId IS NULL OR Id != @PersonId)
        UNION ALL
        SELECT 'Phone' FROM Core.Persons
        WHERE Phone = @Phone AND (@PersonId IS NULL OR Id != @PersonId)";
    }
}
