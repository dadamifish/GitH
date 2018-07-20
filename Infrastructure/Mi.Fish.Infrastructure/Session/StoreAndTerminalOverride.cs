using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.Infrastructure.Session
{
    public class StoreAndTerminalOverride
    {
        public string StorageNo { get; }

        public string TerminalId { get; }

        /// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
        public StoreAndTerminalOverride(string storageNo, string terminalId)
        {
            StorageNo = storageNo;
            TerminalId = terminalId;
        }
    }
}
