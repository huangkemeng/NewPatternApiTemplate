using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NewPattern.Api.Engines.Bases;

namespace NewPattern.Api.Engines.SwaggerEngine
{
    public class SwaggerConfigurationEngine : IEngine<IServiceCollection>
    {
        public void Run(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<DisplayEnumDescFilter>();
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "NewPattern Api", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
                var basePath = AppContext.BaseDirectory;
                options.IncludeXmlComments(Path.Combine(basePath, "NewPattern.Api.xml"), true);
                options.IncludeXmlComments(Path.Combine(basePath, "NewPattern.Api.Primary.xml"), true);
                options.IncludeXmlComments(Path.Combine(basePath, "NewPattern.Api.Infrastructure.xml"), true);
            });
        }
    }
    public class SwaggerUsingEngine : IUsingEngine
    {
        public void Run(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}
