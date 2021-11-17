using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SignalChatik.Helpers;
using SignalChatik.Models;
using System;

namespace SignalChatik
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly string AllowLocalOrigin = "AllowLocal";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(AllowLocalOrigin,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://127.0.0.1:3000", "http://localhost:3000")
                                             .AllowAnyHeader()
                                             .AllowAnyMethod()
                                             .AllowCredentials()
                                             .SetPreflightMaxAge(TimeSpan.FromMinutes(5));
                                  });
            });

            services.AddDbContextPool<ChatikContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ChatikDatabase"),
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

            var authOptionsConfiguration = Configuration.GetSection("Auth");
            var authOptions = authOptionsConfiguration.Get<AuthOptions>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = authOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = authOptions.Audience,
                        ValidateLifetime = true,
                        IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddControllers();

            services.Configure<AuthOptions>(authOptionsConfiguration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors(AllowLocalOrigin);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
