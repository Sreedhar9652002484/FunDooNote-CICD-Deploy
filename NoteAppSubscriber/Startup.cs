using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using NoteAppSubscriber.Interface;
using NoteAppSubscriber.Services;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NoteAppSubscriber
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

            services.AddControllers();

            services.AddSingleton<ConnectionFactory>(_ => new ConnectionFactory
            {
                Uri = new Uri(Configuration["RabbitMQSettings:HostUri"])
            });

            services.AddScoped<IRabbitMQSubscriber>(provider =>
            {
                var factory = provider.GetRequiredService<ConnectionFactory>();
                var configuration = provider.GetRequiredService<IConfiguration>();
                var busControl = provider.GetRequiredService<IBusControl>(); // Inject the MassTransit bus
                return new RabbitMQSubscriber(factory, configuration, busControl);
            });

            // Configure and register MassTransit bus
            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserRegistrationEmailSubscriber>();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(new Uri(Configuration["RabbitMQSettings:HostUri"]));
                });
            });

            services.AddMassTransitHostedService();



            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FunDoNote", Version = "v1" });
            });
        }

                // Add other services and dependencies here

 

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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FunDoNote v1");
            });
        }
    }
}
