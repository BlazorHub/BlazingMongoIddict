using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace BlazingMongoIddict.Client.Models
{
	public class AddressRequest
	{
		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string Zip { get; set; }
	}

	public class AddressResponse : AddressRequest
	{
		public string City { get; set; }
		public string State { get; set; }

		public decimal? Latitude { get; set; }
		public decimal? Longitude { get; set; }
	}

	// Validator that is shared between the browser and server
	public class AddressValidator : AbstractValidator<AddressRequest>
	{
		public AddressValidator(IZipCodeValidator zipCodeValidator)
		{
			RuleFor(a => a.Line1)
				.NotEmpty();

			RuleFor(a => a.Zip)
				.Cascade(CascadeMode.Stop) // Do not run async validation until previous validations succeed
				.NotEmpty()
				.Matches(@"^\d{5}$")
				.MustAsync(zipCodeValidator.IsValidAsync)
				.WithMessage(a => $"'Zip' {a.Zip} is not a valid USPS ZIP Code");
		}
	}

	// Interface that will be replaced by separate implementations for client & server
	public interface IZipCodeValidator
	{
		Task<bool> IsValidAsync(string zip, CancellationToken cancellationToken = default);
	}

	// Client validator will query the API controller and return false on 404
	internal class ZipCodeValidator : IZipCodeValidator
	{
		private readonly HttpClient _httpClient;

		public ZipCodeValidator(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<bool> IsValidAsync(string zip, CancellationToken cancellationToken = default)
		{
			var response = await _httpClient.GetAsync($"ZipCodes/{zip}", cancellationToken);
			return response.IsSuccessStatusCode;
		}
	}
}
