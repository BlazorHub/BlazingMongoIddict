using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;
using Fluxor;
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
				.AddTransient<IZipCodeValidator, ZipCodeValidator>()
				.AddHttpClient("BlazingMongoIddict.ServerAPI",
					client => client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/"));

			// Supply HttpClient instances that include access tokens when making requests to the server project
			builder.Services
				.AddFluxor(o => o
					.ScanAssemblies(typeof(Program).Assembly)
#if DEBUG // Only use RDT in DEBUG mode
					.UseReduxDevTools(rdt =>
					{
						rdt.Name = "BlazingMongoIddict";
						rdt.UseSystemTextJson(_ =>
							new JsonSerializerOptions
							{
								PropertyNamingPolicy = JsonNamingPolicy.CamelCase
							});
					})
#endif
				)
				.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazingMongoIddict.ServerAPI"));

			return builder
				.Build()
				.RunAsync();
		}
	}
}
