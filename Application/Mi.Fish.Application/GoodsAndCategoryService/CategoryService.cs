namespace Mi.Fish.Application
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Abp.AutoMapper;
    using Abp.Runtime.Caching;
    using Mi.Fish.ApplicationDto;
    using Mi.Fish.EntityFramework;
    using Mi.Fish.Infrastructure.Results;
    public class CategoryService : AppServiceBase, ICategoryService
    {
        private readonly ICacheManager _cacheManager;
        private readonly LocalDbContext _context;
        public CategoryService(ICacheManager cacheManager, LocalDbContext context)
        {
            this._cacheManager = cacheManager;
            this._context = context;
        }

        public async Task<Result<CategoryDetailOutPut>> GetCategoryList(int level, string parentid)
        {
            var result = new CategoryDetailOutPut();
            try
            {
                string userid = FishSession.UserId;
                var userdata = _cacheManager.GetUserDataCacheByUserId(userid);
                var defaultResult = GainDefaultResult<CategoryDetailOutPut>(userdata);
                if (defaultResult.Code == ResultCode.Expired)
                {
                    return defaultResult;
                }
                Logger.Info("开始获取分类数据，用户信息 stage no:" + userdata.StorageNo + ",termid:" + userdata.TerminalID + ",level:" + level + ",parentid:" + parentid);
                var key = userid + "_" + level + "_" + parentid;
                result = _cacheManager.GetCategoryCache(key);
                if (result != null)
                {
                    return Result.FromData(result);
                }
                else
                {
                    result = new CategoryDetailOutPut();
                }
                level = level + 1;
                if (level == 1)
                {
                    level = level + 1;
                    var topLevel = await GetTopCategoryList(userdata);
                    if (topLevel != null && topLevel.Count > 0)
                    {
                        result.TopLevelCategory = topLevel.MapTo<List<CategoryOutPut>>();
                        level = await GetSecendLevelCategory(level, topLevel[0].Item_Clsno, result, userdata);
                    }
                }
                else if (level == 2)
                {
                    level = await GetSecendLevelCategory(level, parentid, result, userdata);
                }
                else if (level == 3)
                {
                    level = await GetThirdLevelCategory(level, parentid, result, userdata);
                }
                _cacheManager.SetCategoryCache(key, result);
                return Result.FromData(result);
            }
            catch (Exception ex)
            {
                Logger.Error("获取分类数据失败", ex);
            }
            return new Result<CategoryDetailOutPut>(ResultCode.Fail, new CategoryDetailOutPut());
        }

        private Result<T> GainDefaultResult<T>(UserData userinfo)
        {
            if (string.IsNullOrEmpty(userinfo.StorageNo) || string.IsNullOrEmpty(userinfo.TerminalID))
            {
                return new Result<T>(ResultCode.Expired, default(T), "登录已经失效");
            }
            return new Result<T>(ResultCode.Ok, default(T));
        }
        private async Task<int> GetSecendLevelCategory(int level, string parentid, CategoryDetailOutPut result, UserData userdata)
        {
            level++;
            var secendLevel = await GetSubLevelCateogry(parentid, level, userdata);
            if (secendLevel.Count > 0)
            {
                result.SecendLevelCategory = secendLevel.MapTo<List<CategoryOutPut>>();
                level = await GetThirdLevelCategory(level, secendLevel[0].Item_Clsno, result, userdata);
            }

            return level;
        }

        private async Task<int> GetThirdLevelCategory(int level, string parentid, CategoryDetailOutPut result, UserData userdata)
        {
            level++;
            var thirdLevel = await GetSubLevelCateogry(parentid, level, userdata);
            if (thirdLevel.Count > 0)
            {
                result.ThirdLevelCategory = thirdLevel.MapTo<List<CategoryOutPut>>();
                result.CurrentSelected = thirdLevel[0].MapTo<CategoryOutPut>();
            }
            return level;
        }

        private async Task<List<CategoryDTO>> GetTopCategoryList(UserData userinfo)
        {
            Logger.Info("开始获取顶层分类数据。");
            var topCategorylist = new List<CategoryDTO>();
            var query = ApplicationConsts.QueryTopCategory;
            object obj = new
            {
                storageno = userinfo.StorageNo,
                terminalid = userinfo.TerminalID,
                promotionname = ""
            };
            topCategorylist = await _context.ExecuteFunctionAsync<List<CategoryDTO>>(query, obj);
            return topCategorylist;
        }

        private async Task<List<CategoryDTO>> GetSubLevelCateogry(string parentid, int level, UserData userinfo)
        {
            Logger.Info("开始获取下层分类数据，level:" + level + ",parentid:" + parentid);
            var topCategorylist = new List<CategoryDTO>();
            try
            {
                var query = ApplicationConsts.QuerySubLevelCategory;
                object obj = new
                {
                    storageno = userinfo.StorageNo,
                    terminalid = userinfo.TerminalID,
                    promotionname = "",
                    jibie = level,
                    shangji = parentid,
                };
                topCategorylist = await _context.ExecuteFunctionAsync<List<CategoryDTO>>(query, obj);
            }
            catch (Exception ex)
            {
                Logger.Error("获取下层数据失败", ex);
            }
            return topCategorylist;
        }
    }
}
