using System;
using Scarlet.Core.DI.Enums;
using Scarlet.Core.DI.Interfaces;

namespace Scarlet.Core.DI
{
	// Core/DI/ServiceDescriptor.cs
	public class ServiceDescriptor : IServiceDescriptor
	{
		public Type ServiceType { get; }
		public Type ImplementationType { get; }
		public ServiceLifetime Lifetime { get; }
		public object ImplementationInstance { get; }
		public Func<IServiceProvider, object> ImplementationFactory { get; }

		private ServiceDescriptor(
			Type serviceType,
			Type implementationType,
			ServiceLifetime lifetime)
		{
			ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			ImplementationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
			Lifetime = lifetime;
		}

		private ServiceDescriptor(
			Type serviceType,
			object instance)
		{
			ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			ImplementationType = instance?.GetType() ?? throw new ArgumentNullException(nameof(instance));
			ImplementationInstance = instance;
			Lifetime = ServiceLifetime.Singleton;
		}

		private ServiceDescriptor(
			Type serviceType,
			Func<IServiceProvider, object> factory,
			ServiceLifetime lifetime)
		{
			ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
			ImplementationFactory = factory ?? throw new ArgumentNullException(nameof(factory));
			Lifetime = lifetime;
		}

		#region Factory Methods

		public static ServiceDescriptor Singleton<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService
		{
			return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);
		}

		public static ServiceDescriptor Singleton<TService>(TService instance)
			where TService : class
		{
			return new ServiceDescriptor(typeof(TService), instance);
		}

		public static ServiceDescriptor Singleton<TService>(
			Func<IServiceProvider, TService> factory)
			where TService : class
		{
			return new ServiceDescriptor(typeof(TService), sp => factory(sp), ServiceLifetime.Singleton);
		}

		public static ServiceDescriptor Scoped<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService
		{
			return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);
		}

		public static ServiceDescriptor Scoped<TService>(
			Func<IServiceProvider, TService> factory)
			where TService : class
		{
			return new ServiceDescriptor(typeof(TService), sp => factory(sp), ServiceLifetime.Scoped);
		}

		public static ServiceDescriptor Transient<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService
		{
			return new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);
		}

		public static ServiceDescriptor Transient<TService>(
			Func<IServiceProvider, TService> factory)
			where TService : class
		{
			return new ServiceDescriptor(typeof(TService), factory, ServiceLifetime.Transient);
		}

		#endregion
	}
}
