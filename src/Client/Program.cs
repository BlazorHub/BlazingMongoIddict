using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlazingMongoIddict.Client
{
	internal class Program
	{
		private static Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("#app");

			builder.Services
				.AddHttpClient("BlazingMongoIddict.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
				.AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

			// Supply HttpClient instances that include access tokens when making requests to the server project
			builder.Services
				.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazingMongoIddict.ServerAPI"))
				.AddApiAuthorization();

			return builder
				.Build()
				.RunAsync();
		}
	}
}
