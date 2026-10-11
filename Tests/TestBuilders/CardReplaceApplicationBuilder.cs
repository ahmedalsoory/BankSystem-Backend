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
    public class CardReplaceApplicationBuilder
    {
        protected readonly CardReplaceApplicationAddRequest _request = new();

        public CardReplaceApplicationBuilder()
        {
            _request.CreatedByUserID = 1;
            _request.Notes = "Automated card replacement test";
        }

        public CardReplaceApplicationBuilder(AccountApplicationAddRequest baseRequest)
        {
            _request.AccountID = baseRequest.AccountID;
            _request.CreatedByUserID = baseRequest.CreatedByUserID;
            _request.Notes = baseRequest.Notes;
        }

        public CardReplaceApplicationBuilder ForAccount(int accountId)
        {
            _request.AccountID = accountId;
            return this;
        }

        public CardReplaceApplicationBuilder WithNotes(string notes)
        {
            _request.Notes = notes;
            return this;
        }

        public CardReplaceApplicationBuilder WithOldCardId(int oldCardId)
        {
            _request.oldCardId = oldCardId;
            return this;
        }

        public async Task<OperationResult<int>> BuildAsync(ICardApplicationWriteService cardApplicationWriteService)
        {
            return await cardApplicationWriteService.AddReplacementApplicationAsync(_request);
        }
    }
}
