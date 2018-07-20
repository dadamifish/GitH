using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Mi.Fish.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class EnumParameterFilter : IParameterFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        public void Apply(IParameter parameter, ParameterFilterContext context)
        {
            var type = context.ApiParameterDescription.Type;
            if (type.IsEnum)
            {
                var values = new XmsEnumExtensions().GetXmsEnumValues(type.GetTypeInfo());

                parameter.Extensions.Add(
                    "x-ms-enum", values
                );
            }
        }
    }
}
