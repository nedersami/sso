using AutoMapper;
using SSO.Infrastructure.Data.Domain;
using SSO.Infrastructure.Business.Model;
using Microsoft.Extensions.DependencyInjection;

namespace SSO.UI.Configurations
{
	public static class AutoMapperConfiguration
	{
		public static void AddAutoMapper(this IServiceCollection services)
		{
			var config = new AutoMapper.MapperConfiguration(cfg =>
			{
				cfg.CreateMap<ApplicationUser, IdentityUserModel>();
				cfg.CreateMap<IdentityUserModel, ApplicationUser>();
			});
			IMapper mapper = config.CreateMapper();
			services.AddSingleton(mapper);
		}
	}
}
