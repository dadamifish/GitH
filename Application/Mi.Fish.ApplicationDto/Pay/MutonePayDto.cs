using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{

	/// <summary>
	/// 秒通支付结果Dto
	/// </summary>
    public class MutonePayDto
    {
		public string result { get; set; }

		public string message { get; set; }

		public MutoneData data { get; set; }

	}


	public class MutoneData
	{
		public string username { get; set; }
		public string usercode { get; set; }
		public string mtorderno { get; set; }

		public decimal amount { get; set; }
		public decimal discount { get; set; }
		public decimal payamount { get; set; }
		public string sncode { get; set; }
                       
        public decimal couponamount { get; set; }

		public decimal cyDiscount { get; set; }

	}

}
