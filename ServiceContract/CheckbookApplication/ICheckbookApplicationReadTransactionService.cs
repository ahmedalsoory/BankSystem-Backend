using DTOs.Checkbook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContract.CheckbookApplication
{
    public interface ICheckbookApplicationReadTransactionService
    {
        Task<CheckbookDataForAddCheckbook> GetDataForAddCheckbook(int ApplicationID);
    }
}
