using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Yottabyte.Client.Services;
using Havit.Blazor.Components.Web;
using Havit.Blazor.Components.Web.Bootstrap;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Yottabyte.Client
{
    public class Program
    {
        public class Auth0AuthorizationMessageHandler : AuthorizationMessageHandler
        {
            public Auth0AuthorizationMessageHandler(
                IAccessTokenProvider provider,
                NavigationManager navigation)
                : base(provider, navigation)
            {
                var url = "https://yottabyte-test.eu.auth0.com/";
                ConfigureHandler(authorizedUrls: new[] { url });
            }
        }

        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddTransient<Auth0AuthorizationMessageHandler>();

            builder.Services.AddHttpClient("ServerAPI",
                client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            //.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddHttpClient("Auth0API",
                client => client.BaseAddress = new Uri("https://yottabyte-test.eu.auth0.com/"));
                //.AddHttpMessageHandler<Auth0AuthorizationMessageHandler>();

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddScoped<ISuperHeroService, SuperHeroService>();

            builder.Services.AddHxServices();

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind("Auth0", options.ProviderOptions);
                options.ProviderOptions.ResponseType = "code";
            });

            await builder.Build().RunAsync();
        }
    }
}
