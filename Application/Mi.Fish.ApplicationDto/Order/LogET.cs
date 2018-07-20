using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{

    /// <summary>
    /// 日志实体类
    /// </summary>
    public class LogET
    {

        /// <summary>
        /// 操作人
        /// </summary>
        public string casher_no
        {
            set;get;
        }

        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime oper_date
        {
            set; get;
        }

        /// <summary>
        /// 授权人
        /// </summary>
        public string power_man
        {
            set; get;
        }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string oper_type
        {
            set; get;
        }

        /// <summary>
        /// 操作单号
        /// </summary>
        public string dj_no
        {
            set; get;
        }


        /// <summary>
        /// 机号
        /// </summary>
        public string jh
        {
            set; get;
        }


        /// <summary>
        /// 操作内容
        /// </summary>
        public string oper_text
        {
            set; get;
        }

    }
}
