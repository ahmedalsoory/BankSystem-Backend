using DTOs.GlobalSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryContracts.GlobalSettingRepo
{
    public interface IGlobalSettingReadRepository
    {
        Task<GlobalSetting?> GetCurrentSettingsAsync();
    }
}
