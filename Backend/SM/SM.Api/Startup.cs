using System;
using System.IO;
using System.Reflection;
using CMC.Common.Domain.Entities;
using CMC.Common.Extensions.AspNetCore;
using CMC.Common.Repositories;
using CMC.Common.Repositories.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SM.Api.Helpers;
using SM.Domain.Options;

namespace SM.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.Load("SM.Application");
            string xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            services.AddOptions<SigningConfiguration>(ServiceLifetime.Scoped, "SigningConfigurations");
            services.AddOptions<SmtpConfiguration>(ServiceLifetime.Scoped, "SmtpConfigurations");
            services.AddOptions<ShareConfiguration>(ServiceLifetime.Scoped, "ShareConfiguration");
            services.AddOptions<LevelsConfiguration>(ServiceLifetime.Scoped, "LevelsConfiguration");
            services.AddOptions<ColorScheme>(ServiceLifetime.Scoped, "ColorScheme");
            services.AddOptions<InfoApp>(ServiceLifetime.Scoped, "InfoApp");
            services.AddSwagger("Salary Mark Api", "v1", xmlPath, "Api usada no sistema Salary Mark Carreira Muller");
            services.AddScoped(typeof(IUnitOfWork), s => RepositoryExtensions.GetUnitOfWork<Cmcodes>(Configuration.GetConnectionString("SM")));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            #region Validators
            AssemblyScanner.FindValidatorsInAssembly(assembly)
                           .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
            #endregion
            services.AddMediatR(assembly);
            services.RegisterAllTypes(assembly, "Interactor");
            services.AddCommonServices();
            services.AddJwtSecurity(Configuration.GetValue<string>("SigningConfigurations:SecretKey"),
                Configuration.GetValue<string>("SigningConfigurations:Issuer"),
                Configuration.GetValue<string>("SigningConfigurations:Audience"));
            services.AddMvc(options => options.Filters.Add<NotificationFilter>());
            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .WithOrigins("http://localhost:4200",
                             "https://salarymark-dev.carreira.com.br",
                             "https://salarymark-homolog.carreira.com.br",
                             "https://salarymark.carreira.com.br")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger("Salary Mark Api", "/swagger/v1/swagger.json");
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
