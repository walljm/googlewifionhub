using JMW.Google.OnHub.API.Data;
using JMW.Google.OnHub.API.Options;
using JMW.Google.OnHub.API.Schedule;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Quartz;
using System;

namespace JMW.Google.OnHub.API
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "JMW.Google.OnHub.API",
                    Version = "v1",
                    Contact = new OpenApiContact
                    {
                        Email = "jason@walljm.com",
                        Name = "Jason Wall",
                        Url = new Uri("https://www.walljm.com")
                    },
                    Description = "This is a service that will poll your Google OnHub regularly and store the information so it can be easily queried via rest.",
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://github.com/walljm/googlewifionhub/blob/main/LICENSE")
                    }
                });
            });

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDbContext<ApplicationContext>();

            services.AddOptions<CollectionOptions>()
                .Bind(Configuration.Section<CollectionOptions>())
                .ValidateDataAnnotations();

            services.AddOptions<CacheOptions>()
                .Bind(Configuration.Section<CacheOptions>())
                .ValidateDataAnnotations();

            #region Quartz

            services.AddQuartz(q =>
            {
                var opts = Configuration.GetSection<CollectionOptions>();
                if (opts == null || opts.Schedule == null)
                {
                    throw new ArgumentException("The Collection section of the configuration must be populated.");
                }
                q.AddCollectJob(opts.Schedule);
            });

            services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            #endregion Quartz
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            ILogger<Startup> logger,
            IOptions<CollectionOptions> options)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "JMW.Google.OnHub.API.v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // validate the options.
            try
            {
                _ = options.Value;
            }
            catch (OptionsValidationException ex)
            {
                foreach (string failure in ex.Failures)
                {
                    logger.LogCritical(failure, ex);
                }

                Environment.Exit(1); // kill immediately
            }
        }
    }
}