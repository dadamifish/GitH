using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class OrderDetail
    {
        public string dishno { get; set; }
        public string item_name { get; set; }
        public decimal sale_qnty { get; set; }
        public decimal orderprice { get; set; }
        public decimal sale_money { get; set; }
        public string item_subno { get; set; }
        public string bbarcode { get; set; }
        public decimal flow_id { get; set; }
        public decimal flow_no { get; set; }
        public string deskno { get; set; }
        public string vipno { get; set; }
        public DateTime Oper_date { get; set; }
        public decimal Totalamount { get; set; }
        /// <summary>
        /// 支付详情
        /// </summary>
        public List<OrderpayDetail> PaymentList { get; set; }
    }

    public class OrderpayDetail
    {
        /// <summary>
        /// 1,现金 2找零，8 礼券，84 代金券，120 微信，
        /// 130 支付宝，5 信用卡，112 秒通，113 方特扫码，105 方特
        /// </summary>
        public string pay_way { get; set; }
        public string sell_way { get; set; }
        public decimal old_amount { get; set; }
        public string thirdtradeno { get; set; }
        public string tradeno { get; set; }
        public decimal sale_amount { get; set; }
    }

    /// <summary>
    /// 订单详情
    /// </summary>
    public class OrderDetailOutput
    {
        /// <summary>
        /// 订单商品列表
        /// </summary>
        public List<OrderGoods> OrderGoodsList { get; set; } = new List<OrderGoods>();
        /// <summary>
        /// 订单号
        /// </summary>
        public string FlowNo { get; set; }
        /// <summary>
        /// 桌号
        /// </summary>
        public string DeskNo { get; set; }
        /// <summary>
        /// IC 卡号，已经不再使用
        /// </summary>
        public string VipNo { get; set; }
        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal Totalamount { get; set; }
        /// <summary>
        /// 支付明细
        /// </summary>
        public List<OrderPayDetailOutput> PaymentList { get; set; }
    }

    public class OrderGoods
    {
        /// <summary>
        /// 
        /// </summary>
        public string DishNo { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 商品数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public decimal GoodsPrice { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsID { get; set; }
        /// <summary>
        /// 订单中商品编号
        /// </summary>
        public string FlowId { get; set; }
    }

    /// <summary>
    /// 支付明细
    /// </summary>
    public class OrderPayDetailOutput
    {
        /// <summary>
        /// 支付类型
        /// 1,现金 2找零，8 礼券，84 代金券，120 微信，
        /// 130 支付宝，5 信用卡，112 秒通，113 方特扫码，105 方特 992 微信(外),993 支付宝(外)
        /// </summary>
        public PayType Payway { get; set; }
        /// <summary>
        /// 支付描述
        /// </summary>
        public string PayDescription { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Saleamount { get; set; }
    }
}
