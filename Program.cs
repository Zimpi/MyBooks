using System.Security.Claims;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor.Services;
using Google.Apis.Util.Store;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication("Cookies")
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "MyBookAuthToken";
        opt.LoginPath = "/auth/google-login";
    })
    .AddGoogle(options =>
    {
        options.ClientId = "32590108289-r2lfcelsb9e57h5ekkeri7mr62cqtqhi.apps.googleusercontent.com";
        options.ClientSecret = "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";
        options.Scope.Add("profile");
        options.Events.OnCreatingTicket = context =>
        {
            var picture = context.User.GetProperty("picture").GetString();
            if (picture != null) context.Identity?.AddClaim(new Claim("picture", picture));
            return Task.CompletedTask;
        };
    });


// Registriere den BookService API
builder.Services.AddScoped<BooksService>(_ =>
{
    var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        new ClientSecrets
        {
            ClientId = "32590108289-r2lfcelsb9e57h5ekkeri7mr62cqtqhi.apps.googleusercontent.com", 
            ClientSecret = "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"
        },
        new[] { BooksService.Scope.Books },
        "user", CancellationToken.None, new FileDataStore("Books.ListMyLibrary")).Result;

    // Create the service.
    var service = new BooksService(new BaseClientService.Initializer()
    {
        HttpClientInitializer = credential,
        ApplicationName = "MyBooks",
    });


    return service;
});



builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();