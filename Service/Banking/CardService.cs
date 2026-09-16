using DTOs.Card;
using Microsoft.Extensions.Logging;
using RepositoryContracts.Card;
using ServiceContract.Card;
using Shared;
using Shared.Enums.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Banking
{
    public class CardService :BaseService<CardService>, ICardWriteService
        , ICardReadTransactionService
    {

        private readonly CardSecurityHelper _securityHelper;
        private readonly ICardReadTransactionRepository _read;
        private readonly ICardWriteRepository _write;
        public CardService(CardSecurityHelper cardSecurityHelper
            , ICardReadTransactionRepository read, ICardWriteRepository write,
            ILogger<CardService> logger) : base(logger)
        {
            _write = write;
            _read = read;
            _securityHelper = cardSecurityHelper;
        }

        public async Task<OperationResult<int>> RenewCardAsync(int oldCardId, int applicationId)
        {
 

            // Update old card status to Cancelled first
            var updateResult = await _write.UpdateStatus(oldCardId, CardStatus.Cancelled);
            if (!updateResult.Success)
            {
                return OperationResult<int>.Failure("Failed to update old card status for renewal.");
            }

            // Then generate and insert the new card
            return await GenerateAndInsertCardInternalAsync(applicationId);
        }

        // 3. PUBLIC: Replace an existing card
        public async Task<OperationResult<int>> ReplaceCardAsync(int oldCardId, int applicationId)
        {
            

            // Update old card status to Cancelled first
            var updateResult = await _write.UpdateStatus(oldCardId, CardStatus.Cancelled);
            if (!updateResult.Success)
            {
                return OperationResult<int>.Failure("Failed to update old card status for replacement.");
            }
            
          

            return await GenerateAndInsertCardInternalAsync(applicationId);
        }

        public async Task<OperationResult<int>> IssueCardAsync(int applicationId)
        {
            return await GenerateAndInsertCardInternalAsync(applicationId);
        }
        private async Task<OperationResult<int>> GenerateAndInsertCardInternalAsync(int applicationId)
        {
            // 1. Fetch application details (AccountID, CardTypeID, Customer Name)
            var appDetails = await _read.GetApplicationDetailsForCardCreationAsync(applicationId);
            if (appDetails == null)
            {
                return  OperationResult<int>.Failure("card application deails do not exisy");
            }

            // 2. Generate secure card data using your helper
            string rawCardNumber = _securityHelper.GenerateRandomCardNumber();
            string maskedNumber = _securityHelper.MaskCardNumber(rawCardNumber);
            string cardHash = _securityHelper.ComputeHashedValue(rawCardNumber);

            string rawCvv = _securityHelper.GenerateRandomCVV();
            string cvvHash = _securityHelper.ComputeHashedValue(rawCvv);

            // Example pin hash (you can set a default initial PIN like "0000")
            string pinHashHex = _securityHelper.ComputeHashedValue("0000");
            byte[] pinHashBytes = Encoding.UTF8.GetBytes(pinHashHex); // matches varbinary(64) if needed

            // 3. Build the request object for the repository
            var newCard = new CardAddRequest
            {
                AccountID = appDetails.AccountID,
                ApplicationID = applicationId,
                CardTypeID = appDetails.CardTypeID,
                CardNumberHash = cardHash,
                MaskedCardNumber = maskedNumber,
                CardHolderName = appDetails.ClientFullName,
                ExpirationDate = DateTime.UtcNow.AddYears(3), // Standard 3-year expiry
                CVVHash = cvvHash,
                PinHash = pinHashBytes,
                IsInternationalEnabled = false,
                Status = CardStatus.PendingApproval, // Pending or default status based on your rules
                DailyWithdrawalLimit = 5000.00m,
                DailyOnlinePurchaseLimit = 2000.00m
            };

            // 4. Save to database
            return await _write.AddCard(newCard);
        }

        public async Task<int?> GetCardIdByApplicationIdAsync(int applicationId)
        {
            return await _read.GetCardIdByApplicationIdAsync(applicationId);
        }
    }
}
