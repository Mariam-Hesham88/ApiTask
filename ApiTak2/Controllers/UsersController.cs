using ApiTak2.Data.Entities; // Assuming your User entity is here
using ApiTak2.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiTak2.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IConfiguration _config;
		private readonly UserManager<User> _userManager;

		public UsersController(IConfiguration configuration, UserManager<User> userManager)
		{
			_config = configuration;
			_userManager = userManager;
		}

		[HttpPost]
		[Route("static-login")]
		public ActionResult StaticLogin(LoginViewModel model)
		{
			if (model.UserName != "test")
				return Unauthorized();

			// setting for claims
			var Claims = new List<Claim>
			{
				new Claim("Name" , model.UserName),
				new Claim (ClaimTypes.NameIdentifier , "Identifier")
			};

			// setting secret kay
			var stringKey = _config.GetValue<string>("SecretKey");
			var byteKey = Encoding.ASCII.GetBytes(stringKey);
			var key = new SymmetricSecurityKey(byteKey);

			var SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

			// setting token
			var jwt = new JwtSecurityToken(
				claims: Claims,
				signingCredentials: SigningCredentials,
				expires: DateTime.Now.AddMinutes(_config.GetValue<int>("TokenDuration")),
				notBefore: DateTime.Now
				);
			//convert into string
			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.WriteToken(jwt);

			return Ok(new
			{
				Token = token,
				expiration = DateTime.Now.AddMinutes(_config.GetValue<int>("TokenDuration"))
			});
		}

		[HttpPost]
		[Route("register")]
		public async Task<ActionResult> Register(RegisterVewModel model)
		{
			var use = new User()
			{
				Name = model.UserName,
				SchoolName = model.SchoolName,
			};
			var result = await _userManager.CreateAsync(use, model.Password);

			if (!result.Succeeded)
				return BadRequest(result.Errors);

			var Claims = new List<Claim>
				{
					new Claim(ClaimTypes.Name , model.UserName),
					//new Claim (ClaimTypes.NameIdentifier, use.Id),
					new Claim (ClaimTypes.Role ,"Student")
				};

			var claimResult = await _userManager.AddClaimsAsync(use, Claims);

			if (!claimResult.Succeeded)
				return BadRequest(claimResult.Errors);

			return Ok();
		}

		[HttpPost]
		[Route("login")]
		public async Task<ActionResult> Login(LoginViewModel model)
		{
			var employee = await _userManager.FindByNameAsync(model.UserName);

			if (!await _userManager.CheckPasswordAsync(employee, model.Password))
				return Unauthorized();

			var empClaims = await _userManager.GetClaimsAsync(employee);

			var stringKey = _config.GetValue<string>("SecretKey");
			var byteKey = Encoding.ASCII.GetBytes(stringKey);
			var key = new SymmetricSecurityKey(byteKey);

			var SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

			var jwt = new JwtSecurityToken(
				claims: empClaims,
				signingCredentials: SigningCredentials,
				expires: DateTime.Now.AddMinutes(_config.GetValue<int>("TokenDuration")),
				notBefore: DateTime.Now
				);

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.WriteToken(jwt);

			return Ok(new
			{
				Token = token,
				expiration = DateTime.Now.AddMinutes(_config.GetValue<int>("TokenDuration"))
			});
		}
	}
}