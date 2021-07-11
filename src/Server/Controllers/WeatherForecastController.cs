﻿using System;
using System.Collections.Generic;
using System.Linq;
using BlazingMongoIddict.Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazingMongoIddict.Server.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries =
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		[HttpGet]
		[HttpGet("{id:int}")]
		public ActionResult<IEnumerable<WeatherForecast>> Get(int id = 0)
		{
			return Ok(Enumerable
				.Range(id, 5)
				.Select(i => new WeatherForecast(DateTime.Now.AddDays(i), new Temperature(Random.Shared.Next(-20, 55)),
					Summaries[Random.Shared.Next(Summaries.Length)]))
				.ToArray());
		}
	}
}
