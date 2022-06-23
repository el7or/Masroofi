using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Puzzle.Masroofi.Core.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Puzzle.Masroofi.Core.Helpers
{
    public class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            var lstLangs = Enum.GetValues(typeof(Language)).Cast<Language>().Select(d => d.ToString()).ToList();
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = HeaderParameter.Language.ToString().ToLower(),
                Description = "ar for Arabic or en for English",
                In = ParameterLocation.Header,
                //Schema = new OpenApiSchema() { Type = "String" },
                Schema = new OpenApiSchema
                {
                    Default = new OpenApiString("en"),
                    Type = "string",
                    Enum = lstLangs.Select(p => new OpenApiString(p.ToString())).ToList<IOpenApiAny>()
                },
                Required = true,
                //Example = new OpenApiString("ar")
            });

            var lstEnumValues = Enum.GetValues(typeof(ChannelType)).Cast<ChannelType>().Select(d => (int)d).ToList();
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = HeaderParameter.Channel.ToString().ToLower(),
                Description = "Mobile = 1, POS = 2, Admin = 3",
                In = ParameterLocation.Header,
                Schema = new OpenApiSchema
                {
                    Default = new OpenApiString("1"),
                    Type = "string",
                    Enum = lstEnumValues.Select(p => new OpenApiString(p.ToString())).ToList<IOpenApiAny>()
                },
                Required = true,
            });
        }
    }
}
