using Abp.AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Mi.Fish.ApplicationDto
{

    /// <summary>
    /// 返回参数
    /// </summary>
    public class MenuResultDto
    {
        /// <summary>
        /// 返回结果状态
        /// </summary>
        public bool Result { get; set; }

        /// <summary>
        /// 返回结果提示信息
        /// </summary>
        public string Message { get; set; }

    }

    #region enum

    /// <summary>
    /// 更新商品数量操作类型
    /// </summary>
    public enum UpdateGoodsCountType
    {
        /// <summary>
        /// 设置
        /// </summary>
        [Display(Name = "设置")]
        Set,

        /// <summary>
        /// 加减
        /// </summary>
        [Display(Name = "加减")]
        Plus,
    }

    #endregion

    #region input

    /// <summary>
    /// 商品参数输入
    /// </summary>
    public class SaleMenuInput
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [Required(ErrorMessage = "商品编号不能为空")]
        public string GoodsNo { get; set; }
    }

    /// <summary>
    /// 商品数量修改输入
    /// </summary>
    [AutoMap(typeof(SaleMenuInput))]
    public class UpdateGoodsCountInput
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [Required(ErrorMessage = "商品编号不能为空")]
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品购买数量
        /// </summary>
        [RegularExpression(@"^([1-9][0-9]*|-[1-9][0-9]*)$", ErrorMessage = "数量格式有误，请输入非零数量")]
        public int Qty { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public UpdateGoodsCountType Type { get; set; }
    }

    /// <summary>
    /// 单项打折参数输入
    /// </summary>
    public class SingleDiscountInput
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        [Required(ErrorMessage = "商品编号不能为空")]
        public string GoodsNo { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        [Range(1, 100, ErrorMessage = "折扣应在1到100之间")]
        [Required(ErrorMessage = "折扣不能为空")]
        public int Discount { get; set; }
    }

    /// <summary>
    /// 全部打折参数输入
    /// </summary>
    public class AllDiscountInput
    {
        /// <summary>
        /// 折扣
        /// </summary>
        [Range(1, 100, ErrorMessage = "折扣应在1到100之间")]
        [Required(ErrorMessage = "折扣不能为空")]
        public int Discount { get; set; }
    }

    /// <summary>
    /// 价格折扣策略输入
    /// </summary>
    public class DiscountProcInput
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
        public string saleway { get; set; } = "0";

        /// <summary>
        /// 条形码
        /// </summary>
        public string barcodes { get; set; } = "";

        /// <summary>
        /// 数量
        /// </summary>
        public string qtys { get; set; } = "";

        /// <summary>
        /// 价格
        /// </summary>
        public string prices { get; set; } = "";

        /// <summary>
        /// 产品编号
        /// </summary>
        public string goodsids { get; set; } = "";

        /// <summary>
        /// IC卡编号
        /// </summary>
        public string icno { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public string rates { get; set; } = "";
    }

    #endregion

    #region dto

    /// <summary>
    /// 购物车商品返回结果
    /// </summary>
    public class GoodsMenuDto
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 销售单价
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Qty { get; set; } = 1;

        /// <summary>
        /// 折扣
        /// </summary>
        [Range(1, 100, ErrorMessage = "折扣应在1到100之间")]
        public int Discount { get; set; } = 100;

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayAmount
        {
            get
            {
                return (Qty * SalePrice * Discount) / 100;
            }
        }

        /// <summary>
        /// 套餐子项
        /// </summary>
        public List<MealSubDto> MealSubs { get; set; }

    }

    /// <summary>
    /// 购物车返回结果
    /// </summary>
    public class SaleMenuDto
    {
        /// <summary>
        /// 菜单栏购买商品集合
        /// </summary>
        public List<GoodsMenuDto> Menus { get; set; } = new List<GoodsMenuDto>();

        /// <summary>
        /// 折扣(打折金额)
        /// </summary>
        public decimal DiscountAmount { get; set; }


        /// <summary>
        /// 总计(折扣后金额)
        /// </summary>
        public decimal PayAmount
        {
            get
            {
                return SaleAmount - DiscountAmount;
            }
        }

        /// <summary>
        /// 小计(折扣前金额)
        /// </summary>
        public decimal SaleAmount
        {
            get
            {
                return Menus.Sum(w => w.SalePrice * w.Qty);
            }
        }
    }

    /// <summary>
    /// 套餐子项返回结果
    /// </summary>
    public class MealSubDto
    {

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Qty { get; set; } = 1;

        /// <summary>
        /// 折扣
        /// </summary>
        [Range(1, 100, ErrorMessage = "折扣应在1到100之间")]
        public int Discount { get; set; } = 100;

        /// <summary>
        /// 销售单价
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayAmount
        {
            get { return (Qty * SalePrice * Discount) / 100; }
        }

        /// <summary>
        /// 父级商品编号
        /// </summary>
        public string MasterNo { get; set; }

    }

    /// <summary>
    /// 折扣数据输出
    /// </summary>
    public class GetGoodsTotalPriceDto
    {
        /// <summary>
        /// 折扣(折扣金额)
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// 总计(应付金额)
        /// </summary>
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 小计(销售金额)
        /// </summary>
        public decimal SaleAmount { get; set; }

        /// <summary>
        /// 未付金额
        /// </summary>
        public decimal UnpaidAmount { get; set; }

        /// <summary>
        /// 购买商品总数量
        /// </summary>
        public int TotalQty { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CreateOrderNoDto
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
    }

    #endregion

    #region cache

    /// <summary>
    /// 购物车商品参数
    /// </summary>
    [AutoMap(typeof(GoodsMenuDto),typeof(GoodsInfo))]
    public class GoodsMenu
    {

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Qty { get; set; } = 1;

        /// <summary>
        /// 商品类型
        /// S：普通商品
        /// C：组合商品
        /// </summary>
        public string GoodsType { get; set; }

        /// <summary>
        /// 分类
        /// </summary>
        public string CategoryType { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        [Range(1, 100, ErrorMessage = "折扣应在1到100之间")]
        public int Discount { get; set; } = 100;

        /// <summary>
        /// 销售单价
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 默认单价
        /// </summary>
        public decimal DefaultPrice { get; set; }

        /// <summary>
        /// 成本单价
        /// </summary>
        public int CostPrice { get; set; }

        /// <summary>
        /// 条形码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayAmount
        {
            get { return (Qty * SalePrice * Discount) / 100; }
        }

        /// <summary>
        /// 套餐子项
        /// </summary>
        public List<MealSub> MealSubs { get; set; }
    }

    /// <summary>
    /// 购物车参数
    /// </summary>
    [AutoMap(typeof(SaleMenuDto))]
    public class SaleMenuCache
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 菜单栏购买商品集合
        /// </summary>
        public List<GoodsMenu> Menus { get; set; } = new List<GoodsMenu>();

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal DiscountAmount { get; set; }


        /// <summary>
        /// 总计金额(折扣后金额)
        /// </summary>
        public decimal PayAmount
        {
            get
            {
                return SaleAmount - DiscountAmount;
            }
        }

        /// <summary>
        /// 小计金额(折扣前金额)
        /// </summary>
        public decimal SaleAmount
        {
            get
            {
                return Menus.Sum(w => w.SalePrice * w.Qty);
            }
        }
    }


    /// <summary>
    /// 套餐子项参数
    /// </summary>
    [AutoMap(typeof(MealSubDto), typeof(MealSubInfo))]
    public class MealSub
    {

        /// <summary>
        /// 商品编号
        /// </summary>
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        public int Qty { get; set; } = 1;

        /// <summary>
        /// 套餐产品配有数量
        /// </summary>
        public int MealQty { get; set; } = 1;

        /// <summary>
        /// 折扣
        /// </summary>
        [Range(1, 100, ErrorMessage = "折扣应在1到100之间")]
        public int Discount { get; set; } = 100;

        /// <summary>
        /// 销售单价
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 默认单价
        /// </summary>
        public decimal DefaultPrice { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal PayAmount
        {
            get { return (Qty * SalePrice * Discount) / 100; }
        }

        /// <summary>
        /// 父级商品编号
        /// </summary>
        public string MasterNo { get; set; }

    }

    /// <summary>
    /// 销售总额
    /// </summary>
    public class ActualMenu
    {

        /// <summary>
        /// 销售总额
        /// </summary>
        public decimal SaleAmount { get; set; }
    }

    /// <summary>
    /// 分类
    /// </summary>
    public class GoodsCategory
    {
        
        /// <summary>
        /// 一级分类名称
        /// </summary>
        public string CategoryDesc { get; set; }
    }

    #endregion
}
