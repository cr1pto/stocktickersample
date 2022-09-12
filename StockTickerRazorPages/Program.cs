using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using StockTicker.Lib.Common.Adapters;
using StockTicker.Lib.Common.Interfaces;
using StockTicker.Lib.Common.Models;
using StockTicker.Lib.Common.Services;
using System.Globalization;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var stockTickerApiConfigurationSection = builder.Configuration.GetSection(nameof(StockTickerApiConfiguration));
Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

builder.Services.AddCors(opts =>
{
    var validOrigins = new string[]
    {
        //in a production application, we could move this to an environment variable
        // or an appsettings.json/dockerfile.override, launchSettings.json, etc
        "http://localhost:5000",
        "https://localhost:5001",
        "http://localhost:5002",
        "https://localhost:5003",
        "http://stocktickerapi:5002",
        "https://stocktickerapi:5003",
        "https://stocktickerapi",
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
builder.Services.AddLocalization();
builder.Services.AddOptions<StockTickerApiConfiguration>()
    .Bind(stockTickerApiConfigurationSection)
    .ValidateDataAnnotations();
builder.Services.AddTransient<IStockTickParserService, StockTickParserService>();
builder.Services.AddHttpClient<IStockTickerApiClient, StockTickerApiClient>()
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler()
        {
            //development only!
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    });

// Add services to the container.
builder.Services.AddRazorPages()
        .AddRazorOptions(options =>
        {
            options.ViewLocationFormats.Add("/{0}.cshtml");
        });

builder.Services.AddHealthChecks();

var app = builder.Build();

var stockTickConfig = app.Services.GetService<IOptions<StockTickerApiConfiguration>>();


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

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

IList<CultureInfo> supportedCultures = new List<CultureInfo>
    {
        new CultureInfo("en-US"),
    };

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.MapRazorPages();

app.MapHealthChecks("/health");

app.Run();
