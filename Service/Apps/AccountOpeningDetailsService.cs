using DTOs;
using DTOs.AccountOpeningApplication.interfaces;
using DTOs.AccountOpeningDetail;
using DTOs.AccountOpeningDetail.interfaces;
using DTOs.interfaces;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using RepositoryContracts.AccountOpeningDetail;
using ServiceContract.Account;
using ServiceContract.AccountOpeningApplications.Orchestrators;
using ServiceContract.AccountOpeningDetails;
using ServiceContract.AccountWorkflow;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;

namespace Service.Apps
{
    public class AccountOpeningDetailsService : BaseService<AccountOpeningDetailsService>,
        IAccountOpeningDetailsWriteService
         , IAccountOpeningDetailsReadService
    {

        private readonly IAccountOpeningDetailsReadRepository _read;
        private readonly IAccountOpeningDetailsWriteRepository _write;

        private readonly IValidationService<IAccountOpeningDetailValidtionDTO> _validationService;



        public AccountOpeningDetailsService(IAccountOpeningDetailsReadRepository read,
           IAccountOpeningDetailsWriteRepository write
            , IValidationService<IAccountOpeningDetailValidtionDTO> validationService,
           ILogger<AccountOpeningDetailsService> logger) : base(logger)
        {

            _read = read;
            _write = write;
            _validationService = validationService;

        }



        public async Task<OperationResult> AddDetailAsync(AccountOpeningDetailRequest request)
        {
            List<string> errors = await _validationService.ValidateAsync(request);
            if (errors.Any())
            {
                return OperationResult.Failure(errors);
            }

            return await ExecuteDbOperationAsync(() => _write.AddDetailAsync(request
               ), "A record with this RequirementKey already exists.");
        }

        public async Task<IEnumerable<AccountOpeningDetailResponse>> GetDetailsByApplicationIdAsync(int applicationId)
        {
            return await _read.GetDetailsByApplicationIdAsync(applicationId);
        }


        public async Task<PagedResult<AccountOpeningDetailListItem>> GetDetailsPagedAsync
            (AccountOpeningDetailPagedRequest request)
        {
            return await _read.GetDetailsPagedAsync(request);
        }

        public async Task<OperationResult> RejectDetailAsync(int detailId, string reason)
        {
            return await _write.RejectDetailAsync(detailId, reason);
        }
        public async Task<OperationResult> VerifiedStatusAsync(VerifiedStatusRequest request)
        {



            return await _write.VerifyDetailAsync(request.DetailID);
            


        }


    }
}
