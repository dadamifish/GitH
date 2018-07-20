using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    /// <summary>
    /// 获取秒通四位码输入参数
    /// </summary>
    public class GetMutoneSnCodePayInput
    {

        /// <summary>
        /// 支付总金额
        /// </summary>
        [Required]
        public decimal PayMoney { get; set; }

        /// <summary>
        /// 商品名称集合[格式如： 冰红茶X1;绿茶X2]
        /// </summary>
        [Required]
        public string GoodsName { get; set; }


    }
}
