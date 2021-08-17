using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using Xunit;

namespace Microsoft.Maui.UnitTests.ImageSource
{
	[Category(TestCategory.Core, TestCategory.ImageSource)]
	public class ImageSourceServiceTests
	{
		[Fact]
		public void ResolvesToConcreteTypeOverInterface()
		{
			var provider = CreateImageSourceServiceProvider(services =>
			{
				services.AddService<IStreamImageSource, StreamImageSourceService>();
				services.AddService<MultipleInterfacesImageSourceStub, UriImageSourceService>();
			});

			var service = provider.GetRequiredImageSourceService(new MultipleInterfacesImageSourceStub());
			Assert.IsType<UriImageSourceService>(service);
		}

		private IImageSourceServiceProvider CreateImageSourceServiceProvider(Action<IImageSourceServiceCollection> configure)
		{
			var services = MauiAppBuilder.CreateBuilder()
				.ConfigureImageSources(configure)
				.Build();

			var provider = services.GetRequiredService<IImageSourceServiceProvider>();

			return provider;
		}
	}
}
