using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteBirthy.Data;
using VoteBirthy.Repositories;
using VoteBirthy.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllersWithViews(options => 
{
    // Set more detailed model binding errors for troubleshooting
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
        _ => "This field is required.");
    
    // Add ModelStateInvalidFilter to log model errors
    options.Filters.Add<ModelStateInvalidFilter>();
});

// Configure database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString)
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine, LogLevel.Information));

// Register Repositories
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IGiftRepository, GiftRepository>();
builder.Services.AddScoped<IVoteRepository, VoteRepository>();
builder.Services.AddScoped<IVoteOptionRepository, VoteOptionRepository>();
builder.Services.AddScoped<IVoteCastRepository, VoteCastRepository>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Services
builder.Services.AddScoped<IVoteService, VoteService>();
builder.Services.AddScoped<IMapperService, MapperService>();
// Gift service removed since gifts are handled through database seeding

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.SameSite = SameSiteMode.Lax;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        DbSeeder.SeedData(context).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database initialization.");
    }
}

app.Run();

// Model state invalid filter to log model binding errors
public class ModelStateInvalidFilter : IActionFilter
{
    private readonly ILogger<ModelStateInvalidFilter> _logger;

    public ModelStateInvalidFilter(ILogger<ModelStateInvalidFilter> logger = null)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            _logger?.LogWarning("Model state invalid in controller {Controller}, action {Action}", 
                context.RouteData.Values["controller"], 
                context.RouteData.Values["action"]);

            foreach (var state in context.ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    _logger?.LogWarning("Model validation error - Key: {Key}, Error: {Error}", 
                        state.Key, error.ErrorMessage);
                }
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after execution
    }
}
