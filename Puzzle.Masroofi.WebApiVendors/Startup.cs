using AutoWrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Puzzle.Masroofi.Core.Extensions;
using Puzzle.Masroofi.Core.Helpers;
using Puzzle.Masroofi.Core;
using Puzzle.Masroofi.Core.Enums;
using Puzzle.Masroofi.Core.Mapper;
using Puzzle.Masroofi.Core.ViewModels;
using Puzzle.Masroofi.Core.ViewModels.Auth;
using Puzzle.Masroofi.ServiceInterface;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Globalization;
using System.Threading;
using Puzzle.Masroofi.Core.ViewModels.PushNotifications;

namespace Puzzle.Masroofi.WebApiVendors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(VendorProfile));
            services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });

            services.AddDbContext<MasroofiDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("MasroofiDbConnection")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                                builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            // configure jwt authentication
            var jwtKey = Configuration.GetSection("Security:JWTKey").Value;
            var key = Encoding.ASCII.GetBytes(jwtKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddServices();

            services.Configure<RouteAndroid>(Configuration.GetSection("RouteAndroid"));

            // configuration to get current user data
            services.AddHttpContextAccessor();
            services.AddScoped<UserIdentity>(provider =>
            {
                var context = provider.GetService<IHttpContextAccessor>().HttpContext;
                var user = context.User;
                var name = user?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                Guid? id = null;
                Language? language = null;
                ChannelType? channel = null;
                if (!string.IsNullOrEmpty(name))
                {
                    id = Guid.Parse(name);
                }
                if (context.Request.Headers.TryGetValue(HeaderParameter.Language.ToString().ToLower(), out StringValues languageHeader))
                    language = languageHeader.FirstOrDefault().ToEnum<Language>();
                else
                    language = Language.en;
                CultureInfo cultureInfo = new CultureInfo(language.ToString());
                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;

                if (context.Request.Headers.TryGetValue(HeaderParameter.Channel.ToString().ToLower(), out StringValues channelHeader))
                {
                    channel = channelHeader.First().ToEnum<ChannelType>();
                    //if (channel != ChannelType.Mobile)
                    //    throw new BusinessException("Wrong Channel Type");
                }
                else
                    channel = ChannelType.Mobile;

                return new UserIdentity(id, language, channel);
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Puzzle.Masroofi.WebApiVendors", Version = "v1" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Auth Bearer Scheme",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirments = new OpenApiSecurityRequirement
                {
                    {securitySchema, new []{"Bearer"}}
                };
                c.AddSecurityRequirement(securityRequirments);

                c.OperationFilter<AddRequiredHeaderParameter>();

                var filePath = Path.Combine(AppContext.BaseDirectory, "Puzzle.Masroofi.WebApiVendors.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseApiResponseAndExceptionWrapper<PuzzleApiResponse>(new AutoWrapperOptions { ShowStatusCode = true, UseCustomSchema = true, IgnoreNullValue = false });

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Puzzle.Masroofi.WebApiVendors v1"));

            app.UseHttpsRedirection();

            app.UseCors("AllowAllOrigins");
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
