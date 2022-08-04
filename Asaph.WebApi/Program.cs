using Asaph.Core.UseCases;
using Asaph.WebApi.GcpSecretManagerConfigurationProvider;
using Asaph.WebApi.UseCases;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddGcpSecretManager(builder.Configuration);

builder.Configuration.AddEnvironmentVariables();

string baseUri = builder.Configuration["BaseUri"];

builder.Logging
    .ClearProviders()
    .AddConsole();

builder.Services
    .AddCors()
    .AddUseCaseApis(builder.Configuration)
    .AddAuthorization(options =>
    {
        options.AddPolicy("GrandmasterOnly", policy =>
        {
            policy.RequireClaim("Roles", Roles.GrandmasterSongDirector);
        });
    })
    .AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdb2c");

WebApplication? app = builder.Build();

string[] allowedOrigins = builder.Configuration["Cors:AllowedOrigins"].Split(',');

app
    .UseCors(c => c
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins(allowedOrigins)
        .SetIsOriginAllowed(origin => true)
        .AllowCredentials())
    .UseAuthentication()
    .UseAuthorization();

// Set the API documentation path
string apiDocumentationPath = "/api-docs/current/openapi.json";

// Redirect the base URL to API documentation
app.MapGet("/", () => Results.Redirect(apiDocumentationPath));

// Set up REST API for getting API documentation
app.MapGet(
    apiDocumentationPath,
    (HttpContext http) =>
    {
        OpenApiDocument asaphApiDocumentation = ApiDocumentationBuilder.GetAsaphOpenApiDocument(
            app.Configuration, http.User);

        string json = asaphApiDocumentation
            .Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json);

        return Results.Content(json, "application/json");
    });

string? port;

string? url = null;

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // TODO: add enable swagger environment variable
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint($"{baseUri}{apiDocumentationPath}", "Asaph API");

        // Configure OAuth
        options.OAuthClientId(app.Configuration["SwaggerUI:ClientId"]);
        options.OAuthUsePkce();
    });
}
else
{
    port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    url = $"http://0.0.0.0:{port}";
}

app.Run(url);