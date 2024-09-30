using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ApiTak2.Data.Entities;
using System;

namespace ApiTak2.Data.Context
{
	public class UserContext : IdentityDbContext
	{
		public UserContext() : base() { }

		public UserContext(DbContextOptions<UserContext> options) : base(options)
		{
		}

		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.Entity<IdentityRole>().ToTable("Role");
		}
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Data source=mariam;Initial catalog= ApiTask2;Integrated security = true; TrustServerCertificate=True");
			base.OnConfiguring(optionsBuilder);
		}
	}
}
