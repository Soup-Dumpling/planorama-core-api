using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planorama.User.Core.UseCases.PrivacySetting.GetPrivacySetting
{
    public class GetPrivacySettingViewModel
    {
        public Guid UserId { get; set; }
        public bool IsPrivate { get; set; }
    }
}
