using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Entities
{
 
    public class GlobalSetting
    {
        public int Id { get; set; }
        public bool IsSalaryDayActive { get; set; }
        public DateTime LastUpdated { get; set; }
        public byte MaxAccountsPerClient {  get; set; }
        public int? UpdatedByUserId { get; set; }
        public virtual User? UpdatedByUser { get; set; }
    }
}
