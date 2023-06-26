using System.Security.Claims;
using Google.Apis.Books.v1;
using Google.Apis.Services;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton(_ => new BooksService(new BaseClientService.Initializer
    { ApplicationName = "MyBooks", ApiKey = "AIzaSyC6jcu6mnMvItYIH5_wkv1P27RgH21iBoo" }));
builder.Services.AddMudServices();
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