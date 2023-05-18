using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using HospitalApp.Data;
using ServiceStack;
using HospitalApp.Configuration;
using HospitalApp.Services;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HospitalAppContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalAppContext") ?? throw new InvalidOperationException("Connection string 'HospitalAppContext' not found.")));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache(); // Add a distributed cache service for storing session data
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true; // Ensure the session cookie is only accessible via HTTP
    options.Cookie.IsEssential = true; // Make the session cookie essential to allow it during GDPR consent
});
//builder.Services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings")); //Connect krwaya jason or email class ko 
builder.Services.AddTransient<IMailService, MailServices>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
