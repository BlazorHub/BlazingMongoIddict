using System.Threading.Tasks;
using BlazingMongoIddict.Client.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlazingMongoIddict.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class ZipCodesController : ControllerBase
	{
		[HttpGet("{id:int:required}")]
		// ReSharper disable once RouteTemplates.ParameterTypeCanBeMadeStricter
		public async Task<ActionResult<ZipCode>> GetAsync(string id)
		{
			await Task.Delay(500, HttpContext.RequestAborted);
			// TODO: Wire to database
			if (id == "78704")
			{
				return Ok(new ZipCode("78704", "Austin", "TX", 100, 100));
			}

			return NotFound();
		}
	}
}
