using System.Security.Claims;
using System.Text;
using asp_net_core_erp;
using asp_net_core_erp.Extensions;
using asp_net_core_erp.Models;
using asp_net_core_erp.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<AuthService>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.PrivateKey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization(x =>
{
    x.AddPolicy("admin", p => p.RequireRole("manager"));
    x.AddPolicy("tech", p => p.RequireRole("developer"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", (AuthService service) =>
{
    var user = new User(
        1,
        "test",
        "Test User",
        "test@test.com",
        "P@ssw0rd",
        ["developer"]);

    Response response = new Response(Token: service.Create(user));

    return response;
});

app.MapGet("/test", () => "OK!")
    .RequireAuthorization();

app.MapGet("/test/user", (ClaimsPrincipal user) => new
    {
        id = user.GetId(),
        name = user.GetName(),
        givenName = user.GetGivenName(),
        email = user.GetEmail(),
    })
    .RequireAuthorization();

app.MapGet("/test/admin", () => "admin OK!")
    .RequireAuthorization("admin");

app.MapGet("/test/tech", () => "tech OK!")
    .RequireAuthorization("tech");

app.Run();