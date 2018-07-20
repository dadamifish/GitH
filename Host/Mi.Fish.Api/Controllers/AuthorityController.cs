using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mi.Fish.Application.User;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mi.Fish.Api.Controllers
{
    public class AuthorityController :FishControllerBase
    {
        private readonly IAuthorityAppService _authorityAppService;
        public AuthorityController(IAuthorityAppService authorityAppService)
        {
            _authorityAppService = authorityAppService;
        }

        /// <summary>
        /// 用户授权
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UserAuthority([FromBody]UserAuthorityInputDto input)
        {
            var result = await _authorityAppService.UserAuthorityAsync(input);
            if (result.IsSuccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest(result.BaseResult());
            }
        }
        /// <summary>
        /// 清除授权
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpDelete("{type}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteAuthority(EnumAuthorityType type)
        {
            var result = await _authorityAppService.DeleteAuthorityAsync(type);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return BadRequest(result.BaseResult());
            }
        }

        /// <summary>
        /// 查询权限
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("{type}")]
        [ProducesResponseType(typeof(Judge), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UserPower(EnumAuthorityType type)
        {
            var result = await _authorityAppService.UserPowerAsync(type);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.BaseResult());
            }
        }
    }
}