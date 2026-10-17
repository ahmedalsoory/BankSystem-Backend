using DTOs.CheckbookApplications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.Checkbook
{
    public interface ICheckbookReadTransactionRepository
    {
        Task<CheckbookApplicationDetails> GetDetailsForIssueFirstTime(int applicationId);
        Task<CheckbookApplicationDetails> GetDetailsForReplacementOrRenew(int applicationId);
    }
}
