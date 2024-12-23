using SIHOT.Wallet.API.Configs;
using SIHOT.Wallet.API.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ApplePassService, ApplePassService>();

// Configure CORS to allow all origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

AppleWalletConfig.LoadSecrets(app.Configuration, "Apple");
GoogleWalletConfig.LoadSecrets(app.Configuration, "Google");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseHttpsRedirection();

// Use the configured CORS policy
app.UseCors("AllowAllOrigins");

app.UseAuthorization();

app.MapControllers();

// Configure Kestrel to listen on all IPs (0.0.0.0) and a specific port (e.g., 5143)
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5143); // Listen on all network interfaces on port 5143
});

app.Run();
