using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CardApplication.interfaces
{
    public interface ICardApplicationValidation
    {
        int AccountID { get; set; }
        int? oldCardId { get; set; }
        byte CardTypeID {  get; set; }

    }
}
