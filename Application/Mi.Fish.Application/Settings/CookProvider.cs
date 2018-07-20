using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;

namespace Mi.Fish.Application
{
    public class CookProvider : ICookProvider
    {
        private readonly AppSettings _appSetting;
        public CookProvider(IOptions<AppSettings> options)
        {
            this._appSetting = options.Value;
        }

        public CookSetting GetGetCookSetting(string key)
        {
            if (this._appSetting.CookSetting.ContainsKey(key))
            {
                return this._appSetting.CookSetting[key];
            }
            else
            {
                foreach (var item in this._appSetting.CookSetting)
                {
                    if (item.Key.Contains("-"))
                    {
                        var keys = item.Key.Split(new char[] { '-' });
                        if (int.TryParse(key, out int keyInt) &&
                        int.TryParse(keys[0], out int key1) &&
                        int.TryParse(keys[01], out int key2))
                        {
                            if (keyInt <= key2 && keyInt >= key1)
                            {
                                return item.Value;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
    public class CookSetting
    {
        public string CookPrintIP { get; set; }
        public int CookPort { get; set; }
        public int IsCut { get; set; }
    }
}
