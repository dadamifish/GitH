using Mi.Fish.Application;
using Mi.Fish.Application.Sync;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mi.Fish.Api.Controllers
{
    /// <summary>
    /// 数据同步接口
    /// </summary>
    public class SyncController : FishControllerBase
    {
        private readonly ISyncAppService _syncAppService;
        private readonly ICookProvider _cookSetting;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="syncAppService"></param>
        /// <param name="cookSetting"></param>
        public SyncController(ISyncAppService syncAppService, ICookProvider cookSetting)
        {
            _syncAppService = syncAppService;
            _cookSetting = cookSetting;
        }

        /// <summary>
        /// 数据同步接口
        /// </summary>
        /// <param name="storageNo">门店编号</param>
        /// <param name="terminalId">机器号</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpGet("SyncData/{storageNo}/{terminalId}")]
        [AllowAnonymous]
        public IActionResult SyncData(string storageNo, string terminalId)
        {
            var result = _syncAppService.SyncData(storageNo, terminalId, true);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(new Result(result.Code, result.Message));
        }

        /// <summary>
        /// 厨打测试
        /// </summary>
        /// <param name="printStr">格式 printname:printstring</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        [HttpGet("TestCookPrint")]
        public IActionResult TestCookPrint(string printStr)
        {
            var printJson = string.Empty;
            var setting = _cookSetting.GetGetCookSetting(FishSession.StorageNo);
            if (!string.IsNullOrWhiteSpace(printStr) && setting != null)
            {
                var prints = printStr.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                if (prints.Count() > 1)
                {
                    var printModel = new PrintModel
                    {
                        PrintName = prints[0],
                        IsCut = setting.IsCut,
                        Contents = new List<PrintDetail>()
                    };
                    printModel.Contents.Add(new PrintDetail()
                    {
                        Content = prints[1],
                        Type = PrintType.WriteLine
                    });
                    printJson = JsonConvert.SerializeObject(printModel);
                }
                else
                {
                    return BadRequest(new Result(ResultCode.Fail, "格式错误"));
                }
            }
            else
            {
                return BadRequest(new Result(ResultCode.Fail, "要打印的数据不能为空"));
            }
            var result = _syncAppService.SyncCookPrint(printJson, setting);
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(new Result(result.Code, result.Message));
        }
    }
}
