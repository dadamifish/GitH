using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class OrderSaleInfoTmp
    {

       /// <summary>
       /// 仓库编号
       /// </summary>
        public string storeno { get; set; }

        /// <summary>
        /// 新订单号
        /// </summary>
        public string receiptid { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string cashierno { get; set; }

        /// <summary>
        /// 销售方式
        /// </summary>
        public string saleway { get; set; }

        /// <summary>
        /// 条形码
        /// </summary>
        public string barcodes { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public string qtys { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public string prices { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public string goodsids { get; set; }

        /// <summary>
        /// IC卡编号
        /// </summary>
        public string icno { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public string rates { get; set; }


    }
}
