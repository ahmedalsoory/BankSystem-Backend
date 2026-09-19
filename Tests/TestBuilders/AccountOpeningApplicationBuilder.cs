using DTOs.AccountOpeningApplication;
using ServiceContract.AccountOpeningApplications;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.TestBuilders
{
    public class AccountOpeningApplicationBuilder
    {
        private readonly AccountOpeningApplicationAddRequest _request = new()
        {
            OnboardingTypeID = 2,          // Default onboarding template type
            CreatedByUserID = 1,           // Default system/admin user ID
            AccountType = 1,               // Default account type enum/byte value
            Currency = "USD",              // Default currency
            InitialDeposit = 1000.00f,     // Default starting deposit
            Notes = "Automated integration test application"
        };

        public AccountOpeningApplicationBuilder ForClient(int clientId)
        {
            _request.ClientID = clientId;
            return this;
        }

        public AccountOpeningApplicationBuilder WithOnboardingType(byte onboardingTypeId)
        {
            _request.OnboardingTypeID = onboardingTypeId;
            return this;
        }

        public AccountOpeningApplicationBuilder WithAccountType(byte accountType)
        {
            _request.AccountType = accountType;
            return this;
        }

        public AccountOpeningApplicationBuilder WithDeposit(float amount)
        {
            _request.InitialDeposit = amount;
            return this;
        }

        public AccountOpeningApplicationBuilder WithCurrency(string currency)
        {
            _request.Currency = currency;
            return this;
        }

        public AccountOpeningApplicationBuilder WithUser(int userId)
        {
            _request.CreatedByUserID = userId;
            return this;
        }

        public AccountOpeningApplicationBuilder WithNotes(string notes)
        {
            _request.Notes = notes;
            return this;
        }

        public async Task<OperationResult> BuildAsync(IAccountOpeningApplicationsWriteService writeService)
        {
            return await writeService.AddApplicationAsync(_request);
        }

        // Expose the raw request if you need to inspect or pass it elsewhere
        public AccountOpeningApplicationAddRequest BuildRequest() => _request;
    }
}
