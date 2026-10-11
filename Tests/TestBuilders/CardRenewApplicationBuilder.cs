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
    public class CardRenewApplicationBuilder
    {
        protected readonly CardRenewApplicationAddRequest _request = new();

        public CardRenewApplicationBuilder()
        {
            _request.CreatedByUserID = 1;
            _request.Notes = "Automated card renewal test";
        }

        public CardRenewApplicationBuilder(AccountApplicationAddRequest baseRequest)
        {
            _request.AccountID = baseRequest.AccountID;
            _request.CreatedByUserID = baseRequest.CreatedByUserID;
            _request.Notes = baseRequest.Notes;
        }

        public CardRenewApplicationBuilder ForAccount(int accountId)
        {
            _request.AccountID = accountId;
            return this;
        }

        public CardRenewApplicationBuilder WithNotes(string notes)
        {
            _request.Notes = notes;
            return this;
        }

        public CardRenewApplicationBuilder WithOldCardId(int oldCardId)
        {
            _request.oldCardId = oldCardId;
            return this;
        }

        public async Task<OperationResult<int>> BuildAsync(ICardApplicationWriteService cardApplicationWriteService)
        {
            return await cardApplicationWriteService.AddRenewApplicationAsync(_request);
        }
    }
}
