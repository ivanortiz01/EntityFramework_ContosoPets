using ContosoPets.DataAccess.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using ContosoPets.DataAccess.Services;

namespace ContosoPets.Api
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
            services.AddScoped<OrderService>();

            var builder = new SqlConnectionStringBuilder(
                Configuration.GetConnectionString("ContosoPets"));
            IConfigurationSection contosoPetsCredentials =
                Configuration.GetSection("ContosoPetsCredentials");

            builder.UserID = contosoPetsCredentials["UserId"];
            builder.Password = contosoPetsCredentials["Password"];

            services.AddDbContext<ContosoPetsContext>(options =>
                            options.UseSqlServer(builder.ConnectionString)
                                .EnableSensitiveDataLogging(Configuration.GetValue<bool>("Logging:EnableSqlParameterLogging"))
                        );

            services.AddControllers();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
