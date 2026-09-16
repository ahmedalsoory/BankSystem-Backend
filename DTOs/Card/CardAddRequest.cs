using Shared.Enums.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Card
{
    public class CardAddRequest
    {
        public int AccountID { get; set; }
        public int ApplicationID { get; set; }
        public byte CardTypeID { get; set; }

        // Security & Cryptographic fields generated automatically by backend
        public string CardNumberHash { get; set; } = string.Empty;
        public string MaskedCardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public DateTime ExpirationDate { get; set; }
        public string CVVHash { get; set; } = string.Empty;
        public byte[] PinHash { get; set; } = Array.Empty<byte>();

        // Settings & Limits
        public bool IsInternationalEnabled { get; set; } = false;
        public CardStatus Status { get; set; } = CardStatus.PendingApproval;
        public decimal DailyWithdrawalLimit { get; set; }
        public decimal DailyOnlinePurchaseLimit { get; set; }

    }
}
