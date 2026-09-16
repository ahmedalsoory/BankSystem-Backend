using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class PersonQ
    {
        public const string Add = @"
                INSERT INTO Core.Persons (FirstName, LastName, NationalId, Email, Phone, BirthDate, ImagePath, Gendor, CreatedByUserID)
                OUTPUT inserted.Id, inserted.RowVersion
                VALUES (@FirstName, @LastName, @NationalId, @Email, @Phone, @BirthDate, @ImagePath, @Gendor, 1);";

        public const string GetById = @"
                SELECT Id, FirstName, LastName, NationalId, Email, Phone, BirthDate, 
                       CreatedAt, CreatedByUserID, RowVersion, Gendor, ImagePath
                FROM Core.Persons
                WHERE Id = @id";

        public const string GetByNationalId = @"
                SELECT Id, FirstName, LastName, NationalId, Email, Phone, BirthDate, 
                       CreatedAt, CreatedByUserID, RowVersion, Gendor, ImagePath
                FROM Core.Persons
                WHERE NationalId = @nationalId";

        public const string Update = @"
                UPDATE Core.Persons
                SET [FirstName] = @FirstName, [LastName] = @LastName, [NationalId] = @NationalId,
                    [Email] = @Email, [Phone] = @Phone, [BirthDate] = @BirthDate, 
                    [Gendor] = @Gendor, [ImagePath] = @ImagePath
                WHERE [Id] = @Id AND [RowVersion] = @PersonRowVersion";
    }
}
