using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Mi.Fish.Infrastructure.Session
{
    public class FishClaimTypes
    {
        public const string StorageNo = "http://schemas.restaurant.org/ws/2018/07/identity/claims/storageNo";

        public const string TerminalId = "http://schemas.restaurant.org/ws/2018/07/identity/claims/terminalId";
    }
}
