using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cinder.Data;
using Microsoft.AspNetCore.Identity;
using Cinder.Models;
using Cinder.Seeders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext") ?? throw new InvalidOperationException("Connection string 'ApplicationContext' not found.")));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddSassCompiler();

// Add SignalR for messaging
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.WithOrigins("https://localhost:7058") 
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

builder.Services.AddScoped<MessageService>(); 
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

var app = builder.Build();


using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    DbInitializer.Initialize(context);
}

using(var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
    var context = serviceProvider.GetRequiredService<ApplicationContext>();

    await UserSeeder.SeedRolesAsync(roleManager);
    LanguageSeeder.SeedAllLanguages(context);
    FacultySeeder.SeedAllFaculties(context);
    HobbySeeder.SeedAllHobbies(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors("CorsPolicy");
app.MapHub<ChatHub>("/chatHub");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();
app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
