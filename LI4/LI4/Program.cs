using LI4.Client.Pages;
using LI4.Common;
using LI4.Components;
using LI4.Controllers;
using LI4.Controllers.DAO;
using LI4.Hubs;

var builder = WebApplication.CreateBuilder(args);

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

app.MapHub<ChatHub>(ChatHub.URL);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(LI4.Client._Imports).Assembly);

await MineBuildsLN.initStaticData(builder.Configuration);

app.Run();
