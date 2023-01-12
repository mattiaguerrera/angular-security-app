using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using PTCApi.Model;
using Microsoft.IdentityModel.Tokens;
using PTCApi.EntityClasses;
using System.Text;

namespace PTCApi
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
            // Tell this project to allow CORS
            services.AddCors();

            ConfigureJwt(services);

            services.AddAuthorization(cfg =>
            {
                // NOTE: The claim key and value are case-sensitive
                cfg.AddPolicy("CanAccessProducts", p =>
                   p.RequireClaim("CanAccessProducts", "true"));
            });

            // Convert JSON from Camel Case to Pascal Case
            services.AddControllers().AddJsonOptions(
              options =>
              {
                  options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
              });

            // Setup the PTC DB Context
            // Read in connection string from 
            // appSettings.json file
            services.AddDbContext<PtcDbContext>
              (options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PTCApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PTCApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // NOTE: Make sure CORS comes before UseAuthentication and UseAuthorization
            app.UseCors(options =>
              options.WithOrigins("http://localhost:4200")
              .AllowAnyMethod().AllowAnyHeader()
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureJwt(IServiceCollection services)
        {
            // Get JWT Token Settings from appsettings.json file
            JwtSettings settings = GetJwtSettings();
            services.AddSingleton<JwtSettings>(settings);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters =
                  new TokenValidationParameters
                  {
                      ValidateIssuerSigningKey = true,
                      IssuerSigningKey = new SymmetricSecurityKey(
                      Encoding.UTF8.GetBytes(settings.Key)),
                      ValidateIssuer = true,
                      ValidIssuer = settings.Issuer,

                      ValidateAudience = true,
                      ValidAudience = settings.Audience,

                      ValidateLifetime = true,
                      ClockSkew = TimeSpan.FromMinutes(
                             settings.MinutesToExpiration)
                  };
            });
        }

        public JwtSettings GetJwtSettings()
        {
            JwtSettings settings = new JwtSettings();

            settings.Key = Configuration["JwtToken:key"];
            settings.Audience = Configuration["JwtToken:audience"];
            settings.Issuer = Configuration["JwtToken:issuer"];
            settings.MinutesToExpiration =
             Convert.ToInt32(
                Configuration["JwtToken:minutestoexpiration"]);

            return settings;
        }

    }
}
