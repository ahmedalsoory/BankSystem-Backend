using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Queries
{
    public static class Query
    {
        public static class AccountApplication
        {
            public const string GetById = AccountApplicationQ.GetById;
            public const string Add = AccountApplicationQ.Add;
            public const string UpdateStatus = AccountApplicationQ.UpdateStatus;
            public const string GetApplicationTypeByIdAsync = AccountApplicationQ.GetApplicationTypeByIdAsync;
        }

        public static class AccountOpeningApplication
        {
            public const string AddApplication = AccountOpeningApplicationQ.AddApplication;
            public const string GetByID = AccountOpeningApplicationQ.GetByID;
            public const string UpdateStatus = AccountOpeningApplicationQ.UpdateStatus;
        }

        public static class AccountOpeningDetail
        {
            public const string GetByApplicationId = AccountOpeningDetailQ.GetByApplicationId;
            public const string Add = AccountOpeningDetailQ.Add;
            public const string Update = AccountOpeningDetailQ.Update;
            public const string UpdateStatus = AccountOpeningDetailQ.UpdateStatus;
        }
        public static class Account
        {
            public const string GetById = AccountQ.GetById;
            public const string Add = AccountQ.Add;
            public const string GetByNumber = AccountQ.GetByNumber;
            public const string GetByClientId = AccountQ.GetByClientId;
            public const string UpdateStatus = AccountQ.UpdateStatus;
            public const string LockAccountForTransfer = AccountQ.LockAccountForTransfer;
        }

        public static class AccountWorkflow
        {
            public const string AddWorkflowSteps = AccountWorkflowQ.AddWorkflowSteps;
            public const string CompleteStep = AccountWorkflowQ.CompleteStep;
            public const string CheckIncompleteSteps = AccountWorkflowQ.CheckIncompleteSteps;
        }
        public static class ApplicationType
        {
            public const string GetById = ApplicationTypeQ.GetById;
            public const string GetAll = ApplicationTypeQ.GetAll;
            public const string Update = ApplicationTypeQ.Update;
        }
        public static class Client
        {
            public const string GetByAccountNumber = ClientQ.GetByAccountNumber;
            public const string GetByPersonId = ClientQ.GetByPersonId;
            public const string Register = ClientQ.Register;
            public const string Update = ClientQ.Update;
        }
        public static class Dashboard
        {
            public const string GetHistoricalData = DashboardQ.GetHistoricalData;
            public const string GetSystemCounters = DashboardQ.GetSystemCounters;
            public const string AggregateYesterday = DashboardQ.AggregateYesterday;
        }
        public static class Person
        {
            public const string Add = PersonQ.Add;
            public const string Update = PersonQ.Update;
            public const string GetById = PersonQ.GetById;
            public const string GetByNationalId = PersonQ.GetByNationalId;
        }
        public static class Transaction
        {
            public const string UpdateBalance = TransactionQ.UpdateBalance;
            public const string WithdrawBalance = TransactionQ.WithdrawBalance;
            public const string LogTransaction = TransactionQ.LogTransaction;
        }
        public static class AccountProducts
        {
            public static string GetByID = AccountProductsQ.GetByID;
        }
        public static class OnboardingTypeSteps
        {
            public static string GetAll = OnboardingTypeStepsQ.GetAll;
        }
        public static class OnboardingApplicationTypes
        {
            public static string GetAll = OnboardingApplicationTypesQ.GetAll;
        }
        public static class CardApplication
        {
            public static string InsertNewCardApplication = CardApplicationQ.InsertNewCardApplication;
            public static string InsertReplacementApplication = CardApplicationQ.InsertReplacementApplication;
            public static string InsertRenewApplication = CardApplicationQ.InsertRenewApplication;
        }
        public static class CardType
        {
            public static string GetAll = CardTypeQ.GetAll;
        }
        public static class Card
        {
            public static  string Insert = CardQ.Insert;
            public static string GetDeatils = CardQ.GetDeatils;
            public static string UpdateStatus = CardQ.UpdateStatus;
            public static string GetCardIdByApplicationId = CardQ.GetCardIdByApplicationId;
        }
        public static class CheckbookApplication
        {
            public const string InsertNewCheckbookApplication = CheckbookApplicationQ.InsertNewCheckbookApplication;
            public const string InsertReplaceApplication = CheckbookApplicationQ.InsertReplaceApplication;
            public const string InsertRenewApplication = CheckbookApplicationQ.InsertRenewApplication;
            public const string GetDataForAddCheckbook = CheckbookApplicationQ.GetDataForAddCheckbook;
        }
        public static class Checkbook
        {
            public const string Insert = CheckbookQ.Insert;
            public const string getLastCheck = CheckbookQ.getLastCheck;
        }
    }
}
