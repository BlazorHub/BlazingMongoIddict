using Fluxor;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;

namespace BlazingMongoIddict.Client.Components
{
	// Since state & dispatch are commonly injected provide a base class that provides both
	public abstract class FluxorDispatchComponent<T> : FluxorComponent where T : class
	{
		[Inject] private IDispatcher Dispatcher { get; set; }

		[Inject] private IState<T> StateContainer { get; set; }

		// Convenience property to provide direct access to the state
		protected T State => StateContainer?.Value;

		// Convenience method to handle the dispatch
		protected void Dispatch(object action) => Dispatcher.Dispatch(action);
	}
}
