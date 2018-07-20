using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
	/// <summary>
	/// 支付相关数据缓存类
	/// </summary>
    public class PayInfo
    {
		public string AppInterface { get; set; }

		public string Webkey { get; set; }

		public string PayInterface { get; set; }

		public string MutonPayLink { get; set; }
		public string Parkid { get; set; }
		public string MutoneKey { get; set; }
		public string MutonePrice { get; set; }
		public string LeyouAccount { get; set; }
		public string LeyouPwd { get; set; }
		public string LeyouPayLink { get; set; }
		public string LeyouKey { get; set; }

		public string LeyouPrice { get; set; }

		public string StorageName { get; set; }

		public string StorageNo { get; set; }

		public string TerminalId { get; set; }

	}
}
