using library.Core.Seeds;
using library.Data.Models;
using library.Infrastructure.AutoMapper;
using library.Infrastructure.Services.Authors;
using library.Infrastructure.Services.Books;
using library.Infrastructure.Services.Copies;
using library.Web;
using Microsoft.AspNetCore.Identity;
using System.Reflection;
using UoN.ExpressiveAnnotations.NetCore.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using library.Data;
using library.Core.Services.Images;
using Microsoft.AspNetCore.Identity.UI.Services;
using library.Web.Services.Email;
using Microsoft.AspNetCore.DataProtection;
using WhatsAppCloudApi.Extensions;
using Hangfire;
using Hangfire.Dashboard;
using WhatsAppCloudApi.Services;
using library.Web.Tasks;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<libraryDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<libraryDbContext>();

//builder.Services.ConfigureApplicationCookie(option =>
//{
//    option.AccessDeniedPath = "";

//});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(
    options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<libraryDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;
    //options.User.AllowedUserNameCharacters = "abc**";
    options.User.RequireUniqueEmail = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
    options.Lockout.MaxFailedAccessAttempts = 4;
});
builder.Services.AddDataProtection().SetApplicationName(nameof(library));
builder.Services.AddControllersWithViews();
builder.Services.AddExpressiveAnnotations();

//mapper
builder.Services.AddAutoMapper(Assembly.GetAssembly(typeof(MapperProfile)));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection(nameof(CloudinarySettings)));
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));

// Configure My Services 
builder.Services.AddWhatsAppApiClient(builder.Configuration);
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ICopyService, CopyService>();
builder.Services.AddScoped<IUser, UserService>();
builder.Services.AddTransient<IImageService, ImageService>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailBodyBuilder, EmailBodyBuilder>();

//Add Hangfire
builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();


builder.Services.Configure<AuthorizationOptions>(options =>
options.AddPolicy("AdminsOnly", policy =>
{
    policy.RequireAuthenticatedUser();
    policy.RequireRole(AppRoles.Admin);
}));

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();

using var scope = scopeFactory.CreateScope();

var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var userManger = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

await DefaultRoles.SeedRoles(roleManger);
await DefaultUsers.SeedAdminUser(userManger);


//hangfire
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Library Dashboard",
    //IsReadOnlyFunc = (DashboardContext context) => true,
    Authorization = new IDashboardAuthorizationFilter[]
    {
        new HangfireAuthorizationFilter("AdminsOnly")
    }
});

var dbContext = scope.ServiceProvider.GetRequiredService<libraryDbContext>();
var webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
var whatsAppClient = scope.ServiceProvider.GetRequiredService<IWhatsAppClient>();
var emailBodyBuilder = scope.ServiceProvider.GetRequiredService<IEmailBodyBuilder>();
var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

var hangfireTasks = new HangfireTasks(dbContext, webHostEnvironment, whatsAppClient,
    emailBodyBuilder, emailSender);

RecurringJob.AddOrUpdate(() => hangfireTasks.PrepareExpirationAlert(), "0 14 * * *");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
