using Microsoft.EntityFrameworkCore;
using NitStore.Data;
using NitStore.Service;
using NitStore.Service.ServiceImplement;

var builder = WebApplication.CreateBuilder(args);



// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<NitDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CompanyDB")));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSession(option =>
{
    option.IdleTimeout = TimeSpan.FromMinutes(120);
});
builder.Services.AddScoped<IVnPayService, VnPayServiceImplement>();

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
app.UseSession();
app.UseRouting();


app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Authen}/{action=Login}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=ListProduct}");
app.Run();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}");
app.Run();