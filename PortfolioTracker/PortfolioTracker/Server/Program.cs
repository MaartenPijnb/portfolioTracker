using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using PortfolioTracker.Degiro.Runner.Controller;
using PortfolioTracker.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddSwaggerGen();
#if DEBUG
builder.Services.AddDbContext<MPortfolioDBContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaijnzzzDB"), options => options.MigrationsAssembly("PortfolioTracker.Server"));
    });
#endif

#if RELEASE
builder.Services.AddDbContext<MPortfolioDBContext>(options => {
    options.UseSqlServer(Environment.GetEnvironmentVariable("SQLAZURECONNSTR_PaijnzzzDB"), options => options.MigrationsAssembly("PortfolioTracker.Server"));
    });
#endif 

builder.Services.AddTransient<IDegiroController, DegiroController>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");    
});

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
