using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{ 
    /// <summary>
    /// 秒通支付查询Dto
    /// </summary>
    public class MutonPayQueryDto
    {
        public string result { get; set; }
        public string message { get; set; }
        public List<MutonPayQueryData> data { get; set; }
    }

    public class MutonPayQueryData
    {
        public string username { get; set; }
        public string usercode { get; set; }
        public string mtorderno { get; set; }
        public string orderno { get; set; }
        public decimal amount { get; set; }
        public decimal? discount { get; set; }
        public decimal payamount { get; set; }

        public decimal couponamount { get; set; }

    }
}
