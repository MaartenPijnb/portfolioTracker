using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Client;
using PortfolioTracker.Model;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("PortfolioTracker.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
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
// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("PortfolioTracker.ServerAPI"));

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("api://api.id.uri/access_as_user");
});

await builder.Build().RunAsync();
