using SSO.API.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace SSO.API
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;

			Log.Logger = new LoggerConfiguration()
				.MinimumLevel
				.Information()
				.WriteTo.RollingFile("Logs/log-{Date}.txt", LogEventLevel.Information)
				.CreateLogger();
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddAutoMapper();
			services.ConfigureCors();
			services.AddSwaggerConfig();
			services.AddDIConfiguration(Configuration);

			services.AddLogging(loggingBuilder =>
			{
				loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
				loggingBuilder.AddConsole();
				loggingBuilder.AddDebug();
			});

			services.AddMvcSecurity(Configuration);
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				app.UseHsts();
			}

			app.UseCors(c =>
			{
				c.AllowAnyHeader();
				c.AllowAnyMethod();
				c.AllowAnyOrigin();
			});

			app.UseExceptionHandler(config =>
			{
				config.Run(async context =>
				{
					context.Response.StatusCode = 500;
					context.Response.ContentType = "application/json";

					var error = context.Features.Get<IExceptionHandlerFeature>();
					if (error != null)
					{
						var ex = error.Error;

						await context.Response.WriteAsync(ex.ToString());
					}
				});
			});

			app.UseRouting();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
			app.UseAuthentication();
			app.UseHttpsRedirection();

			app.UseSwagger();
			app.UseSwaggerUI(s =>
			{
				s.RoutePrefix = string.Empty;
				s.SwaggerEndpoint("./swagger/v1/swagger.json", "SSO.API API v1");
			});
		}
	}
}
