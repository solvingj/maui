using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Hosting;
using Xunit;

namespace Microsoft.Maui.UnitTests.Hosting
{
	[Category(TestCategory.Core, TestCategory.Hosting)]
	public class HostBuilderServicesTests
	{
		[Fact]
		public void CanGetServices()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			var services = appBuilder.Build();

			Assert.NotNull(services);
		}

		[Fact]
		public void GetServiceThrowsWhenConstructorParamTypesWereNotRegistered()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooBarService, FooBarService>();
			var services = appBuilder.Build();

			Assert.Throws<InvalidOperationException>(() => services.GetService<IFooBarService>());
		}

		[Fact]
		public void GetServiceThrowsWhenNoPublicConstructors()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, BadFooService>();
			var services = appBuilder.Build();

			var ex = Assert.Throws<InvalidOperationException>(() => services.GetService<IFooService>());
			Assert.Contains("suitable constructor", ex.Message);
		}

		[Fact]
		public void GetServiceHandlesFirstOfMultipleConstructors()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IFooBarService, FooDualConstructor>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();

			var foobar = Assert.IsType<FooDualConstructor>(service);
			Assert.IsType<FooService>(foobar.Foo);
		}

		[Fact]
		public void GetServiceHandlesSecondOfMultipleConstructors()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IBarService, BarService>();
			appBuilder.Services.AddTransient<IFooBarService, FooDualConstructor>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();

			var foobar = Assert.IsType<FooDualConstructor>(service);
			Assert.IsType<BarService>(foobar.Bar);
		}

		[Fact]
		public void GetServiceHandlesUsesCorrectCtor_DefaultWithNothing()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooBarService, FooTrioConstructor>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();

			var trio = Assert.IsType<FooTrioConstructor>(service);
			Assert.Null(trio.Foo);
			Assert.Null(trio.Bar);
			Assert.Null(trio.Cat);
			Assert.Equal("()", trio.Option);
		}

		[Fact]
		public void GetServiceHandlesUsesCorrectCtor_DefaultWithBar()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IBarService, BarService>();
			appBuilder.Services.AddTransient<IFooBarService, FooTrioConstructor>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();

			var trio = Assert.IsType<FooTrioConstructor>(service);
			Assert.Null(trio.Foo);
			Assert.Null(trio.Bar);
			Assert.Null(trio.Cat);
			Assert.Equal("()", trio.Option);
		}

		[Fact]
		public void GetServiceHandlesUsesCorrectCtor_Foo()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IFooBarService, FooTrioConstructor>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();

			var trio = Assert.IsType<FooTrioConstructor>(service);
			Assert.IsType<FooService>(trio.Foo);
			Assert.Null(trio.Bar);
			Assert.Null(trio.Cat);
			Assert.Equal("(Foo)", trio.Option);
		}

		[Fact]
		public void GetServiceHandlesUsesCorrectCtor_FooWithCat()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<ICatService, CatService>();
			appBuilder.Services.AddTransient<IFooBarService, FooTrioConstructor>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();

			var trio = Assert.IsType<FooTrioConstructor>(service);
			Assert.IsType<FooService>(trio.Foo);
			Assert.Null(trio.Bar);
			Assert.Null(trio.Cat);
			Assert.Equal("(Foo)", trio.Option);
		}

		[Fact]
		public void GetServiceHandlesUsesCorrectCtor_FooBar()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IBarService, BarService>();
			appBuilder.Services.AddTransient<IFooBarService, FooTrioConstructor>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();

			var trio = Assert.IsType<FooTrioConstructor>(service);
			Assert.IsType<FooService>(trio.Foo);
			Assert.IsType<BarService>(trio.Bar);
			Assert.Null(trio.Cat);
			Assert.Equal("(Foo, Bar)", trio.Option);
		}

		[Fact]
		public void GetServiceHandlesUsesCorrectCtor_FooBarCat()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IBarService, BarService>();
			appBuilder.Services.AddTransient<ICatService, CatService>();
			appBuilder.Services.AddTransient<IFooBarService, FooTrioConstructor>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();

			var trio = Assert.IsType<FooTrioConstructor>(service);
			Assert.IsType<FooService>(trio.Foo);
			Assert.IsType<BarService>(trio.Bar);
			Assert.IsType<CatService>(trio.Cat);
			Assert.Equal("(Foo, Bar, Cat)", trio.Option);
		}

		[Fact]
		public void GetServiceCanReturnTypesThatHaveConstructorParams()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IBarService, BarService>();
			appBuilder.Services.AddTransient<IFooBarService, FooBarService>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();

			var foobar = Assert.IsType<FooBarService>(service);
			Assert.IsType<FooService>(foobar.Foo);
			Assert.IsType<BarService>(foobar.Bar);
		}

		[Fact]
		public void GetServiceCanReturnTypesThatHaveUnregisteredConstructorParamsButHaveDefaultValues()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooBarService, FooDefaultValueConstructor>();
			var services = appBuilder.Build();

			var foo = services.GetService<IFooBarService>();

			Assert.NotNull(foo);

			var actual = Assert.IsType<FooDefaultValueConstructor>(foo);

			Assert.Null(actual.Bar);
		}

		[Fact]
		public void GetServiceCanReturnTypesThatHaveRegisteredConstructorParamsAndHaveDefaultValues()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IBarService, BarService>();
			appBuilder.Services.AddTransient<IFooBarService, FooDefaultValueConstructor>();
			var services = appBuilder.Build();

			var foo = services.GetService<IFooBarService>();

			Assert.NotNull(foo);

			var actual = Assert.IsType<FooDefaultValueConstructor>(foo);

			Assert.NotNull(actual.Bar);
			Assert.IsType<BarService>(actual.Bar);
		}

		[Fact]
		public void GetServiceCanReturnTypesThatHaveSystemDefaultValues()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooBarService, FooDefaultSystemValueConstructor>();
			var services = appBuilder.Build();

			var foo = services.GetService<IFooBarService>();

			Assert.NotNull(foo);

			var actual = Assert.IsType<FooDefaultSystemValueConstructor>(foo);

			Assert.Equal("Default Value", actual.Text);
		}

		[Fact]
		public void GetServiceCanReturnEnumerableParams()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IFooService, FooService2>();
			appBuilder.Services.AddTransient<IFooBarService, FooEnumerableService>();
			var services = appBuilder.Build();

			var service = services.GetService<IFooBarService>();
			var foobar = Assert.IsType<FooEnumerableService>(service);

			var serviceTypes = foobar.Foos
				.Select(s => s.GetType().FullName)
				.ToArray();
			Assert.Contains(typeof(FooService).FullName, serviceTypes);
			Assert.Contains(typeof(FooService2).FullName, serviceTypes);
		}

		[Fact]
		public void WillRetrieveDifferentTransientServices()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			var services = appBuilder.Build();

			AssertTransient<IFooService, FooService>(services);
		}

		[Fact]
		public void WillRetrieveSameSingletonServices()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddSingleton<IFooService, FooService>();
			var services = appBuilder.Build();

			AssertSingleton<IFooService, FooService>(services);
		}

		[Fact]
		public void WillRetrieveMixedServices()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddSingleton<IFooService, FooService>();
			appBuilder.Services.AddTransient<IBarService, BarService>();
			var services = appBuilder.Build();

			AssertSingleton<IFooService, FooService>(services);
			AssertTransient<IBarService, BarService>(services);
		}

		[Fact]
		public void WillRetrieveEnumerables()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IFooService, FooService2>();
			var services = appBuilder.Build();

			var fooServices = services
				.GetServices<IFooService>()
				.ToArray();
			Assert.Equal(2, fooServices.Length);

			var serviceTypes = fooServices
				.Select(s => s.GetType().FullName)
				.ToArray();
			Assert.Contains(typeof(FooService).FullName, serviceTypes);
			Assert.Contains(typeof(FooService2).FullName, serviceTypes);
		}

		[Fact]
		public void CanCreateLogger()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddLogging(logging => logging.AddConsole());
			var services = appBuilder.Build();

			var factory = services.GetRequiredService<ILoggerFactory>();

			var logger = factory.CreateLogger<HostBuilderServicesTests>();

			Assert.NotNull(logger);
		}

		static void AssertTransient<TInterface, TConcrete>(IServiceProvider services)
		{
			var service1 = services.GetService<TInterface>();

			Assert.NotNull(service1);
			Assert.IsType<TConcrete>(service1);

			var service2 = services.GetService<TInterface>();

			Assert.NotNull(service2);
			Assert.IsType<TConcrete>(service2);

			Assert.NotEqual(service1, service2);
		}

		static void AssertSingleton<TInterface, TConcrete>(IServiceProvider services)
		{
			var service1 = services.GetService<TInterface>();

			Assert.NotNull(service1);
			Assert.IsType<TConcrete>(service1);

			var service2 = services.GetService<TInterface>();

			Assert.NotNull(service2);
			Assert.IsType<TConcrete>(service2);

			Assert.Equal(service1, service2);
		}
	}
}