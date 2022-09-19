using System.Net;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration);

builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    var existingOnRedirectToIdentityProviderHandler = options.Events.OnRedirectToIdentityProvider;
    options.Events.OnRedirectToIdentityProvider = async context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            // Return 401 instead of default 302 redirect in case API request is unauthorized
            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.HandleResponse();
            return;
        }

        await existingOnRedirectToIdentityProviderHandler(context);
    };
});

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;

        // SameSiteMode.None is required for single sign out to work.
        // Azure AD implements front-channel single sign out.
        // https://learn.microsoft.com/en-us/azure/active-directory/develop/v2-protocols-oidc#single-sign-out

        //options.Cookie.SameSite = SameSiteMode.None;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
