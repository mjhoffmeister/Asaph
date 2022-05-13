using Asaph.Bootstrapper;
using Asaph.Core.UseCases;
using Asaph.Core.UseCases.AddSongDirector;
using Asaph.Core.UseCases.GetSongDirectors;
using Asaph.WebApi.GcpSecretManagerConfigurationProvider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddGcpSecretManager(builder.Configuration);

builder.Configuration.AddEnvironmentVariables();

string baseUri = builder.Configuration["BaseUri"];

string hydraContextUri = builder.Configuration["HydraContextUri"];

string songDirectorsBaseUri = @$"{baseUri.TrimEnd('/')}/song-directors/";

builder.Services.AddCors();

// Add Asaph services and use cases
builder.Services
    .AddAsaphServices(builder.Configuration)
    .AddAddSongDirectorUseCase(songDirectorsBaseUri, hydraContextUri)
    .AddGetSongDirectorsUseCase(songDirectorsBaseUri, hydraContextUri);

// Add Azure AD B2C authentication
builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, "AzureAdb2c");

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("GrandmasterOnly", policy =>
    {
        policy.RequireClaim("Roles", Roles.GrandmasterSongDirector);
    });
});

WebApplication? app = builder.Build();

string[] allowedOrigins = builder.Configuration["Cors:AllowedOrigins"].Split(',');

app.UseCors(c => c
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithOrigins(allowedOrigins)
    .SetIsOriginAllowed(origin => true)
    .AllowCredentials());

app.UseAuthentication();

app.UseAuthorization();

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

// Set up REST API for adding song directors
app.MapPost(
    "/song-directors",
    [Authorize] async (
        HttpContext http,
        SongDirectorApiModel newSongDirector,
        IAsyncUseCaseInteractor<AddSongDirectorRequest, IResult> addSongDirectorInteractor) =>
{
    string? requesterId = http.User.GetNameIdentifierId();

    if (requesterId == null)
        return Results.Unauthorized();

    AddSongDirectorRequest addSongDirectorRequest = new(
        requesterId,
        newSongDirector.Name,
        newSongDirector.EmailAddress,
        newSongDirector.PhoneNumber,
        newSongDirector.Rank,
        newSongDirector.IsActive);

    return await addSongDirectorInteractor
            .HandleAsync(addSongDirectorRequest)
            .ConfigureAwait(false);
});

// Set up REST API for getting song directors
app.MapGet(
    "/song-directors",
    [Authorize] async (
        HttpContext http,
        IAsyncUseCaseInteractor<
            GetSongDirectorsRequest, IResult> getSongDirectorsInteractor) =>
    {
        string? requesterId = http.User.GetNameIdentifierId();

        if (requesterId == null)
            return Results.Unauthorized();

        GetSongDirectorsRequest getSongDirectorsRequest = new(requesterId);

        return await getSongDirectorsInteractor
            .HandleAsync(getSongDirectorsRequest)
            .ConfigureAwait(false);
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