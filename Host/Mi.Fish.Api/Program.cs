using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mi.Fish.ApplicationDto;
using Mi.Fish.Infrastructure.Results;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mi.Fish.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = NLog.Web.NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

            try
            {
                ChangeCulture();

                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                logger.Fatal(e);
                throw;
            }
            finally
            {
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        private static void ChangeCulture()
        {
            var culture = CultureInfo.CurrentCulture;
            string[] simples = new[] { "zh-CN", "zh-SG" }.Select(o => o.ToLower()).ToArray();
            string[] traditional = new[] { "zh-HK", "zh-MO", "zh-TW" }.Select(o => o.ToLower()).ToArray();

            CultureInfo newCulture = null;
            if (simples.Contains(culture.Name.ToLower()))
            {
                newCulture = new CultureInfo("zh-Hans");
            }

            if (traditional.Contains(culture.Name.ToLower()))
            {
                newCulture = new CultureInfo("zh-Hant");
            }

            if (newCulture != null)
            {
                CultureInfo.CurrentCulture = newCulture;
                CultureInfo.CurrentUICulture = newCulture;

                CultureInfo.DefaultThreadCurrentCulture = newCulture;
                CultureInfo.DefaultThreadCurrentUICulture = newCulture;
            }
        }
    }
}

