using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.AspNetCore;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.AspNetCore.Mvc.Validation;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Web.Models;
using Castle.Facilities.Logging;
using Castle.Windsor.MsDependencyInjection;
using Mi.Fish.Api.JwtBearer;
using Mi.Fish.ApplicationDto;
using Mi.Fish.EntityFramework;
using Mi.Fish.Infrastructure.Api.Exception;
using Mi.Fish.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Mi.Fish.Application;
using Mi.Fish.Infrastructure.Api;
using Mi.Fish.Infrastructure.Api.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Mi.Fish.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<DbOptions>(Configuration.GetSection(DbOptions.SectionKey));
            services.Configure<LeyouSetting>(Configuration.GetSection("Leyou"));
            services.Configure<JwtSetting>(Configuration.GetSection("JWT"));

            services.AddScoped<IUrlHelper>(provider =>
            {
                var actionContext = provider.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddApiVersioning(option =>
            {
                option.ReportApiVersions = true;
                option.ApiVersionReader = new HeaderApiVersionReader("version");
                option.AssumeDefaultVersionWhenUnspecified = true;
                option.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info()
                {
                    Title = "Fish API Version 1.0",
                    Description = "餐饮客户端 api",
                    Version = "1.0",
                    TermsOfService = "None"
                });
                options.SwaggerDoc("v2", new Info
                {
                    Title = "Fish API Version 2.0",
                    Description = "餐饮客户端 api",
                    Version = "2.0",
                    TermsOfService = "None"
                });

                options.DescribeAllEnumsAsStrings();
                options.DescribeAllParametersInCamelCase();

                options.SchemaFilter<EnumSchemaFilter>();
                options.ParameterFilter<EnumParameterFilter>();
                //options.OperationFilter<AddVersionHeaderParameter>();

                var xmls = GetXmls();
                foreach (var xml in xmls)
                {
                    options.IncludeXmlComments(xml);
                }

                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example - Authorization: Bearer {token}",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[] { } }
                });

            });

            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //}).AddCookie();

            //JwtBearer认证
            var symmetricKeyAsBase64 = Configuration["JWT:ServerSecret"];
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var tokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = signingKey,
                ValidIssuer = Configuration["JWT:Issuer"],
                ValidAudience = Configuration["JWT:Audience"],
                ClockSkew = TimeSpan.Zero
            };
            services.AddAuthorization(options =>
            {
                var authRequirement = new AuthRequirement("/api/user/login", ClaimTypes.Name, Configuration["JWT:Issuer"], Configuration["JWT:Audience"], signingCredentials);
                options.AddPolicy("POS", policy => policy.Requirements.Add(authRequirement));
            }).AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                //不使用https
                o.RequireHttpsMetadata = false;
                o.TokenValidationParameters = tokenValidationParameters;
            });

            //注入授权Handler
            services.AddSingleton<IAuthorizationHandler, AuthHandler>();

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    if (Configuration["FormattingIndented"].Equals(true.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        options.SerializerSettings.Formatting = Formatting.Indented;
                    }
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //use PostConfigureOptions
            services.PostConfigure<MvcOptions>(options =>
            {
                var removeFilters = options.Filters.Where(o =>
                 {
                     var filter = o as ServiceFilterAttribute;
                     Type type = filter?.ServiceType;

                     return type == typeof(AbpExceptionFilter) || type == typeof(AbpValidationActionFilter);
                 }).ToList();

                foreach (var filterMetadata in removeFilters)
                {
                    options.Filters.Remove(filterMetadata);
                }

                options.Filters.AddService<ApiExceptionFilter>();
            });

            //ModelStateInvalidFilter
            services.Configure<ApiBehaviorOptions>(options =>
                options.InvalidModelStateResponseFactory = InvalidModelStateExecutor.Executer);

            services.AddCors(options => options.AddPolicy(FishApiConsts.DefaultPolicy,
                builder => builder.AllowAnyOrigin().AllowAnyHeader()
                    .WithMethods(HttpMethods.Get, HttpMethods.Patch, HttpMethods.Post, HttpMethods.Delete)));

            return services.AddAbp<FishApiModule>(options =>
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(facility =>
                    facility.LogUsing<NLogLoggerFactory>().WithConfig("NLog.config")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAbp();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Fish API V1.0");
                options.SwaggerEndpoint("/swagger/v2/swagger.json", "Fish API V2.0");
                options.ShowExtensions();
            });

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHsts();
            }

            app.UseAuthentication();

            //app.Run(async context =>
            //{
            //    if (!context.User.Identities.Any(identity => identity.IsAuthenticated))
            //    {
            //        await context.Response.WriteAsync("Hello First timer");
            //        return;
            //    }

            //    context.Response.ContentType = "text/plain";
            //    await context.Response.WriteAsync("Hello old timer");
            //});


            app.UseCors(FishApiConsts.DefaultPolicy);

            app.UseMvc();
        }

        private IReadOnlyList<string> GetXmls()
        {
            var baseDir = AppContext.BaseDirectory;

            return Directory.GetFiles(baseDir, "Mi.*.xml").ToImmutableList();
        }
    }
}
