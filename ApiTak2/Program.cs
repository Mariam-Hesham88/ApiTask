
using ApiTak2.Data.Context;
using ApiTak2.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace ApiTak2
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			//context
			builder.Services.AddDbContext<UserContext>(options =>
			{
				options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
			});

			builder.Services.AddIdentity<User, IdentityRole>(options =>
			{
				options.Password.RequiredLength = 6;
				options.Password.RequireUppercase = false;
			}).AddEntityFrameworkStores<UserContext>();

			//Auth
			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = "Default";
				options.DefaultChallengeScheme = "Default";
			}).AddJwtBearer("Default", options =>
			{
				var keyString = builder.Configuration.GetValue<string>("SecretKey");
				var keyInBytes = Encoding.ASCII.GetBytes(keyString);
				var key = new SymmetricSecurityKey(keyInBytes);
				options.TokenValidationParameters = new TokenValidationParameters
				{
					IssuerSigningKey = key,
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

			//authorization
			builder.Services.AddAuthorization(options =>
			options.AddPolicy("Mariam",p => p.RequireClaim(ClaimTypes.Role, "CEO")));


			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthentication();

			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}
