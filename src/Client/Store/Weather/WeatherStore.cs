using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;
using Fluxor;

namespace BlazingMongoIddict.Client.Store.Weather
{
	public class WeatherState
	{
		public bool IsLoading { get; }
		public IEnumerable<WeatherForecast> Forecasts { get; }

		public WeatherState(bool isLoading = false, IEnumerable<WeatherForecast> forecasts = null)
		{
			IsLoading = isLoading;
			Forecasts = forecasts;
		}
	}

	public record FetchDataAction;

	public record FetchDataResultAction(IEnumerable<WeatherForecast> Forecasts);

	public static class Reducers
	{
		[ReducerMethod]
		public static WeatherState ReduceFetchDataAction(WeatherState state, FetchDataAction action) =>
			new(true);

		[ReducerMethod]
		public static WeatherState ReduceFetchDataResultAction(WeatherState state, FetchDataResultAction action) =>
			new(forecasts: action.Forecasts);
	}

	public class Feature : Feature<WeatherState>
	{
		public override string GetName() => "Weather";
		protected override WeatherState GetInitialState() =>
			new();
	}

	public class Effects
	{
		private readonly HttpClient _http;

		public Effects(HttpClient http)
		{
			_http = http;
		}

		[EffectMethod]
		public async Task HandleFetchDataAction(FetchDataAction action, IDispatcher dispatcher)
		{
			dispatcher.Dispatch(new FetchDataResultAction(await _http.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast")));
		}
	}
}
