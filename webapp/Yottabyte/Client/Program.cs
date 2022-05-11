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
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Tewr.Blazor.FileReader;

namespace Yottabyte.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddHttpClient("AuthServerAPI",
                client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddHttpClient("ServerAPI",
                client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
           // .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddScoped<IUsersServices, UserServices>();
            
            builder.Services.AddFileReaderService(options => {
                options.UseWasmSharedBuffer = true;
            });

            await builder.Build().RunAsync();
        }
    }
}
