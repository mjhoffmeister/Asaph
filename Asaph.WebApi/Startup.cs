using Asaph.Bootstrapper;
using Asaph.Core.UseCases;
using Asaph.Core.UseCases.AddSongDirector;
using Asaph.WebApi.AddSongDirector;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;

namespace Asaph.WebApi
{
    /// <summary>
    /// Web API start-up class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public Startup(IConfiguration configuration) => Configuration = configuration;

        /// <summary>
        /// Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the
        /// container.
        /// </summary>
        /// <param name="services">Services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            string hydraContextUri = Configuration["HydraContextUri"];

            // Configure Asaph services
            services.AddAsaphServices(Configuration);

            // Configure the add song director use case
            services
                .AddTransient<
                    IAddSongDirectorBoundary<IActionResult>,
                    AddSongDirectorApiBoundary>(factory => new(new(hydraContextUri, "")))
                .AddTransient<
                    IAsyncUseCaseInteractor<AddSongDirectorRequest, IActionResult>,
                    AddSongDirectorInteractor<IActionResult>>();

            // Configure Azure AD B2C authentication
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAdb2c"));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("GrandmasterOnly", policy =>
                {
                    policy.RequireClaim("Roles", Roles.GrandmasterSongDirector);
                });
            });

            // TODO: restrict to known origins
            services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowAll",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyMethod()
                                      .AllowAnyHeader());
            });

            services.AddControllers();
            services.AddMvc();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc(
            //        "current", new OpenApiInfo { Title = "Asaph API", Version = "current" });
            //});
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request
        /// pipeline.
        /// </summary>
        /// <param name="app">App builder.</param>
        /// <param name="env">Web host environment.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger(c => c.RouteTemplate = "api-docs/{documentName}/openapi.json");
                app.UseSwaggerUI(c => c.SwaggerEndpoint(
                    "api-docs/current/openapi.json", "Asaph API"));
            }

            app.UseHttpsRedirection();

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
