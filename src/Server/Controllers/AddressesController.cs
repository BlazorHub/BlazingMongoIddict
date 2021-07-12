using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazingMongoIddict.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AddressesController : ControllerBase
	{
		[HttpPost]
		public async Task<ActionResult<AddressResponse>> PostAsync([FromBody] AddressRequest request)
		{
			await Task.Delay(500, HttpContext.RequestAborted);
			// TODO: Add automapper to simplify mapping
			// TODO: Add mediatr to simplify database call to look up city, state, & coordinates by zip
			return Ok(new AddressResponse
			{
				Line1 = request.Line1,
				Line2 = request.Line2,
				City = "Austin",
				State = "TX",
				Zip = request.Zip,
				Longitude = 100,
				Latitude = 100
			});
		}
	}
}
