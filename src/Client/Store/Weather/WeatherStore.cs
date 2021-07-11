using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;
using Fluxor;

namespace BlazingMongoIddict.Client.Store.Weather
{
	public class WeatherState
	{
		public bool IsLoading => Forecasts is null || !Forecasts.ContainsKey(Index);
		public int Index { get; }

		public IEnumerable<WeatherForecast> CurrentForecasts => IsLoading ? default : Forecasts[Index];

		public IReadOnlyDictionary<int, IEnumerable<WeatherForecast>> Forecasts { get; } 

		public WeatherState(int index = 0, IReadOnlyDictionary<int, IEnumerable<WeatherForecast>> forecasts = null)
		{
			Index = index;
			Forecasts = forecasts ?? new Dictionary<int, IEnumerable<WeatherForecast>>();
		}
	}

	public record LoadPageAction(int Index = 0);

	public record FetchDataAction : LoadPageAction
	{
		public FetchDataAction(int index) : base(index) { }
	}

	public record FetchDataResultAction(IEnumerable<WeatherForecast> Forecasts, int Index = 0);

	public static class Reducers
	{
		[ReducerMethod]
		public static WeatherState ReduceLoadPageAction(WeatherState state, LoadPageAction action) =>
			new(action.Index, state.Forecasts);
		
		[ReducerMethod]
		public static WeatherState ReduceFetchDataResultAction(WeatherState state, FetchDataResultAction action) =>
			new(action.Index, new Dictionary<int, IEnumerable<WeatherForecast>>(
				state.Forecasts.Append(
					new KeyValuePair<int, IEnumerable<WeatherForecast>>(action.Index, action.Forecasts))));
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
		public async Task HandleFetchDataAction(FetchDataAction action, IDispatcher dispatcher) =>
			dispatcher.Dispatch(new FetchDataResultAction(await _http.GetFromJsonAsync<WeatherForecast[]>($"WeatherForecast/{action.Index}"), action.Index));
	}
}
