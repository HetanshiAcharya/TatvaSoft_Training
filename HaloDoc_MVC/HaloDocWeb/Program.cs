using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.DataModels;
using HaloDocRepository.Repositories;
using HaloDocRepository.Interface;
using Microsoft.EntityFrameworkCore;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using HaloDocDataAccess.ViewModels;

var builder = WebApplication.CreateBuilder(args);
var emailConfig = builder.Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);// Add services to the container.
//builder.Services.AddDefaultIdentity<Microsoft.AspNetCore.Identity.IdentityUser>().AddEntityFrameworkStores<HaloDocDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HaloDocDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("HaloDocDbContext")));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IProviderService, ProviderService>();

builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });

builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".HaloDocweb.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Adjust as needed
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseNotyf();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
