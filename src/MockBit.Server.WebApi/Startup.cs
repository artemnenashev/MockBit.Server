using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MockBit.Server.WebApi
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Will be here setup api");
            });
        }

        private void ConfigureMock(IApplicationBuilder app)
        {
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Will be here mock api");
            });
        }
    }
}
