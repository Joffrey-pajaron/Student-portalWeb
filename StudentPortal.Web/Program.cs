using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StudentPortal.Web.Data;
using StudentPortal.Web.Repositories; // ✅ Make sure spelling is correct
using StudentPortal.Web.Repostories;
using StudentPortal.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Services ------------------

// EF Core + Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// MVC + Session
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// ------------------ Dapper + Repositories ------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DapperContext singleton
builder.Services.AddSingleton<DapperContext>();

// Repositories using DapperContext
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<EnrollmentRepository>();
builder.Services.AddScoped<InstructorRepository>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<CourseSubjectServiceRepository>();
builder.Services.AddScoped<SubjectRepository>();
builder.Services.AddScoped<SubjectRepository>();
// Custom services
builder.Services.AddTransient<AuthService>();

// ------------------ Build App ------------------
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapRazorPages();

app.Run();
