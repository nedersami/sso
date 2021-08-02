using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SSO.API.Configurations
{
	public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
			var isAuthorized = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
			var allowAnonymous = filterPipeline.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);

			if (isAuthorized && !allowAnonymous)
			{
				if (operation.Parameters == null)
					operation.Parameters = new List<OpenApiParameter>();

				operation.Parameters.Add(new OpenApiParameter
				{
					Name = "Authorization",
					In = ParameterLocation.Header,
					Description = "access token",
					Required = true
				});
			}
		}
	}

	public static class SwaggerConfiguration
	{
		public static void AddSwaggerConfig(this IServiceCollection services)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
			string dataPublicacao = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location).ToString("dd/MM/yyyy HH:mm");

			services.AddSwaggerGen(s =>
			{
				s.SwaggerDoc("v1", new OpenApiInfo
				{
					Version = "v1",
					Title = "SSO.API",
					Description = $"Última Publicação: {dataPublicacao}"
				});

				s.OperationFilter<AuthorizationHeaderParameterOperationFilter>();

				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				s.IncludeXmlComments(xmlPath);
			});

		}
	}
}
