using Dapper;
using FinApp.Data.CustomMapping;
using FinApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Register custom type handlers before building the application
SqlMapper.AddTypeHandler(typeof(UserRole), new EnumTypeHandler<UserRole>()); //to map the enum to string and vice versa
SqlMapper.AddTypeHandler(typeof(LicenseKeyType), new EnumTypeHandler<LicenseKeyType>()); //to map the enum to string and vice versa

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
