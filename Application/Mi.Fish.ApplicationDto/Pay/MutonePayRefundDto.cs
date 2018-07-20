using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public  class MutonePayRefundDto
    {

		public string result { get; set; }
		public string message { get; set; }
		public string time { get; set; }
		public string out_trade_no { get; set; }
		public string data { get; set; }
		public MutoneData mutondata { get; set; }

	}
}
