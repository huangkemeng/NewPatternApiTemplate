﻿using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.ComponentModel;
using System.Reflection;

namespace NewPattern.Api.Engines.SwaggerEngine
{
    public class DisplayEnumDescFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            var type = context.Type;
            if (type.IsEnum)
            {
                var descriptions = GetTypeDescriptions(type);
                if (descriptions.Any())
                {
                    schema.Description = string.Join("  ", descriptions.Select(e => $"{e.Key}.{e.Value}"));
                }
            }
        }

        private List<KeyValuePair<string, string>> GetTypeDescriptions(Type type)
        {
            List<KeyValuePair<string, string>> descriptions = new List<KeyValuePair<string, string>>();
            var fieldInfos = type.GetFields().Where(e => e.FieldType == type);
            foreach (var field in fieldInfos)
            {
                var descType = field.GetCustomAttribute<DescriptionAttribute>();
                var key = field.GetRawConstantValue();
                if (descType != null)
                {
                    descriptions.Add(new KeyValuePair<string, string>(Convert.ToString(key!)!, descType.Description));
                }
                else
                {
                    descriptions.Add(new KeyValuePair<string, string>(Convert.ToString(key!)!, "--"));
                }
            }
            return descriptions;
        }
    }
}
