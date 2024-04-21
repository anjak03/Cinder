using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cinder.Data;
using Microsoft.AspNetCore.Identity;
using Cinder.Models;
using Cinder.Seeders;
using SendGrid;
using SendGrid.Helpers.Mail;
using dotenv.net;

DotEnv.Load(); 

var builder = WebApplication.CreateBuilder(args);

var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
var client = new SendGridClient(sendGridApiKey);

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationContext") ?? throw new InvalidOperationException("Connection string 'ApplicationContext' not found.")));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddSassCompiler();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.WithOrigins("https://localhost:7058") 
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<MessageService>(); 
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddSingleton<ISendGridClient>(client);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationContext>();
    DbInitializer.Initialize(context);
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    
    await UserSeeder.SeedRolesAsync(roleManager);
    LanguageSeeder.SeedAllLanguages(context);
    FacultySeeder.SeedAllFaculties(context);
    HobbySeeder.SeedAllHobbies(context);
}

app.UseExceptionHandler("/Home/Error");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();