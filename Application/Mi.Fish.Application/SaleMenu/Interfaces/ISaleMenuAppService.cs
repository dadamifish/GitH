using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi.Fish.Application.SaleMenu
{
    /// <summary>
    /// POS销售菜单接口
    /// </summary>
    public interface ISaleMenuAppService
    {
        /// <summary>
        /// 获取当前购物车数据
        /// </summary>
        /// <returns></returns>
        Task<Result<SaleMenuDto>> GetMenu();

        /// <summary>
        /// 新增商品栏
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<SaleMenuDto>> AddGoods(SaleMenuInput input);

        /// <summary>
        /// 修改商品购买数量
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<SaleMenuDto>> UpdateGoodsCount(UpdateGoodsCountInput input);

        /// <summary>
        /// 移除该商品
        /// </summary>
        /// <param name="goodsNo"></param>
        /// <returns></returns>
        Task<Result<SaleMenuDto>> RemoveGoods(string goodsNo);

        /// <summary>
        /// 清空购买商品
        /// </summary>
        /// <returns></returns>
        Task<Result<SaleMenuDto>> ClearMenu();

        /// <summary>
        /// 单项打折
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<SaleMenuDto>> SingleDiscount(SingleDiscountInput input);

        /// <summary>
        /// 全部打折
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<SaleMenuDto>> AllDiscount(AllDiscountInput input);

        /// <summary>
        /// 订单商品销售总额（不参与折扣）
        /// </summary>
        /// <returns></returns>
        Task<Result<ActualMenu>> GetActualMenu();

        /// <summary>
        /// 获取当前已付款信息
        /// </summary>
        /// <returns></returns>
        Task<Result<SMPayDto>> GetPay();

        /// <summary>
        /// 现金支付
        /// </summary>
        /// <returns></returns>
        Task<Result<SMPayDto>> CashPay(CashPayInput input);

        /// <summary>
        /// 第三方支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<SMPayDto>> ThirdPay(ThirdPayInput input);

        /// <summary>
        /// 礼券支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<SMPayDto>> CouponPay(CouponPayInput input);

        /// <summary>
        /// 信用卡支付
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<SMPayDto>> CreditCardPay(CreditCardPayInput input);
    }
}
