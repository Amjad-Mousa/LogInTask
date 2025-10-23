using LogInTask;
using LogInTask.Services;
using LogInTask.Models;
using FluentValidation;
using ElectricMeterApp.Validators;
using LogInTask.Components;

var builder = WebApplication.CreateBuilder(args);

// ? Add Razor Components (Blazor Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ? Register your services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IElectricMeterService, ElectricMeterService>();

// ? Register FluentValidation validator
builder.Services.AddScoped<IValidator<MeterQueryRequest>, MeterQueryRequestValidator>();

// ? Register HttpClient (optional)
builder.Services.AddScoped<HttpClient>();

var app = builder.Build();

// ? Middleware setup
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
