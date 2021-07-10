using System.Threading.Tasks;
using BlazingMongoIddict.Server.Data;
using BlazingMongoIddict.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
							.AddDbContext<ApplicationDbContext>(options =>
								options.UseSqlite(context.Configuration.GetConnectionString("DefaultConnection")))
							.AddDatabaseDeveloperPageExceptionFilter()
							.AddDefaultIdentity<ApplicationUser>(options =>
								options.SignIn.RequireConfirmedAccount = true)
							.AddEntityFrameworkStores<ApplicationDbContext>();

						services
							.AddIdentityServer()
							.AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

						services
							.AddAuthentication()
							.AddIdentityServerJwt();

						services.AddControllersWithViews();
						services.AddRazorPages();
					})
					.Configure((context, app) =>
					{
						if (context.HostingEnvironment.IsDevelopment())
						{
							app
								.UseDeveloperExceptionPage()
								.UseMigrationsEndPoint()
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
							.UseIdentityServer()
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
