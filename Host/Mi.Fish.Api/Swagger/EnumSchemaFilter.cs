using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Mi.Fish.Api
{
    /// <summary>
    /// 
    /// </summary>
    public class EnumSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(Schema schema, SchemaFilterContext context)
        {
            var typeInfo = context.SystemType.GetTypeInfo();

            if (typeInfo.IsEnum)
            {
                var values = new XmsEnumExtensions().GetXmsEnumValues(typeInfo);

                schema.Extensions.Add(
                    "x-ms-enum", values
                );
            };
        }
    }

}
