using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class ClientQ
    {
        public const string GetByAccountNumber = @"
                SELECT Id, ClientNumber, RiskLevel, IsActive, RowVersion
                FROM Core.Clients 
                WHERE ClientNumber = @accountNumber";

        public const string GetByPersonId = @"
                SELECT c.PersonId, p.FirstName, p.LastName, p.Phone, p.Email, p.BirthDate,
                       p.Gendor, p.ImagePath, p.NationalId, c.ClientNumber, c.IsActive, 
                       c.RiskLevel, c.JoinedDate, c.RowVersion as ClientVersion, 
                       p.RowVersion as PersonVersion
                FROM Core.Persons p 
                INNER JOIN Core.Clients c ON c.PersonId = p.id
                WHERE p.Id = @Id";

        public const string Register = @"
                INSERT INTO Core.Clients (PersonId, ClientNumber, RiskLevel, IsActive, JoinedDate, CreatedByUserID)
                VALUES (@PersonId, @ClientNumber, @RiskLevel, @IsActive, GETDATE(), @CreatedByUserID);";

        public const string Update = @"
                UPDATE Core.Clients 
                SET RiskLevel = @RiskLevel, IsActive = @IsActive
                WHERE PersonID = @Id AND RowVersion = @ClientRowVersion;";
    }
}

