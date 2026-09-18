using DTOs.AccountApplications;
using DTOs.interfaces;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RepositoryContracts;
using RepositoryContracts.AccountApplications;
using RepositoryContracts.AccountApplicationsType;
using RepositoryContracts.AccountProducts;
using Service.Apps;
using Service.Banking;
using Service.Core;
using Service.Orchestrators.AccountApplication;
using Service.Orchestrators.AccountOpening;
using Service.Validation;
using Service.Validation.Account;
using Service.Validation.AccountApplication;
using Service.Validation.AccountOpeningApplication;
using Service.Validation.AccountOpeningDetail;
using Service.Validation.ApplicationTypes;
using Service.Validation.CardApplication;
using Service.Validation.Client;
using Service.Validation.Person;
using Service.Validation.Transaction;
using ServiceContract;
using ServiceContract.Account;
using ServiceContract.AccountApplicationsType;
using ServiceContract.AccountOpeningApplications;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.AccountOpeningDetails;
using ServiceContract.AccountWorkflow;
using ServiceContract.Applications;
using ServiceContract.Applications.Orchestrators;
using ServiceContract.ApplicationWorkflowStep;
using ServiceContract.Card;
using ServiceContract.CardApplication;
using ServiceContract.Checkbook;
using ServiceContract.CheckbookApplication;
using ServiceContract.Client;
using ServiceContract.CSVExportService;
using ServiceContract.Dashboard;
using ServiceContract.IAccountProducts;
using ServiceContract.IOnboardingTypeSteps;
using ServiceContract.OnboardingApplicationTypes;
using ServiceContract.Person;
using ServiceContract.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Extension_Method
{
    public static class ServiceDependencyInjection
    {

        static void AddOrchestrators(IServiceCollection services)
        {
            services.AddScoped<IAccountOpeningOrchestrator, AccountOpeningOrchestrator>();
            services.AddScoped<IAccountApplicationAddNewApplicationOrchestrators, AccountApplicationAddNewApplicationOrchestrators>();
            services.AddScoped<IAccountApplicationUpdateStatusOrchestrators, AccountApplicationUpdateStatusOrchestrators>();
        }
        static void AddValidtion(IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<PersonAddValidator>();
            services.AddValidatorsFromAssemblyContaining<PersonUpdateValidator>();

          //  services.AddValidatorsFromAssemblyContaining<ClientAddValidator>();
           // services.AddValidatorsFromAssemblyContaining<ClientUpdateValidator>();

            services.AddValidatorsFromAssemblyContaining<AccountAddValidator>();
            
            services.AddValidatorsFromAssemblyContaining<ApplicationTypeUpdateRequestValidator>();

            services.AddValidatorsFromAssemblyContaining<AccountApplicationPagedRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<AccountApplicationAddValidator>();
            services.AddValidatorsFromAssemblyContaining<AccountApplicationStatusUpdateValidator>();

            
            services.AddValidatorsFromAssemblyContaining<AccountOpeningApplicationAddRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<AccountOpeningPagedRequestValidator>();


            services.AddValidatorsFromAssemblyContaining<AccountOpeningDetailPagedRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<AccountOpeningDetailRequestValidator>();

            services.AddValidatorsFromAssemblyContaining<CardApplicationAddRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CardReplaceApplicationAddRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<CardReplaceApplicationAddRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<InternationalCardAddRequestValidator>();

            services.AddValidatorsFromAssemblyContaining<DepositRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<TransactionBaseRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<TransferRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<WithdrawRequestValidator>();
        }
        public static IServiceCollection AddBankService(this IServiceCollection services)
        {
            // --- Single Contract Services ---
            services.AddScoped<IPersonWriteService, PersonService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IAccountProductsReadTransactionalService, AccountProductsService>();
            services.AddScoped<IOnboardingTypeStepsReadService, OnboardingTypeStepsService>();
            services.AddScoped<IOnboardingApplicationTypesReadServcie, OnboardingApplicationTypesServcie>();
            services.AddScoped<IApplicationWorkflowStepReadService, ApplicationWorkflowStepService>();
            services.AddScoped<ICardApplicationWriteService, CardApplicationService>();
            services.AddScoped<ICSVExportService, CSVExportService>();
            services.AddScoped<ICheckbookWriteService, CheckbookService>();
            //CheckbookService :BaseService<CheckbookService> ,ICheckbookWriteService

            // --- Multi-Contract Services (Explicitly Forwarded to Share the Same Instance) ---
            services.AddScoped<CheckbookApplicationService>();
            services.AddScoped<ICheckbookApplicationWriteService>(sp => sp.GetRequiredService<CheckbookApplicationService>());
            services.AddScoped<ICheckbookApplicationReadTransactionService>(sp => sp.GetRequiredService<CheckbookApplicationService>());

            // AccountService
            services.AddScoped<AccountService>();
            services.AddScoped<IAccountWriteService>(sp => sp.GetRequiredService<AccountService>());
            services.AddScoped<IAccountReadService>(sp => sp.GetRequiredService<AccountService>());

            // TransactionService
            services.AddScoped<TransactionService>();
            services.AddScoped<ITransactionWriteService>(sp => sp.GetRequiredService<TransactionService>());
            services.AddScoped<ITransactionReadService>(sp => sp.GetRequiredService<TransactionService>());

            // ClientService
            services.AddScoped<ClientService>();
            services.AddScoped<IClientWriteService>(sp => sp.GetRequiredService<ClientService>());
            services.AddScoped<IClientReadService>(sp => sp.GetRequiredService<ClientService>());

            // AccountApplicationService
            services.AddScoped<AccountApplicationService>();
            services.AddScoped<IAccountApplicationReadService>(sp => sp.GetRequiredService<AccountApplicationService>());
            services.AddScoped<IAccountApplicationWriteService>(sp => sp.GetRequiredService<AccountApplicationService>());
            services.AddScoped<IAccountApplicationReadTransactionService>(sp => sp.GetRequiredService<AccountApplicationService>());

            // ApplicationTypeService
            services.AddScoped<ApplicationTypeService>();
            services.AddScoped<IApplicationTypeReadService>(sp => sp.GetRequiredService<ApplicationTypeService>());
            services.AddScoped<IApplicationTypeWriteService>(sp => sp.GetRequiredService<ApplicationTypeService>());
            services.AddScoped<IApplicationTypeReadTransactionService>(sp => sp.GetRequiredService<ApplicationTypeService>());

            // AccountOpeningApplicationsService
            services.AddScoped<AccountOpeningApplicationsService>();
            services.AddScoped<IAccountOpeningApplicationsWriteService>(sp => sp.GetRequiredService<AccountOpeningApplicationsService>());
            services.AddScoped<IAccountOpeningApplicationsReadService>(sp => sp.GetRequiredService<AccountOpeningApplicationsService>());
            services.AddScoped<IAccountOpeningApplicationsReadTransactionService>(sp => sp.GetRequiredService<AccountOpeningApplicationsService>());

            // AccountOpeningDetailsService
            services.AddScoped<AccountOpeningDetailsService>();
            services.AddScoped<IAccountOpeningDetailsReadService>(sp => sp.GetRequiredService<AccountOpeningDetailsService>());
            services.AddScoped<IAccountOpeningDetailsWriteService>(sp => sp.GetRequiredService<AccountOpeningDetailsService>());

            // AccountWorkflowService
            services.AddScoped<AccountWorkflowService>();
            services.AddScoped<IAccountWorkflowWriteService>(sp => sp.GetRequiredService<AccountWorkflowService>());
            services.AddScoped<IAccountWorkflowReadService>(sp => sp.GetRequiredService<AccountWorkflowService>());

            // CardService
            services.AddScoped<CardService>();
            services.AddScoped<ICardWriteService>(sp => sp.GetRequiredService<CardService>());
            services.AddScoped<ICardReadTransactionService>(sp => sp.GetRequiredService<CardService>());


            // --- Helpers & Helpers/Orchestrators ---
            services.AddSingleton<CardSecurityHelper>();

            AddOrchestrators(services);
            AddValidtion(services);

            return services;
        }
    }
}
