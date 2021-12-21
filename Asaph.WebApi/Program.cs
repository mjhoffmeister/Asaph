using Asaph.Bootstrapper;
using Asaph.Core.UseCases;
using Asaph.Core.UseCases.AddSongDirector;
using Asaph.Core.UseCases.GetSongDirectors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

string baseUri = builder.Configuration["BaseUri"];
string hydraContextUri = builder.Configuration["HydraContextUri"];

string songDirectorsBaseUri = @$"{baseUri.TrimEnd('/')}/song-directors/";

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

app.UseAuthentication();

app.UseAuthorization();

// Set the API documentation path
string apiDocumentationPath = "/api-docs/current/openapi.json";

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
        AddSongDirectorRequest addSongDirectorRequest,
        IAsyncUseCaseInteractor<AddSongDirectorRequest, IResult> addSongDirectorInteractor) => 
{
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

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint($"{baseUri}{apiDocumentationPath}", "Asaph API");

        // Configure OAuth
        options.OAuthClientId(app.Configuration["SwaggerUI:ClientId"]);
        options.OAuthUsePkce();
    });
}

app.Run();