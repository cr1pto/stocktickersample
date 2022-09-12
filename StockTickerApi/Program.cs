using Microsoft.AspNetCore.Authentication.Certificate;
using StockTicker.Lib.Common.Adapters;
using StockTicker.Lib.Common.Interfaces;
using StockTicker.Lib.Common.Models;
using System.Security.Claims;

namespace StockTickerApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var stockTickerApiConfigurationSection = builder.Configuration.GetSection(nameof(StockTickerApiConfiguration));

        builder.Services.AddCors(opts =>
        {
            var validOrigins = new string[]
            {
                "http://localhost:5000",
                "https://localhost:5001",
                "http://localhost:5002",
                "https://localhost:5003",
                "http://stocktickerapi:5002",
                "https://stocktickerapi:5003",
            };

            opts.AddDefaultPolicy((policy) =>
            {
                policy.WithOrigins(validOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
           .AddCertificate(options =>
           {
               options.Events = new CertificateAuthenticationEvents
               {
                   OnCertificateValidated = context =>
                   {
                       var claims = new[]
                       {
                                    new Claim(ClaimTypes.NameIdentifier, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer),
                                    new Claim(ClaimTypes.Name, context.ClientCertificate.Subject, ClaimValueTypes.String, context.Options.ClaimsIssuer)
                       };

                       context.Principal = new ClaimsPrincipal(new ClaimsIdentity(claims, context.Scheme.Name));
                       context.Success();

                       return Task.CompletedTask;
                   }
               };
               // Adding a ICertificateValidationCache will result in certificate auth caching the results, the default implementation uses a memory cache
           }).AddCertificateCache();

        builder.Services.AddAuthorization();

        // Add services to the container.
        builder.Services.AddOptions<StockTickerApiConfiguration>()
            .Bind(stockTickerApiConfigurationSection)
            .ValidateDataAnnotations();
        builder.Services.AddHttpClient<IFinanceApiAdapter, FinanceApiAdapter>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            });

        builder.Services.AddControllers();

        builder.Services.AddHealthChecks();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapHealthChecks("/health");

        app.Run();
    }
}