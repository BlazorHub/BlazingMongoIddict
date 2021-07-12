using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;
using BlazingMongoIddict.Server.Validators;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BlazingMongoIddict.Server
{
	internal class Program
	{
		private static Task Main(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder => webBuilder
					.ConfigureServices((context, services) =>
					{
						services
							.AddTransient<IZipCodeValidator, ZipCodeValidator>()
							.AddAuthentication();
						services
							.AddControllersWithViews()
							.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddressValidator>());
						services.AddRazorPages();
					})
					.Configure((context, app) =>
					{
						if (context.HostingEnvironment.IsDevelopment())
						{
							app
								.UseDeveloperExceptionPage()
								.UseWebAssemblyDebugging();
						}
						else
						{
							// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
							app
								.UseExceptionHandler("/Error")
								.UseHsts();
						}

						app
							.UseHttpsRedirection()
							.UseBlazorFrameworkFiles()
							.UseStaticFiles()
							.UseRouting()
							.UseAuthentication()
							.UseAuthorization()
							.UseEndpoints(endpoints =>
							{
								endpoints.MapRazorPages();
								endpoints.MapControllers();
								endpoints.MapFallbackToFile("index.html");
							});
					}))
				.RunConsoleAsync();
	}
}
