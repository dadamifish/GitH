using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{

    /// <summary>
    /// 方特支付结果Dto
    /// </summary>
    public class FtPayDto
    {
        public string result { get; set; }

        public string message { get; set; }

        public FtData data { get; set; }

    }


    public class FtData
    {
        public string username { get; set; }
        public string usercode { get; set; }
        public string mtorderno { get; set; }

        public decimal amount { get; set; }
        public string discount { get; set; }
        public decimal payamount { get; set; }
        public string sncode { get; set; }

        public decimal couponamount { get; set; }

        public decimal cyDiscount { get; set; }

    }

}
