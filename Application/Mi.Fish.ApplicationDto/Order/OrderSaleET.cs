using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 销售信息实体类
    /// </summary>
    public class OrderSaleET
    {
        public string flow_no { get; set; }
        public string flow_id { get; set; }
        public string branch_no { get; set; }
        public string item_no { get; set; }
        public string source_price { get; set; }
        public string sale_price { get; set; }
        public string sale_qnty { get; set; }
        public string source_qnty { get; set; }
        public string sale_money { get; set; }
        public string cost_price { get; set; }
        public string sell_way { get; set; }
        public string oper_id { get; set; }
        /// <summary>
        /// 打折人
        /// </summary>
        public string other1 { get; set; }
        /// <summary>
        /// 条形码
        /// </summary>
        public string item_subno { get; set; }
        /// <summary>
        /// 特殊类型
        /// </summary>
        public string special_type { get; set; }
        public string jh { get; set; }
        public string work_group { get; set; }
        public string deal_no { get; set; }
        public string pay_way { get; set; }
        public string coin_no { get; set; }
        public decimal coin_rate { get; set; }
        public decimal total_amount { get; set; }
        public decimal old_amount { get; set; }
        /// <summary>
        /// 没有库存是否可以销售，0为不允许，1,2为允许
        /// </summary>
        public int tjstorage { get; set; }
        public string parentid { get; set; }
        public string returnsflowno { get; set; }
        public string returnsflowid { get; set; }
        public string deskno { get; set; }
        public int isfast { get; set; }
        /// <summary>
        /// 是否管理库存(具体到单品)'1'是,'0'否
        /// </summary>
        public string have_stocklist { get; set; }
        public decimal CreditCardMoney { get; set; }
        public string CreditCardNo { get; set; }
        public string CreditCardRemark { get; set; }
        public string CreditCardType { get; set; }
        public string icno { get; set; }
        public int consumetype { get; set; }
        public decimal fangtemoney { get; set; }
        /// <summary>
        /// 团餐ic消费类型(0服务中心，1餐饮，2水吧，3礼品店，4酒店)
        /// </summary>
        public int fangteisintegral { get; set; }
        public string card_no { get; set; }
        public decimal fangterate { get; set; }
        public decimal goldcardmoney { get; set; }
        public string vip_id { get; set; }
        public string tvolumelist { get; set; }
        public decimal tvolumeamount { get; set; }
        public decimal zhaolin_amount { get; set; }
        public decimal rmbmoney { get; set; }
        public string ticketcode { get; set; }
        public decimal wxmoney { get; set; }
        public string wxtradeno { get; set; }
        public decimal almoney { get; set; }
        public string altradeno { get; set; }
        public decimal mutonmoney { get; set; }
        public string mutontradeno { get; set; }
        public decimal vouchermoney { get; set; }
        public decimal mutonedisAmount { get; set; }
        public decimal mutonerate { get; set; }
        public string mutoneorder { get; set; }
        public decimal leyoumoney { get; set; }
        public decimal wxwaimoney { get; set; }
        public decimal aliwaimoney { get; set; }
        public decimal leyouscanMoney { get; set; }
    }
}
