using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System;

namespace SSO.UI.Configurations
{
    public static class SSOMiddlewareExtension
    {
        public static IServiceCollection AddSSOUI(this IServiceCollection services, IConfiguration configuration, Action<IdentityOptions> options)
        {
			services.AddAutoMapper();
			services.AddDIConfiguration(configuration);
			services.AddMvcSecurity(configuration, options ?? (opts => { }));
            return services;
        }

        public static IApplicationBuilder UseSSOUI(this IApplicationBuilder app)
        {
            app.UseAuthentication();
			app.UseCookiePolicy();
			app.UseAuthorization();
			return app;
        }

        public static IMvcBuilder AddSSORoutes(this IMvcBuilder services)
        {
            var ssoAssembly = typeof(Controllers.AccountController).Assembly;
            services.AddApplicationPart(ssoAssembly);
            return services;
        }
    }
}