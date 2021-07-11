using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;
using Fluxor;

namespace BlazingMongoIddict.Client.Store.Weather
{
	public record WeatherState
	{
		private readonly IReadOnlyDictionary<int, IEnumerable<WeatherForecast>> _forecasts;

		public bool IsLoading => !Contains(Index);
		
		public int Index { get; init; }

		public bool Contains(int index) => _forecasts.ContainsKey(index);

		public IEnumerable<WeatherForecast> Forecasts => IsLoading ? default : _forecasts[Index];

		internal IReadOnlyDictionary<int, IEnumerable<WeatherForecast>> AddForecastResults(int index, IEnumerable<WeatherForecast> forecasts) =>
			new Dictionary<int, IEnumerable<WeatherForecast>>(
				_forecasts.Append(
					new KeyValuePair<int, IEnumerable<WeatherForecast>>(index, forecasts)));

		public WeatherState(int index = 0, IReadOnlyDictionary<int, IEnumerable<WeatherForecast>> forecasts = null)
		{
			Index = index;
			_forecasts = forecasts ?? new Dictionary<int, IEnumerable<WeatherForecast>>();
		}
	}

	internal record LoadPageAction(int Index = 0);

	internal record FetchDataAction : LoadPageAction
	{
		public FetchDataAction(int index) : base(index) { }
	}

	internal record FetchDataResultAction(IEnumerable<WeatherForecast> Forecasts, int Index = 0);

	internal static class Reducers
	{
		[ReducerMethod]
		public static WeatherState ReduceLoadPageAction(WeatherState state, LoadPageAction action) =>
			state with { Index = action.Index };

		[ReducerMethod]
		public static WeatherState ReduceFetchDataResultAction(WeatherState state, FetchDataResultAction action) =>
			new(state.Index, state.AddForecastResults(action.Index, action.Forecasts));
	}

	internal class Feature : Feature<WeatherState>
	{
		public override string GetName() => "Weather";
		protected override WeatherState GetInitialState() =>
			new();
	}

	internal class Effects
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
