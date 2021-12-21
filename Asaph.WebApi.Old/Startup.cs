using Asaph.Bootstrapper;
using Asaph.Core.UseCases;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System.Collections.Generic;

namespace Asaph.WebApi;

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
        string baseUri = Configuration["BaseUri"];
        string hydraContextUri = Configuration["HydraContextUri"];

        // Configure Asaph services
        services.AddAsaphServices(Configuration);

        // Configure use cases
        services
            .AddAddSongDirectorUseCase(baseUri, hydraContextUri)
            .AddGetSongDirectorsUseCase(baseUri, hydraContextUri);

        // Configure Azure AD B2C authentication
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(Configuration.GetSection("AzureAdb2c"));

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";
                options.Authority = Configuration["oidc:Authority"];
                options.ClientId = Configuration["oidc:ClientId"];
                options.ResponseType = "code";
                options.Prompt = "login";
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
            });
            //.AddMicrosoftIdentityWebApp(options => {
            //    options.Authority = "https://asaphworship.b2clogin.com/asaphworship.onmicrosoft.com/B2C_1_sign_in";
            //    options.ClientId = Configuration["SwaggerUI:ClientId"];
            //    options.Domain = Configuration["AzureAdB2c:Domain"];
            //    options.Instance = Configuration["AzureAdB2c:Instance"];
            //    options.SignUpSignInPolicyId = Configuration["AzureAdB2c:SignUpSignInPolicyId"];
            //});

        services.AddAuthorization(options =>
        {
            options.AddPolicy("GrandmasterOnly", policy =>
            {
                policy.RequireClaim("Roles", Roles.GrandmasterSongDirector);
            });
        });

        services.AddControllers();

        //services
        //    .AddControllersWithViews()
        //    .AddMicrosoftIdentityUI();

        // TODO: restrict to known origins
        services.AddCors(options => {
            options.AddPolicy(
                "AllowAll",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        //services.AddMvc();

        services.AddOptions();
        services.Configure<OpenIdConnectOptions>(Configuration.GetSection("iodc"));
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
            //app.UseSwaggerUIAuthentication();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    $"{Configuration["BaseUri"]}/api-docs/current/openapi.json", "Asaph API");

                // Configure OAuth
                c.OAuthClientId(Configuration["SwaggerUI:ClientId"]);
                c.OAuthClientSecret(Configuration["Swagger:ClientSecret"]);
                c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
            });
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCookiePolicy();

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
