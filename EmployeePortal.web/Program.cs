using EmployeePortal.Data;
using EmployeePortal.Models;
using EmployeePortal.Repositories;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

using Rotativa.AspNetCore;
using ServiceStack;
using ServiceStack.Text;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.Cookie.Name = "MyCookieAuth";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization(); // Add this too

builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IPayslipRepository, PayslipRepository>();



AppContext.SetSwitch("EPPlus.LicenseContext", true);




var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // shows full error
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

RotativaConfiguration.Setup(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Rotativa"));




app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
SeedEmployees(app);



app.Run();

void SeedEmployees(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();

    // ✅ Seed only if no employees exist
    if (!context.Employees.Any())
    {
        var sampleEmployees = new List<Employee>
        {
            new Employee { FullName = "Alice Johnson", Email = "alice@example.com", Position = "HR", Department = "HR", HireDate = DateTime.Now.AddYears(-2), DateOfBirth = new DateTime(1990, 5, 10), Gender = Gender.Female, EmployeeType = EmployeeType.Permanent, Salary = 50000 },
            new Employee { FullName = "Bob Smith", Email = "bob@example.com", Position = "Developer", Department = "IT", HireDate = DateTime.Now.AddYears(-1), DateOfBirth = new DateTime(1988, 3, 15), Gender = Gender.Male, EmployeeType = EmployeeType.Permanent, Salary = 70000 },
            new Employee { FullName = "Cathy Brown", Email = "cathy@example.com", Position = "Tester", Department = "QA", HireDate = DateTime.Now.AddMonths(-18), DateOfBirth = new DateTime(1992, 7, 20), Gender = Gender.Female, EmployeeType = EmployeeType.Contract, Salary = 45000 },
            new Employee { FullName = "David Lee", Email = "david@example.com", Position = "Designer", Department = "UX", HireDate = DateTime.Now.AddMonths(-12), DateOfBirth = new DateTime(1993, 8, 12), Gender = Gender.Male, EmployeeType = EmployeeType.Contract, Salary = 48000 },
            new Employee { FullName = "Emma Watson", Email = "emma@example.com", Position = "Manager", Department = "Operations", HireDate = DateTime.Now.AddMonths(-8), DateOfBirth = new DateTime(1985, 11, 1), Gender = Gender.Female, EmployeeType = EmployeeType.Permanent, Salary = 90000 },
            new Employee { FullName = "Frank Green", Email = "frank@example.com", Position = "Support", Department = "IT", HireDate = DateTime.Now.AddMonths(-6), DateOfBirth = new DateTime(1991, 4, 18), Gender = Gender.Male, EmployeeType = EmployeeType.Contract, Salary = 42000 },
            new Employee { FullName = "Grace Hopper", Email = "grace@example.com", Position = "Architect", Department = "IT", HireDate = DateTime.Now.AddYears(-3), DateOfBirth = new DateTime(1980, 12, 9), Gender = Gender.Female, EmployeeType = EmployeeType.Permanent, Salary = 100000 }
        };

        context.Employees.AddRange(sampleEmployees);
        context.SaveChanges();
    }
}




