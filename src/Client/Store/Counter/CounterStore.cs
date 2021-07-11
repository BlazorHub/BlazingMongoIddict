using Fluxor;

namespace BlazingMongoIddict.Client.Store.Counter
{
	// State needs to be a class for reference equality
	public class CounterState
	{
		public CounterState(int count = 0)
		{
			Count = count;
		}

		public int Count { get; }
	}

	// Action(s) can be records for simplicity because equality is not used
	public record ChangeCounterAction(int Number);
	
	// Reducer methods must be static
	public static class Reducers
	{
		[ReducerMethod]
		public static CounterState ReduceChangeCounterAction(CounterState state, ChangeCounterAction action) =>
			new(state.Count + action.Number);
	}

	// Initial state provided by Feature
	public class Feature : Feature<CounterState>
	{
		public override string GetName() => "Counter";
		protected override CounterState GetInitialState() => new();
	}
}
