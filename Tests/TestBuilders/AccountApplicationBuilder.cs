using DTOs.AccountApplications;
using DTOs.CardApplication;
using ServiceContract.CardApplication;
using Shared;
using System;
using System.Threading.Tasks;

namespace Tests.TestBuilders
{
    // Base Account Application Builder
    public class AccountApplicationBuilder
    {
        protected readonly AccountApplicationAddRequest _request = new();

        public AccountApplicationBuilder()
        {
            _request.CreatedByUserID = 1;
            _request.Notes = "Automated integration test application";
        }

        // Constructor to accept a base request if bridged from somewhere else
        public AccountApplicationBuilder(AccountApplicationAddRequest request)
        {
            _request = request;
        }

        public AccountApplicationBuilder ForAccount(int accountId)
        {
            _request.AccountID = accountId;
            return this;
        }

        public AccountApplicationBuilder WithNotes(string notes)
        {
            _request.Notes = notes;
            return this;
        }

        public AccountApplicationBuilder WithUser(int userId)
        {
            _request.CreatedByUserID = userId;
            return this;
        }

        // Internal exposure so extension methods can grab the request object
        internal AccountApplicationAddRequest GetRequest() => _request;
    }

    // 1. Card Application Builder
   

   
}