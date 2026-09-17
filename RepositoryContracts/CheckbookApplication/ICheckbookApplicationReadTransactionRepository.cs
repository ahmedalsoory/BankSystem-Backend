using DTOs.Checkbook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.CheckbookApplication
{
    public interface ICheckbookApplicationReadTransactionRepository
    {
        Task<CheckbookDataForAddCheckbook> GetDataForAddCheckbook(int ApplicationID);
    }
}
