using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Hosting;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Core.UnitTests
{
	[TestFixture]
	public class HostBuilderAppTests
	{
		[Test]
		public void UseMauiAppRegistersApp()
		{
			var serviceProvider = MauiAppBuilder.CreateBuilder()
				.UseMauiApp<ApplicationStub>()
				.Build();

			var app = (ApplicationStub)serviceProvider.GetRequiredService<IApplication>();
			Assert.AreEqual("Default", app.Property);
		}

		[Test]
		public void UseMauiAppRegistersAppWithFactory()
		{
			var serviceProvider = MauiAppBuilder.CreateBuilder()
				.UseMauiApp(services => new ApplicationStub { Property = "Factory" })
				.Build();

			var app = (ApplicationStub)serviceProvider.GetRequiredService<IApplication>();
			Assert.AreEqual("Factory", app.Property);
		}

		[Test]
		public void UseMauiAppRegistersSingleton()
		{
			var serviceProvider = MauiAppBuilder.CreateBuilder()
				.UseMauiApp<ApplicationStub>()
				.Build();

			var app1 = serviceProvider.GetRequiredService<IApplication>();
			var app2 = serviceProvider.GetRequiredService<IApplication>();

			Assert.AreEqual(app1, app2);
		}
	}
}
