using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;
using Fluxor;

namespace BlazingMongoIddict.Client.Store.Weather
{
	// Record here to leverage the with syntax from C# 9.0
	public record WeatherState
	{
		// State instance variable not to be bound to so made private
		private readonly IReadOnlyDictionary<int, IEnumerable<WeatherForecast>> _forecasts;

		// Helper method to see if the current page is loading or not
		public bool IsLoading => !Contains(Index);

		// Current date index
		public int Index { get; init; }

		// Function to see if the state already has the given date range loaded
		public bool Contains(int index) => _forecasts.ContainsKey(index);

		// Property to databind in the UI
		public IEnumerable<WeatherForecast> Forecasts => IsLoading ? default : _forecasts[Index];

		// Helper function to return back a new Dictionary with the added index & results
		internal IReadOnlyDictionary<int, IEnumerable<WeatherForecast>> AddForecastResults(int index,
			IEnumerable<WeatherForecast> forecasts) =>
			new Dictionary<int, IEnumerable<WeatherForecast>>(
				_forecasts.Append(
					new KeyValuePair<int, IEnumerable<WeatherForecast>>(index, forecasts)));

		// Constructor to build the state
		public WeatherState(int index = 0, IReadOnlyDictionary<int, IEnumerable<WeatherForecast>> forecasts = null)
		{
			Index = index;
			_forecasts = forecasts ?? new Dictionary<int, IEnumerable<WeatherForecast>>();
		}
	}

	// This action will only fire the reducer to move the index it won't trigger any async load
	internal record LoadPageAction(int Index = 0);

	// This action will both fire the reducer to move the index as well as dispatch the fetch action from the API
	internal record FetchDataAction : LoadPageAction
	{
		public FetchDataAction(int index) : base(index)
		{
		}
	}

	// This action will only fire the reducer to add the new forecasts & date index to the cache
	internal record FetchDataResultAction(IEnumerable<WeatherForecast> Forecasts, int Index = 0);

	internal static class Reducers
	{
		// This reducer method only sets the current page and because we use inheritance it will fire for both
		// FetchDataAction & LoadPageAction
		[ReducerMethod]
		public static WeatherState ReduceLoadPageAction(WeatherState state, LoadPageAction action) =>
			state with {Index = action.Index};

		// This reducer method only adds the results to the cache it doesn't move the page
		[ReducerMethod]
		public static WeatherState ReduceFetchDataResultAction(WeatherState state, FetchDataResultAction action) =>
			new(state.Index, state.AddForecastResults(action.Index, action.Forecasts));
	}

	// Not sure why they chose Feature for the name but this provides a name & initial state
	internal class Feature : Feature<WeatherState>
	{
		public override string GetName() => "Weather";

		protected override WeatherState GetInitialState() =>
			new();
	}

	// Side effect producing operations (i.e. going back to the API)
	internal class Effects
	{
		private readonly HttpClient _http;

		public Effects(HttpClient http)
		{
			_http = http;
		}

		// Side effect producing function that loads data from the server
		[EffectMethod]
		public async Task HandleFetchDataAction(FetchDataAction action, IDispatcher dispatcher) =>
			dispatcher.Dispatch(new FetchDataResultAction(
				await _http.GetFromJsonAsync<WeatherForecast[]>($"WeatherForecast/{action.Index}"), action.Index));
	}
}
