using FinApp.Data;
using FinApp.Services;

var builder = WebApplication.CreateBuilder(args);

//for checking only
var tokenSecret = builder.Configuration["JwtSettings:TokenSecret"];

var connstr = builder.Configuration["ConnectionStrings:DefaultConnection"];

// Add services to the container
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<DbContext>(provider =>
{
    var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"] ?? throw new Exception("connection string not found");
    return new DbContext(connectionString); // Your DbContext constructor expects a connection string
});

builder.Services.AddScoped<JwtService>(provider =>
{
    var jwtSettings = new JwtSetting { TokenSecret = builder.Configuration["JwtSettings:TokenSecret"] ?? throw new Exception("tokensecret can't be empty") };
    return new JwtService(jwtSettings);
});

builder.Services.AddControllers(config =>
{
    config.Filters.Add(new JwtAuthenticationFilter("jwttoken", tokenSecret));
});

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

app.MapControllerRoute(name: "default", pattern: "{controller=Auth}/{action=Login}/{id?}").WithStaticAssets();

app.Run();
