using Dapper;
using DTOs.Client;
using Microsoft.Data.SqlClient;
using NATS.Client;
using Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Client.helper
{
    public static class TestDataHelper
    {
        
            public static async Task<ClientDetailDto?> GetClientDetailDirectlyAsync(
                int personId,
                IDbConnectionProvider connectionProvider)
            {
                const string query = @"
                SELECT c.PersonId, p.FirstName, p.LastName, p.Phone, p.Email, p.BirthDate,
                       p.Gendor, p.ImagePath, p.NationalId, c.ClientNumber, c.IsActive, 
                       c.RiskLevel, c.JoinedDate as JoinDate, c.RowVersion as ClientVersion, 
                       p.RowVersion as PersonVersion
                FROM Core.Persons p 
                INNER JOIN Core.Clients c ON c.PersonId = p.Id
                WHERE p.Id = @Id";


            var connection = await connectionProvider.GetConnectionAsync();

                return await connection.QueryFirstOrDefaultAsync<ClientDetailDto>(query, new { Id = personId },
                    connectionProvider.CurrentTransaction);
            }
        }
    }

