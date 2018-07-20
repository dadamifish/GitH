using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Mi.Fish.ApplicationDto
{
    public class AddAppGoodsInputDto
    {
        /// <summary>
        /// 商品编码
        /// </summary>
        [Required(ErrorMessage ="商品编码不能为空")]
        public string MealId { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        [Required(ErrorMessage ="商品名称不能为空")]
        public string MealName { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        public double Price { get; set; }
    }
    
    public class AddAppGoodsToAppInputDto : AddAppGoodsInputDto
    {
        /// <summary>
        /// 门店编码
        /// </summary>
        public string StoreId { get; set; }
    }
}
