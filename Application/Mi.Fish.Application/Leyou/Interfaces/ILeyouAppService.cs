using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mi.Fish.Application.Leyou
{
    /// <summary>
    /// 乐游服务接口
    /// </summary>
    public interface ILeyouAppService
    {
        /// <summary>
        /// 是否有新的订单
        /// </summary>
        /// <returns></returns>
        Task<Result<bool>> IsNewOrder();

        /// <summary>
        /// 获取上传到乐游APP的商品
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetAppGoodsDto>>> GetAppGoods();

        /// <summary>
        /// 获取POS机未上传到乐游APP的商品
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetFishGoodsDto>>> GetFishGoods();

        /// <summary>
        /// 添加商品到乐游APP
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> AddAppGoods(AddAppGoodsInput input);

        /// <summary>
        /// 从乐游APP中删除商品
        /// </summary>
        /// <param name="goodsNo"></param>
        /// <returns></returns>
        Task<Result> DelAppGoods(string goodsNo);
    }
}
