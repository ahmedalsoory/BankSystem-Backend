using DTOs.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.Card
{
    public interface ICardReadTransactionRepository
    {
        Task<CardApplicationDetails> GetDetailsForReplacementOrRenew(int applicationId);
        Task<CardApplicationDetails> GetDetailsForIssueFirstTime(int applicationId);
        Task<int?> GetCardIdByApplicationIdAsync(int applicationId);
    }
}
