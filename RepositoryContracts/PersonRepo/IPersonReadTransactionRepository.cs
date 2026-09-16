using DTOs.Person;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.PersonRepo
{
    public interface IPersonReadTransactionRepository
    {
        Task<PersonResponse?> GetByIdAsync_Transaction(int id);
    }
}
