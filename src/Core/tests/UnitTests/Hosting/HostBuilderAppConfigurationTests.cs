using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Xunit;

namespace Microsoft.Maui.UnitTests.Hosting
{
	[Category(TestCategory.Core, TestCategory.Hosting)]
	public class HostBuilderAppConfigurationTests
	{
		[Fact]
		public void ConfigureAppConfigurationConfiguresValues()
		{
			var services = MauiAppBuilder.CreateBuilder()
				.ConfigureAppConfiguration((_, builder) =>
				{
					builder.AddInMemoryCollection(new Dictionary<string, string>
					{
						{ "key 1", "value 1" },
					});
				})
				.Build();

			var configuration = services.GetRequiredService<IConfiguration>();

			Assert.Equal("value 1", configuration["key 1"]);
		}

		[Fact]
		public void ConfigureAppConfigurationOverwritesValues()
		{
			var services = MauiAppBuilder.CreateBuilder()
				.ConfigureAppConfiguration((_, builder) =>
				{
					builder.AddInMemoryCollection(new Dictionary<string, string>
					{
						{ "key 1", "value 1" },
						{ "key 2", "value 2" },
					});
				})
				.ConfigureAppConfiguration((_, builder) =>
				{
					builder.AddInMemoryCollection(new Dictionary<string, string>
					{
						{ "key 1", "value a" },
					});
				})
				.Build();

			var configuration = services.GetRequiredService<IConfiguration>();

			Assert.Equal("value a", configuration["key 1"]);
			Assert.Equal("value 2", configuration["key 2"]);
		}

		[Fact]
		public void ConfigureServicesCanUseConfig()
		{
			string value = null;

			var appBuilder = MauiAppBuilder.CreateBuilder()
				.ConfigureAppConfiguration((_, builder) =>
				{
					builder.AddInMemoryCollection(new Dictionary<string, string>
					{
						{ "key 1", "value 1" },
					});
				});

			// TODO: Need appBuilder.Host.ConfigureService(...);

			//appBuilder.
			//	.ConfigureServices((context, services) =>
			//	{
			//		value = context.Configuration["key 1"];
			//	});
			var services = appBuilder.Build();

			Assert.Equal("value 1", value);
			throw new System.Exception("TODO: Not implemented yet");
		}
	}
}