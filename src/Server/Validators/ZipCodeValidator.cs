using System.Threading;
using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;

namespace BlazingMongoIddict.Server.Validators
{
	internal class ZipCodeValidator : IZipCodeValidator
	{
		public async Task<bool> IsValidAsync(string zip, CancellationToken cancellationToken = default)
		{
			await Task.Delay(500, cancellationToken);
			return zip == "78704"; // TODO: Wire to database
		}
	}
}
