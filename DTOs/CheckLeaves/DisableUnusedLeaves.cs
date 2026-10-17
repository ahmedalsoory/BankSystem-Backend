using Shared.Enums.CheckLeaf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.CheckLeaves
{
    public class DisableUnusedLeaves
    {
        public int OldCheckbookId { get; set; }
        public CheckLeafStatus DisabledStatus { get; set; }
        public CheckLeafStatus ActiveStatus { get; set; }
    }
}
