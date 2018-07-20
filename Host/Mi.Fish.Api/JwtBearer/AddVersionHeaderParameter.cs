using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mi.Fish.Api.JwtBearer
{
    public class AddVersionHeaderParameter : IOperationFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null) operation.Parameters = new List<IParameter>();
            operation.Parameters.Add(new NonBodyParameter()
            {
                Name = "version",
                In = "header",//query header body path formData
                Type = "string",
                Required = true //是否必选
            });
        }

    }
}
