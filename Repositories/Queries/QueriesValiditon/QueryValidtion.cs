using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries.QueriesValiditon
{
    public static class QueryValidtion
    {
        public static class AccountApplicationValidation
        {
            public const string CheckPendingApplication = AccountApplicationValidationQ.CheckPendingApplication;
        }
        public static class AccountOpeningValidation
        {
            public const string CheckActiveApplications = AccountOpeningValidationQ.CheckActiveApplications;
        }
        public static class AccountOpeningDetailValidation
        {
            public const string CheckActiveDetail = AccountOpeningDetailQ.CheckActiveDetail;
        }
        public static class AccountValidation
        {
            public const string CountAccountsForClient = AccountValidationQ.CountAccountsForClient;
        }
        public static class PersonValidation
        {
            public const string CheckConflicts = PersonValidationQ.CheckConflicts;
        }
        public static class WorkflowValidation
        {
            public const string GetStepOrderIndex = WorkflowValidationQ.GetStepOrderIndex;
            public const string CheckForBlockingSteps = WorkflowValidationQ.CheckForBlockingSteps;
        }

        public static class CardApplicationValidtion
        {
            public const string IsCardOwnedByAccount = CardApplicationValidationQ.IsCardOwnedByAccount;
            public const string IsBalanceSufficient = CardApplicationValidationQ.IsBalanceSufficient;
            public const string HasActiveCardOfType = CardApplicationValidationQ.HasActiveCardOfType;
        }
        public static class TransactionValidation
        {
            public const string IsBalanceHasWithdrawAmount = TransactionValidationQ.IsBalanceHasWithdrawAmount;
        }


    }
}
