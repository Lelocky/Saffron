using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Spice.DiscordClient.DependencyInjection;
using Spice.Saffron;
using Spice.Saffron.Areas.Identity;
using Spice.Saffron.Configuration.Options;
using Spice.Saffron.Data;
using Spice.Saffron.Factories;
using Spice.Saffron.Services;
using System.Configuration;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDefaultIdentity<ApplicationUser>(
        options =>
        {

        })
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<DiscordBotSettings>(builder.Configuration.GetSection("DiscordBot"));
builder.Services.Configure<DiscordGuildSettings>(builder.Configuration.GetSection("DiscordGuild"));

builder.Services.AddDiscordService(options =>
{
    var discordbotConfig = builder.Configuration.GetSection("DiscordBot").Get<DiscordBotSettings>();

    options.Version = 9;
    options.BotToken = discordbotConfig.BotToken;
});

builder.Services.AddSingleton<IDiscordClaimsService, DiscordClaimsService>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>,AdditionalUserClaimsPrincipalFactory>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
builder.Services.AddAuthentication()
    .AddDiscord(options =>
    {
        var discordOAuthConfig = builder.Configuration.GetSection("DiscordOAuth").Get<DiscordOAuthSettings>();

        options.ClientId = discordOAuthConfig.ClientId;
        options.ClientSecret = discordOAuthConfig.ClientSecret;
        //options.SaveTokens = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagementOnly", policy => policy.RequireRole("Officer", "Leader"));
    options.AddPolicy("MemberOnly", policy => policy.RequireRole("Member", "Coconut Farmer", "Founder", "Officer", "Leader"));
    options.AddPolicy("CommunityOnly", policy => policy.RequireRole("Member", "Coconut Farmer", "Founder", "Officer", "Leader", "Company Friend"));
});

builder.Services.AddAntDesign();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
