using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using System.Threading.Tasks;

namespace Mi.Fish.Application.Print
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPrintAppService
    {
        /// <summary>
        /// 订单重打印
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result<RePrintDto>> RePrint(RePrintInput input);
    }
}
