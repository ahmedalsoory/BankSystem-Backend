using DTOs.Checkbook;
using DTOs.CheckbookApplications;
using DTOs.CheckLeaves;
using Microsoft.Extensions.Logging;
using RepositoryContracts.Checkbook;
using RepositoryContracts.CheckLeaves;
using ServiceContract.Checkbook;
using Shared;
using Shared.Enums.Checkbook;
using Shared.Enums.CheckLeaf;
using System.Threading.Tasks;

namespace Service.Banking
{
    public class CheckbookService : BaseService<CheckbookService>, ICheckbookWriteService
    {
        private readonly ICheckbookWriteRepository _checkbookWriteRepo;
        private readonly ICheckbookReadTransactionRepository _checkbookReadRepo;
        private readonly ICheckLeavesWriteRepository _checkLeavesWriteRepo;

        public CheckbookService(
            ICheckbookWriteRepository checkbookWriteRepository,
            ICheckbookReadTransactionRepository checkbookReadRepository,
            ICheckLeavesWriteRepository checkLeavesWriteRepository,
            ILogger<CheckbookService> logger) : base(logger)
        {
            _checkbookWriteRepo = checkbookWriteRepository;
            _checkbookReadRepo = checkbookReadRepository;
            _checkLeavesWriteRepo = checkLeavesWriteRepository;
        }

        // 1. Issue a brand new checkbook (First time)
        public async Task<OperationResult<int>> IssueCheckbookAsync(int applicationId)
        {
            return await GenerateAndInsertCheckbookInternalAsync(applicationId, isFirstIssue: true);
        }

        // 2. Renew an existing checkbook
        public async Task<OperationResult<int>> RenewCheckbookAsync(int oldCheckbookId, int applicationId)
        {
            // Update old checkbook status to Cancelled/Inactive first
            var updateResult = await _checkbookWriteRepo.UpdateStatus(oldCheckbookId, CheckbookStatus.Cancelled); // 2 = Cancelled/Inactive
            if (!updateResult.Success)
            {
                return OperationResult<int>.Failure("Failed to update old checkbook status for renewal.");
            }

            // Generate the new checkbook and leaves
            return await GenerateAndInsertCheckbookInternalAsync(applicationId, isFirstIssue: false);
        }

        // 3. Replace a lost, stolen, or damaged checkbook
        public async Task<OperationResult<int>> ReplaceCheckbookAsync(int oldCheckbookId, int applicationId)
        {
            // Update old checkbook status to Cancelled first
            var updateResult = await _checkbookWriteRepo.UpdateStatus(oldCheckbookId, CheckbookStatus.Cancelled);
            if (!updateResult.Success)
            {
                return OperationResult<int>.Failure("Failed to update old checkbook status for replacement.");
            }
            var checkLeafUpdate= await _checkLeavesWriteRepo.DisableUnusedLeavesAsync(new DisableUnusedLeaves()
            {
                ActiveStatus = CheckLeafStatus.Active,
                DisabledStatus = CheckLeafStatus.Disabled,
                OldCheckbookId = oldCheckbookId,

            });

            if (!checkLeafUpdate.Success)
                return OperationResult<int>.Failure("Failed to update old checkbook status for replacement.");



            // Generate the new checkbook and leaves
            return await GenerateAndInsertCheckbookInternalAsync(applicationId, isFirstIssue: false);
        }

        // Private Internal Orchestrator (Handles Header + Recursive CTE Check Leaves Generation)
        private async Task<OperationResult<int>> GenerateAndInsertCheckbookInternalAsync(int applicationId, bool isFirstIssue = true)
        {
            // 1. Fetch application details (AccountID, BeginCheckNumber, NumberOfLeaves, etc.)
            CheckbookApplicationDetails appDetails;
            if (isFirstIssue)
                appDetails = await _checkbookReadRepo.GetDetailsForIssueFirstTime(applicationId);
            else
                appDetails = await _checkbookReadRepo.GetDetailsForReplacementOrRenew(applicationId);

            if (appDetails == null)
            {
                return OperationResult<int>.Failure("Checkbook application details do not exist.");
            }

            // 2. Build the Checkbook Header request
            var checkbookRequest = new CheckbookAddRequest
            {
                AccountID = appDetails.AccountID,
                ApplicationID = applicationId,
                NumberOfLeaves = appDetails.NumberOfLeaves
            };

            // 3. Insert Checkbook Header and get the new CheckbookID back
            var checkbookResult = await _checkbookWriteRepo.Add(checkbookRequest);
            if (!checkbookResult.Success)
            {
                return OperationResult<int>.Failure(checkbookResult.Errors);
            }

            int newCheckbookId = checkbookResult.Data;

            // 4. Build the AddCheckLeafs DTO for the recursive CTE generator
            var leavesRequest = new AddCheckLeafs
            {
                CheckbookID = newCheckbookId,
                AccountID = appDetails.AccountID,
                BeginCheckNumber = appDetails.BeginCheckNumber,
                NumberOfLeaves = appDetails.NumberOfLeaves,
                Status = 1 // 1 = Available / Active leaf status
            };

            // 5. Generate the check leaves batch atomically
            var leavesResult = await _checkLeavesWriteRepo.AddRangeAsync(leavesRequest);
            if (!leavesResult.Success)
            {
                return OperationResult<int>.Failure(leavesResult.Errors);
            }

            return OperationResult<int>.Ok(newCheckbookId);
        }
    }
}