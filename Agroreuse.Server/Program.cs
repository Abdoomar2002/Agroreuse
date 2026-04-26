using Agroreuse.Application;
using Agroreuse.Application.Services;
using Agroreuse.Domain.Entities;
using Agroreuse.Infrastructure;
using Agroreuse.Infrastructure.Persistence;
using Agroreuse.Server.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure Identity first (without default authentication scheme)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Configure password rules here
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ArgoreuseContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secret = jwtSettings["Secret"];
var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    // Allow SignalR to receive token from query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("UserType", "Admin"));
    options.AddPolicy("Farmer", policy => policy.RequireClaim("UserType", "Farmer"));
    options.AddPolicy("Factory", policy => policy.RequireClaim("UserType", "Factory"));
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// Add SignalR
builder.Services.AddSignalR();

// Initialize Firebase Admin SDK
var firebaseCredentialPath = builder.Configuration["Firebase:CredentialPath"];
var temp=File.ReadAllText(firebaseCredentialPath);
if (!string.IsNullOrEmpty(firebaseCredentialPath) && File.Exists(firebaseCredentialPath))
{
    FirebaseApp.Create(new AppOptions
    {
        Credential = GoogleCredential.FromJson(temp)
    });
}
else
{
    // Try to initialize from environment variable or embedded JSON
    var firebaseJson = builder.Configuration["Firebase:CredentialJson"];
    if (!string.IsNullOrEmpty(firebaseJson))
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(firebaseJson)
        });
    }
}

// Register notification pushers
builder.Services.AddScoped<SignalRNotificationPusher>();
builder.Services.AddScoped<FcmNotificationPusher>();
builder.Services.AddScoped<INotificationPusher, CompositeNotificationPusher>();

var app = builder.Build();

// Apply pending migrations automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ArgoreuseContext>();
    dbContext.Database.Migrate();
}

app.UseDefaultFiles();
app.UseStaticFiles(); // Enable serving static files from wwwroot (including uploaded images)
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
}
app.MapOpenApi();
app.MapScalarApiReference();

app.UseCors(core =>
{
    core.AllowAnyHeader();
    core.AllowAnyMethod();
    core.AllowAnyOrigin();
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Map SignalR hubs
app.MapHub<Agroreuse.Server.Hubs.NotificationHub>("/hubs/notifications");

app.MapFallbackToFile("/index.html");

app.Run();

