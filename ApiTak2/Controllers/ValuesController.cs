using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiTak2.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		[Authorize]
		public ActionResult Get()
		{
			return Ok(new List<string> { "test", "test1" });
		}

		[Authorize]
		public ActionResult GetAll()
		{
			return Ok(new List<string> { "test11", "test12" });
		}
	}
}
