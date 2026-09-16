using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.AccountOpeningDetail.interfaces
{
    public interface IAccountOpeningDetailValidtionDTO
    {
        public int ApplicationID { get; set; }


        public string RequirementKey { get; set; }
    }
}
