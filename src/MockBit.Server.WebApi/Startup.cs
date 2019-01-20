using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockBit.Server.WebApi.Extensions;
using MockBit.Server.WebApi.Infrastructure;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MockBit.Server.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v0.1", new Info { Title = "Mock Bit Server Setup API", Version = "v0.1" });
                var filePath = Path.Combine(AppContext.BaseDirectory, "MockBit.Server.WebApi.xml");
                c.IncludeXmlComments(filePath);
            });

            services.AddSingleton<SynchronizedRouteCollection>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.MapByPort(int.Parse(_configuration["ListeningPorts:SetupApiPort"]), ConfigureSetup);
            app.MapByPort(int.Parse(_configuration["ListeningPorts:MockApiPort"]), ConfigureMock);
        }

        private void ConfigureSetup(IApplicationBuilder app)
        {
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v0.1/swagger.json", "Mock Bit Server Setup API V0.1");
            });
        }

        private void ConfigureMock(IApplicationBuilder app)
        {
            var routeCollection = app.ApplicationServices.GetRequiredService<SynchronizedRouteCollection>();
            app.UseRouter(routeCollection);
        }
    }
}
