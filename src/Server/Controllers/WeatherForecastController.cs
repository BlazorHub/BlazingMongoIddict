using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazingMongoIddict.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries =
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		[HttpGet]
		[HttpGet("{id:int}")]
		public async Task<ActionResult<IEnumerable<WeatherForecast>>> Get(int id = 0)
		{
			// Delay for 1 second to simulate loading
			await Task.Delay(1000, HttpContext.RequestAborted);
			return Ok(Enumerable
				.Range(id, 5)
				.Select(i => new WeatherForecast(DateTime.Now.AddDays(i), new Temperature(Random.Shared.Next(-20, 55)),
					Summaries[Random.Shared.Next(Summaries.Length)]))
				.ToArray());
		}
	}
}
