using DTOs.AccountApplications;
using DTOs.CardApplication;
using ServiceContract.CardApplication;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestBuilders
{
    public class IssueCardFirstTimeApplicationBuilder
    {
        protected readonly CardApplicationAddRequest _request = new();

        public IssueCardFirstTimeApplicationBuilder()
        {
            _request.CreatedByUserID = 1;
            _request.Notes = "Automated card application test";
        }

        // Constructor to map base request properties over to the card request
        public IssueCardFirstTimeApplicationBuilder(AccountApplicationAddRequest baseRequest)
        {

            _request.AccountID = baseRequest.AccountID;
            _request.CreatedByUserID = baseRequest.CreatedByUserID;
            _request.Notes = baseRequest.Notes;
        }

        public IssueCardFirstTimeApplicationBuilder ForAccount(int accountId)
        {
            _request.AccountID = accountId;
            return this;
        }

        public IssueCardFirstTimeApplicationBuilder WithNotes(string notes)
        {
            _request.Notes = notes;
            return this;
        }

        public IssueCardFirstTimeApplicationBuilder WithCardType(byte cardTypeId)
        {
            _request.CardTypeID = cardTypeId;
            return this;
        }

        public async Task<OperationResult<int>> BuildAsync(ICardApplicationWriteService cardApplicationWriteService)
        {
            return await cardApplicationWriteService.AddNewCardApplicationAsync(_request);
        }
    }
}
