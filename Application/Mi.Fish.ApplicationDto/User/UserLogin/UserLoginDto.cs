using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class UserLoginDto
    {
        public string cashierid { get; set; }

        public string cashierpw { get; set; }

        public decimal startamount { get; set; }

        public int banci { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class UserLoginOutPut
    {
        /// <summary>
        /// 密钥
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string UserName { get; set; }
    }

}
