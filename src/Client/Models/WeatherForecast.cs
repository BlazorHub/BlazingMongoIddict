using System;

namespace BlazingMongoIddict.Client.Models
{

	public record Temperature(int Celsius)
	{
		public int Fahrenheit => 32 + (int) (Celsius / 0.5556m);
	}

	public record WeatherForecast(DateTime Date, Temperature Temperature, string Summary);
}
