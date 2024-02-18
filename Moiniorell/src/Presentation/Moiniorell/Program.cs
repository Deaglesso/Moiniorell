
using Moiniorell.Persistence.ServiceRegistration;
using Moiniorell.Application.ServiceRegistration;
using Moiniorell.Persistence.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddApplicationServices();



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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chatHub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Chat}/{action=Index}/{id?}");
});
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<OnlineUsersHub>("/onlineUsersHub");
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});


app.Run();
