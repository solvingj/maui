using System;
using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Maui.Handlers.Benchmarks
{
	[MemoryDiagnoser]
	public class MauiServiceProviderBenchmarker
	{
		IServiceProvider _serviceProvider;

		[Params(100_000)]
		public int N { get; set; }

		[IterationSetup(Target = nameof(DefaultBuilder))]
		public void SetupForDefaultBuilder()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();

			appBuilder.Services.AddTransient<IFooService, FooService>();

			_serviceProvider = appBuilder.Build();
		}

		[IterationSetup(Target = nameof(DefaultBuilderWithConstructorInjection))]
		public void SetupForDefaultBuilderWithConstructorInjection()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
				//.UseMauiServiceProviderFactory(true)
			appBuilder.Services.AddTransient<IFooService, FooService>();
			_serviceProvider = appBuilder.Build();
		}

		[IterationSetup(Target = nameof(OneConstructorParameter))]
		public void SetupForOneConstructorParameter()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			//.UseMauiServiceProviderFactory(true)
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IFooBarService, FooBarWithFooService>();
			_serviceProvider = appBuilder.Build();
		}

		[IterationSetup(Target = nameof(TwoConstructorParameters))]
		public void SetupForTwoConstructorParameters()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			//.UseMauiServiceProviderFactory(true)
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IBarService, BarService>();
			appBuilder.Services.AddTransient<IFooBarService, FooBarWithFooAndBarService>();
			_serviceProvider = appBuilder.Build();
		}

		[IterationSetup(Target = nameof(ExtensionsWithConstructorInjection))]
		public void SetupForExtensionsWithConstructorInjection()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			_serviceProvider = appBuilder.Build();
		}

		[IterationSetup(Target = nameof(ExtensionsWithOneConstructorParameter))]
		public void SetupForExtensionsWithOneConstructorParameter()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IFooBarService, FooBarWithFooService>();
			_serviceProvider = appBuilder.Build();
		}

		[IterationSetup(Target = nameof(ExtensionsWithTwoConstructorParameters))]
		public void SetupForExtensionsWithTwoConstructorParameters()
		{
			var appBuilder = MauiAppBuilder.CreateBuilder();
			appBuilder.Services.AddTransient<IFooService, FooService>();
			appBuilder.Services.AddTransient<IBarService, BarService>();
			appBuilder.Services.AddTransient<IFooBarService, FooBarWithFooAndBarService>();
			_serviceProvider = appBuilder.Build();
		}

		[Benchmark(Baseline = true)]
		public void DefaultBuilder()
		{
			for (int i = 0; i < N; i++)
			{
				_serviceProvider.GetService<IFooService>();
			}
		}

		[Benchmark]
		public void DefaultBuilderWithConstructorInjection()
		{
			for (int i = 0; i < N; i++)
			{
				_serviceProvider.GetService<IFooService>();
			}
		}

		[Benchmark]
		public void OneConstructorParameter()
		{
			for (int i = 0; i < N; i++)
			{
				_serviceProvider.GetService<IFooBarService>();
			}
		}

		[Benchmark]
		public void TwoConstructorParameters()
		{
			for (int i = 0; i < N; i++)
			{
				_serviceProvider.GetService<IFooBarService>();
			}
		}

		[Benchmark]
		public void ExtensionsWithConstructorInjection()
		{
			for (int i = 0; i < N; i++)
			{
				_serviceProvider.GetService<IFooService>();
			}
		}

		[Benchmark]
		public void ExtensionsWithOneConstructorParameter()
		{
			for (int i = 0; i < N; i++)
			{
				_serviceProvider.GetService<IFooBarService>();
			}
		}

		[Benchmark]
		public void ExtensionsWithTwoConstructorParameters()
		{
			for (int i = 0; i < N; i++)
			{
				_serviceProvider.GetService<IFooBarService>();
			}
		}

		public class DIExtensionsServiceProviderFactory : IServiceProviderFactory<ServiceCollection>
		{
			public ServiceCollection CreateBuilder(IServiceCollection services)
				=> new ServiceCollection { services };

			public IServiceProvider CreateServiceProvider(ServiceCollection containerBuilder)
				=> containerBuilder.BuildServiceProvider();
		}
	}
}