using LI4.Client.Pages;
using LI4.Common;
using LI4.Components;
using LI4.Controllers;
using LI4.Controllers.DAO;
using LI4.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options => {
//        options.LoginPath = "/login";
//        //options.AccessDeniedPath = "/access-denied";
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
//    });

//builder.Services.AddAuthorization(options => {
//    options.FallbackPolicy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//});
//builder.Services.AddCascadingAuthenticationState();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddSignalR(e => {
    e.EnableDetailedErrors = true;
});

builder.Services.AddScoped<UserDAO>(sp => new UserDAO(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IMineBuildsLN>(sp => new MineBuildsLN(builder.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseWebAssemblyDebugging();
} else {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapHub<ChatHub>(ChatHub.URL);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LI4.Client._Imports).Assembly);

Console.WriteLine("Starting to load static data...");
await MineBuildsLN.initStaticDataAsync(builder.Configuration);
Console.WriteLine("Loaded static data!");

Console.WriteLine("Starting to recover orders and assembly lines...");
await MineBuildsLN.initDynamicDataAsync(builder.Configuration);
Console.WriteLine("Loaded orders and assembly lines!");

app.Run();
