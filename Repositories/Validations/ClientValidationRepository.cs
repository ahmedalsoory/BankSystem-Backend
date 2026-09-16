
using DTOs.interfaces;
using Shared.Interfaces;
using Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.Person.interfaces;

namespace Repositories.Validations
{
    //public class ClientValidationRepository : BaseRepository, IValidationService<IHasClient>
    //{
    //    private readonly IValidationService<IPersonValidtionDTO> _personValidationService;

    //    public ClientValidationRepository(IDbConnectionProvider connection, Context error
    //        , IValidationService<IPersonValidtionDTO> validationService)
    //        : base(connection, error) {
    //        _personValidationService = validationService;
        
    //    }


    //    public async Task<List<string>> ValidateAsync(IHasClient dto, int? id = null)
    //    {
    //        return await _personValidationService.ValidateAsync(dto, id);
    //    }
    //}
}
