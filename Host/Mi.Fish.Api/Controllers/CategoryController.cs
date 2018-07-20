using System;
namespace Mi.Fish.Api.Controllers
{
    using System.Threading.Tasks;
    using Mi.Fish.Application;
    using Mi.Fish.ApplicationDto;
    using Mi.Fish.Infrastructure.Results;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    /// <summary>
    /// 分类列表
    /// </summary>
    /// <seealso cref="FishControllerBase" />
    [ApiVersionNeutral]
    public class CategoryController : FishControllerBase
    {
        private readonly ICategoryService _CategoryService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryService"></param>
        public CategoryController(ICategoryService categoryService)
        {
            this._CategoryService = categoryService;
        }

        /// <summary>
        /// 获取分类列表
        /// </summary>
        /// <param name="level">层级</param>
        /// <param name="parentNo">父级category no</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(CategoryDetailOutPut), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpGet("categories/{level}/{parentNo?}")]
        public async Task<IActionResult> GainCategoryList(int level, string parentNo)
        {
            var result = await _CategoryService.GetCategoryList(level, parentNo);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.BaseResult());
        }
    }
}