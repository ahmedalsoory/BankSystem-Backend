using ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Service.Validation
{
    // T = The concrete DTO (e.g., ClientUpdateRequest)
    // TContract = The interface it implements (e.g., IPersonValidtionDTO)

    /*
    public class ValidationService<T, TContract> : IValidationService<T, TContract>
        where T : class, TContract
    {
        private readonly IValidationService<TContract>? _specializedRepo;
        private readonly IValidator<T>? _fluentValidator; // Optional injection

        public ValidationService(
            IValidationService<TContract>? specializedRepo,
            IValidator<T>? fluentValidator = null) // Nullable
        {
            _specializedRepo = specializedRepo;
            _fluentValidator = fluentValidator;
        }

        public async ValueTask<List<string>> ValidateAsync(T dto, int? id = null, IDbTransaction? transaction = null)
        {
            // Tier 1: Syntax (Fluent)
            if (dto != null && _fluentValidator != null)
            {
                var result = _fluentValidator.Validate(new ValidationContext<T>(dto));
                if (!result.IsValid)
                {
                    return result.Errors.Select(e => e.ErrorMessage).ToList();
                }
            }

            if (_specializedRepo != null)
            {
                // Tier 2: Infrastructure (ID & Concurrency)
                int? effectiveId = id ?? ((dto is IHasId hasId) ? hasId.Id : null);

                if (dto is IVersioned versioned && (versioned.RowVersion == null || versioned.RowVersion.Length == 0))
                {
                    return new List<string> { "Concurrency token is missing." };
                }

                // Tier 3: Domain (Database uniqueness)
                return await _specializedRepo.ValidateAsync(dto, effectiveId, transaction);
            }
            return new List<string>();
        }
    }

    */
}
