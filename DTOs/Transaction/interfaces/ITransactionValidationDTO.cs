using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Transaction.interfaces
{
    public interface ITransactionValidationDTO
    {
        public decimal Amount { get; set; }
        public int FromAccountId {  get; set; }
    }
}
