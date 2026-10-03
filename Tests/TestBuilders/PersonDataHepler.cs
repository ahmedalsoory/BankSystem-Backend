using Dapper;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Tests.TestBuilders
{
    public static class PersonDataHelper
    {
        public static async Task<PersonData?> GetPersonDataAsync()
        {
            using var connection = new SqlConnection(ConnectionString.Connection);

            const string sql = "SELECT TOP 1 Phone, Email, NationalId FROM Core.Persons";

            return await connection.QueryFirstOrDefaultAsync<PersonData>(sql);
        }
    }

    public class PersonData
    {
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
    }
}