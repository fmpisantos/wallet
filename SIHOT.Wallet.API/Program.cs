using SIHOT.Wallet.API.Configs;
using SIHOT.Wallet.API.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 5143);
});

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

builder.Configuration.AddEnvironmentVariables();

Console.WriteLine("Apple Config");
AppleWalletConfig.LoadEnvironmentVariables(app.Configuration, "Apple");
Console.WriteLine("Google Config");
GoogleWalletConfig.LoadEnvironmentVariables(app.Configuration, "Google");

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


app.Run();
