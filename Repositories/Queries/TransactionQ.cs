using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class TransactionQ
    {
        public const string UpdateBalance = @"
                UPDATE Banking.Accounts WITH (UPDLOCK, ROWLOCK)
                SET Balance = Balance + @Amount 
                WHERE AccountID = @Id";

        public const string WithdrawBalance = @"
                UPDATE Banking.Accounts WITH (UPDLOCK, ROWLOCK)
                SET Balance = Balance - @Amount 
                WHERE AccountID = @Id AND Balance >= @Amount";

        public const string LogTransaction = @"
    INSERT INTO [Banking].[Transactions] (
        [TransactionDate], [Amount], [FromAccountId], [ToAccountId], 
        [ExecutedByUserId], [SourceId], [Type], [SourceType]
    ) 
    OUTPUT INSERTED.Id
    VALUES (
        GETDATE(), @Amount, 
        @FromAccountId, @ToAccountId, @UserId, @SourceId, @Type, @SourceType
    )";
    }
}
