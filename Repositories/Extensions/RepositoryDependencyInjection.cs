using DTOs.Account;
using DTOs.Account.interfaces;
using DTOs.AccountApplications.interfaces;
using DTOs.AccountOpeningApplication.interfaces;
using DTOs.AccountOpeningDetail.interfaces;
using DTOs.AccountOpeningWorkflow.interfaces;
using DTOs.CardApplication.interfaces;
using DTOs.interfaces;
using DTOs.Person.interfaces;
using DTOs.Transaction.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Apps;
using Repositories.Banking;
using Repositories.Core;
using Repositories.Validations;
using RepositoryContracts;
using RepositoryContracts.AccountApplications;
using RepositoryContracts.AccountApplicationsType;
using RepositoryContracts.AccountOpeningApplications;
using RepositoryContracts.AccountOpeningDetail;
using RepositoryContracts.AccountProducts;
using RepositoryContracts.AccountRepo;
using RepositoryContracts.AccountWorkflowRepository;
using RepositoryContracts.ApplicationWorkflowStep;
using RepositoryContracts.Card;
using RepositoryContracts.CardApplication;
using RepositoryContracts.Checkbook;
using RepositoryContracts.CheckbookApplication;
using RepositoryContracts.ClientRepo;
using RepositoryContracts.DashboardRepo;
using RepositoryContracts.ExportRepository;
using RepositoryContracts.OnboardingApplicationTypes;
using RepositoryContracts.OnboardingTypeSteps;
using RepositoryContracts.PersonRepo;
using RepositoryContracts.Transaction;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Extensions
{
    public static  class RepositoryDependencyInjection
    {

        static void AddValdtionRepositories(IServiceCollection services)
        {
            services.AddScoped<IValidationService<IPersonValidtionDTO>, PersonValidationRepository>();
            services.AddScoped<IValidationService<IAccountValidtionDTO>, AccountValidationRepository>();
            services.AddScoped<IValidationService<IAccountApplicationValidationDTO>, AccountApplicationValidationRepository>();


            services.AddScoped<IValidationService<IWorkflowValidationDTO>, WorkflowValidationRepository>();
            services.AddScoped<IValidationService<IAccountOpeningValidationDTO>, 
                AccountOpeningApplicationsValidationRepository>();

            services.AddScoped< IValidationService<IAccountOpeningDetailValidtionDTO>,
            AccountOpeningDetailValidtion> ();

            services.AddScoped<IValidationService<ITransactionValidationDTO>,
        TransactionValidationRepository>();

            services.AddScoped<IValidationService<ICardApplicationValidation>,
         CardApplicationValidationRepository>();
       
        }

        public static IServiceCollection AddBankRepositories(this IServiceCollection services)
        {
            // --- Single Contract Repositories ---
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            services.AddScoped<IAccountProductsReadTransactionalRepository, AccountProductsRepository>();
            services.AddScoped<IOnboardingTypeStepsReadRepository, OnboardingTypeStepsRepository>();
            services.AddScoped<IOnboardingApplicationTypesReadRepository, OnboardingApplicationTypesRepository>();
            services.AddScoped<IApplicationWorkflowStepReadRepository, ApplicationWorkflowStepRepository>();
            services.AddScoped<ICardApplicationWriteRepository, CardApplicationRepository>();
            services.AddScoped<IExportRepository, ExportRepository>();


            // --- Multi-Contract Repositories (Explicitly Forwarded to Share the Same Instance) ---

            services.AddScoped<CheckbookApplicationRepository>();
            services.AddScoped<ICheckbookApplicationWriteRepository>(sp => sp.GetRequiredService<CheckbookApplicationRepository>());
            services.AddScoped<ICheckbookApplicationReadTransactionRepository>(sp => sp.GetRequiredService<CheckbookApplicationRepository>());

            // PersonRepository
            services.AddScoped<PersonRepository>();
            services.AddScoped<IPersonWriteRepository>(sp => sp.GetRequiredService<PersonRepository>());
            services.AddScoped<IPersonReadTransactionRepository>(sp => sp.GetRequiredService<PersonRepository>());

            // AccountRepository
            services.AddScoped<AccountRepository>();
            services.AddScoped<IAccountReadRepository>(sp => sp.GetRequiredService<AccountRepository>());
            services.AddScoped<IAccountWriteRepository>(sp => sp.GetRequiredService<AccountRepository>());
            services.AddScoped<IAccountLockRepository>(sp => sp.GetRequiredService<AccountRepository>());

            // TransactionRepository
            services.AddScoped<TransactionRepository>();
            services.AddScoped<ITransactionWriteRepository>(sp => sp.GetRequiredService<TransactionRepository>());
            services.AddScoped<ITransactionReadRepository>(sp => sp.GetRequiredService<TransactionRepository>());

            // ClientRepository
            services.AddScoped<ClientRepository>();
            services.AddScoped<IClientReadRepository>(sp => sp.GetRequiredService<ClientRepository>());
            services.AddScoped<IClientWriteRepository>(sp => sp.GetRequiredService<ClientRepository>());

            // AccountApplicationRepository
            services.AddScoped<AccountApplicationRepository>();
            services.AddScoped<IAccountApplicationReadRepository>(sp => sp.GetRequiredService<AccountApplicationRepository>());
            services.AddScoped<IAccountApplicationWriteRepository>(sp => sp.GetRequiredService<AccountApplicationRepository>());
            services.AddScoped<IAccountApplicationReadTransactionRepository>(sp => sp.GetRequiredService<AccountApplicationRepository>());

            // ApplicationTypeRepository
            services.AddScoped<ApplicationTypeRepository>();
            services.AddScoped<IApplicationTypeReadRepository>(sp => sp.GetRequiredService<ApplicationTypeRepository>());
            services.AddScoped<IApplicationTypeWriteRepository>(sp => sp.GetRequiredService<ApplicationTypeRepository>());
            services.AddScoped<IApplicationTypeReadTransactionRepository>(sp => sp.GetRequiredService<ApplicationTypeRepository>());

            // AccountOpeningApplicationsRepository
            services.AddScoped<AccountOpeningApplicationsRepository>();
            services.AddScoped<IAccountOpeningApplicationsWriteRepository>(sp => sp.GetRequiredService<AccountOpeningApplicationsRepository>());
            services.AddScoped<IAccountOpeningApplicationsReadTransactionRepository>(sp => sp.GetRequiredService<AccountOpeningApplicationsRepository>());
            services.AddScoped<IAccountOpeningApplicationsReadRepository>(sp => sp.GetRequiredService<AccountOpeningApplicationsRepository>());

            // AccountOpeningDetailsRepository
            services.AddScoped<AccountOpeningDetailsRepository>();
            services.AddScoped<IAccountOpeningDetailsReadRepository>(sp => sp.GetRequiredService<AccountOpeningDetailsRepository>());
            services.AddScoped<IAccountOpeningDetailsWriteRepository>(sp => sp.GetRequiredService<AccountOpeningDetailsRepository>());

            // AccountWorkflowRepository
            services.AddScoped<AccountWorkflowRepository>();
            services.AddScoped<IAccountWorkflowWriteRepository>(sp => sp.GetRequiredService<AccountWorkflowRepository>());
            services.AddScoped<IAccountWorkflowReadRepository>(sp => sp.GetRequiredService<AccountWorkflowRepository>());

            // CardRepository
            services.AddScoped<CardRepository>();
            services.AddScoped<ICardReadTransactionRepository>(sp => sp.GetRequiredService<CardRepository>());
            services.AddScoped<ICardWriteRepository>(sp => sp.GetRequiredService<CardRepository>());

            // CardRepository
            services.AddScoped<CheckbookRepository>();
            services.AddScoped<ICheckbookWriteRepository>(sp => sp.GetRequiredService<CheckbookRepository>());


            // --- Validation Repositories ---
            AddValdtionRepositories(services);

            return services;
        }
    }
}
