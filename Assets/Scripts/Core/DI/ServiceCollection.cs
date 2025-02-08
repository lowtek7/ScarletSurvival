using System;
using System.Collections.Generic;
using System.Linq;
using Scarlet.Core.DI.Interfaces;

namespace Scarlet.Core.DI
{
	public class ServiceCollection : List<ServiceDescriptor>, IServiceCollection
	{
		private readonly Dictionary<Type, ServiceDescriptor> descriptorLookup
			= new Dictionary<Type, ServiceDescriptor>();

		public new void Add(ServiceDescriptor descriptor)
		{
			if (descriptor == null)
				throw new ArgumentNullException(nameof(descriptor));

			// 이미 등록된 서비스 검사
			if (descriptorLookup.ContainsKey(descriptor.ServiceType))
			{
				throw new InvalidOperationException(
					$"Service type {descriptor.ServiceType.Name} is already registered.");
			}

			base.Add(descriptor);
			descriptorLookup[descriptor.ServiceType] = descriptor;
		}

		public new void Clear()
		{
			base.Clear();
			descriptorLookup.Clear();
		}

		public bool IsRegistered<T>()
		{
			return descriptorLookup.ContainsKey(typeof(T));
		}

		public IServiceDescriptor GetDescriptor<T>()
		{
			return descriptorLookup.TryGetValue(typeof(T), out var descriptor)
				? descriptor
				: null;
		}

		#region Advanced Registration Methods

		public IServiceCollection AddSingleton<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService
		{
			Add(ServiceDescriptor.Singleton<TService, TImplementation>());
			return this;
		}

		public IServiceCollection AddSingleton<TService>(TService instance)
			where TService : class
		{
			Add(ServiceDescriptor.Singleton(instance));
			return this;
		}

		public IServiceCollection AddSingleton<TService>(
			Func<IServiceProvider, TService> factory)
			where TService : class
		{
			Add(ServiceDescriptor.Singleton(factory));
			return this;
		}

		public IServiceCollection AddScoped<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService
		{
			Add(ServiceDescriptor.Scoped<TService, TImplementation>());
			return this;
		}

		public IServiceCollection AddScoped<TService>(
			Func<IServiceProvider, TService> factory)
			where TService : class
		{
			Add(ServiceDescriptor.Scoped(factory));
			return this;
		}

		public IServiceCollection AddTransient<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService
		{
			Add(ServiceDescriptor.Transient<TService, TImplementation>());
			return this;
		}

		public IServiceCollection AddTransient<TService>(
			Func<IServiceProvider, TService> factory)
			where TService : class
		{
			Add(ServiceDescriptor.Transient(factory));
			return this;
		}

		public IServiceCollection Replace<TService, TNewImplementation>()
			where TService : class
			where TNewImplementation : class, TService
		{
			var descriptor = GetDescriptor<TService>();
			if (descriptor != null)
			{
				Remove((ServiceDescriptor)descriptor);
				descriptorLookup.Remove(typeof(TService));
			}

			return AddSingleton<TService, TNewImplementation>();
		}

		public IServiceCollection TryAddSingleton<TService, TImplementation>()
			where TService : class
			where TImplementation : class, TService
		{
			if (!IsRegistered<TService>())
			{
				AddSingleton<TService, TImplementation>();
			}

			return this;
		}

		#endregion

		#region Validation

		public void ValidateServices()
		{
			var registeredTypes = new HashSet<Type>();
			foreach (var descriptor in this)
			{
				// 중복 등록 검사
				if (!registeredTypes.Add(descriptor.ServiceType))
				{
					throw new InvalidOperationException(
						$"Duplicate registration for type {descriptor.ServiceType.Name}");
				}

				// 구현 타입 검사
				if (descriptor.ImplementationType != null)
				{
					if (!descriptor.ServiceType.IsAssignableFrom(descriptor.ImplementationType))
					{
						throw new InvalidOperationException(
							$"Type {descriptor.ImplementationType.Name} " +
							$"is not assignable to {descriptor.ServiceType.Name}");
					}
				}

				// 팩토리 메서드 검사
				if (descriptor.ImplementationFactory != null &&
					descriptor.ImplementationInstance != null)
				{
					throw new InvalidOperationException(
						$"Service {descriptor.ServiceType.Name} has both " +
						"implementation factory and instance");
				}
			}
		}

		#endregion
	}
}
