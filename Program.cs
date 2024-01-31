using Microsoft.EntityFrameworkCore;
using ProductsCRUDMVC.Data;
using ProductsCRUDMVC.Interfaces;
using ProductsCRUDMVC.Repository;
using ProductsCRUDMVC.Models;
using Microsoft.AspNetCore.Identity;
using ProductsCRUDMVC.Areas.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//configurer l'utilisation d'une base de donn�es SQL Server pour  l'application
//Etablir une connection avec une base de donn�e 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});

builder.Services.AddDbContext<ProductsCRUDMVCContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});


builder.Services.AddDefaultIdentity<client>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ProductsCRUDMVCContext>();

builder.Services.AddLogging(builder =>
{
	builder.AddConsole();
	builder.AddDebug();
});


//Ajouter les services de Product
builder.Services.AddScoped<IProductRepository, ProductRepository>();

//Ajouter les services de Client
//builder.Services.AddScoped<IClientRepository, ClientRepository>();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
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
app.UseAuthentication();

app.UseAuthorization();


app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();