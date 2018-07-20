using System;
using System.Collections.Generic;
using System.Text;

namespace Mi.Fish.ApplicationDto
{

	/// <summary>
	/// 支付结果返回集合
	/// </summary>
    public class PayOutput
    {

        /// <summary>
        /// 实际支付方式
        /// </summary>
        public PayType PayType { get; set; }

        /// <summary>
        /// 原支付金额
        /// </summary>

        public decimal PayMoney { get; set; }


		/// <summary>
		/// 实际支付金额
		/// </summary>

		public decimal PayAmount { get; set; }

		/// <summary>
		/// 红包金额
		/// </summary>

		public decimal CouponAmount { get; set; }

		/// <summary>
		/// 折扣金额(包括红包金额)
		/// </summary>

		public decimal RateAmount { get; set; }


		/// <summary>
		/// 折扣
		/// </summary>

		public decimal Rate { get; set; }

		/// <summary>
		/// 餐饮支付订单号
		/// </summary>

		public string PayOrderNo { get; set; }

		/// <summary>
		/// 第三方支付订单号
		/// </summary>

		public string TradeNo { get; set; }


	}
}
