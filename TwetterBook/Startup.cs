using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwetterBook.Data;
using TwetterBook.Options;
using TwetterBook.Services;

namespace TwetterBook
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

            var JwtSettings = new JwtSettings();
            Configuration.Bind(key: nameof(JwtSettings), JwtSettings);
            services.AddSingleton(JwtSettings);

            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<DataContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddAuthentication(configureOptions: x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            })
             .AddJwtBearer(x =>
             {
                 x.SaveToken = true;
                 x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSettings.Secret)),
                     ValidateIssuer = true,
                     ValidateAudience = false,
                     RequireExpirationTime = false,
                     ValidateLifetime = true,

                 };

             });


            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Info { Title = "TweetBook API", Version = "v1" });

                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer",new string[0] }
                };

                x.AddSecurityDefinition(name: "Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization Header Using The Bearer Scheme",
                    Name = "Authorization",
                    In = "Header",
                    Type = "apiKey"

                });
                x.AddSecurityRequirement(security);

            });




            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IIdentityService, IdentityService>();




        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
               
            }
            else
            {
                app.UseHsts();
            }

            var swaggerOptions = new SwaggerOptions1();
            Configuration.GetSection(nameof(SwaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger(option => {
                option.RouteTemplate = swaggerOptions.JsonRoute;
                });

            app.UseSwaggerUI(option => option.SwaggerEndpoint(swaggerOptions.UiEndPoint, swaggerOptions.Description));



            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
