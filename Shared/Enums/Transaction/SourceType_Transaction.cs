using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums.Transaction
{
    /// <summary>
    /// Defines the specific origin or business category of a transaction.
    /// Used for audit logs, reporting, and linking transactions to external entities.
    /// </summary>
    public enum SourceType_Transaction : byte
    {
        /// <summary> Default/System-level operations, manual entries, or uncategorized events. </summary>
        GeneralSystem = 0,

        // --- Deposit Channels ---
        /// <summary> Money deposited via bank teller or cash deposit machine. </summary>
        CashDeposit = 1,
        /// <summary> Salary payments credited to the account. </summary>
        SalaryDeposit = 2,
        /// <summary> Incoming funds via domestic or international wire. </summary>
        WireTransfer = 3,
        /// <summary> Initial funds provided upon account opening. </summary>
        InitialDeposit = 4,

        // --- Lending & Credit ---
        /// <summary> Disbursement of funds from a loan contract. </summary>
        LoanDisbursement = 11,
        /// <summary> Payments made toward an existing loan balance. </summary>
        LoanRepayment = 12,
        /// <summary> Interest earned on savings or charged on debt. </summary>
        InterestAccrual = 13,

        // --- Fees & Charges ---
        /// <summary> Fees incurred during the account application process. </summary>
        AccountApplicationFee = 21,
        /// <summary> Penalty fees for exceeding account balance limits. </summary>
        OverdraftFee = 22,
        /// <summary> Penalty fees for missing a scheduled payment. </summary>
        LatePaymentFee = 23,

        // --- Withdrawal Channels ---
        /// <summary> Cash withdrawn from an ATM. </summary>
        AtmWithdrawal = 31,
        /// <summary> Purchases made via Point of Sale (POS) terminals. </summary>
        PosPurchase = 32,
        /// <summary> Payments made for utilities, services, or bills. </summary>
        BillPayment = 33,
        /// <summary> Costs associated with issuing new check leaves. </summary>
        CheckLeaves = 34
    }
}
