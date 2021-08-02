using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SSO.Infrastructure.Business.Model;
using SSO.Infrastructure.Business.Interface;

namespace SSO.UI.Configurations
{
	public static class DependencyInjectionConfiguration
	{
		public static void AddDIConfiguration(this IServiceCollection services, IConfiguration configuration)
		{
			var settingConfiguration = new SettingConfigurationModel();
			new ConfigureFromConfigurationOptions<SettingConfigurationModel>(
				configuration.GetSection("SettingConfiguration"))
					.Configure(settingConfiguration);
			services.AddSingleton(settingConfiguration);

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
		}
	}
}
