using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services;

namespace Mi.Fish.Application
{
    public interface ICookProvider : IApplicationService
    {
        CookSetting GetGetCookSetting(string key);
    }
}
