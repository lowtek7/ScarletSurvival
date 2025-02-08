using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Scarlet.Core.Async;
using Scarlet.Core.Async.Interfaces;
using Scarlet.Core.Collection;
using Scarlet.Core.Communication.Interfaces;
using Scarlet.Core.Event;
using Scarlet.Core.Event.Interfaces;
using Scarlet.Core.Logging;
using Scarlet.Core.Logging.Appenders;
using Scarlet.Core.Logging.Enums;
using Scarlet.Core.Logging.Interfaces;
using Scarlet.Core.Services.Interfaces;

namespace Scarlet.Core
{
	public class CoreSandbox : IDisposable
	{
		private readonly CoreId _id;
		private readonly CoreConfiguration _config;
		private readonly Dictionary<Type, IService> _services;
		private readonly RingBuffer<Action> _mainThreadActions;
		private readonly object _servicesLock = new();

		private volatile int _mainThreadId;
		private volatile bool _isInitialized;
		private volatile bool _isDisposed;

		public CoreId Id => _id;
		public bool IsInitialized => _isInitialized;
		public bool IsDisposed => _isDisposed;

		private const int DEFAULT_ACTION_BUFFER_SIZE = 1024;
		private const int MAX_ACTIONS_PER_UPDATE = 64;

		private CoreSandbox(CoreId id, CoreConfiguration config)
		{
			_id = id;
			_config = config;
			_services = new Dictionary<Type, IService>();
			_mainThreadActions = new RingBuffer<Action>(DEFAULT_ACTION_BUFFER_SIZE);
			_mainThreadId = Thread.CurrentThread.ManagedThreadId;
		}

		public static IAsyncOperation<CoreSandbox> CreateAsync(CoreConfiguration config)
		{
			var operation = new AsyncOperation<CoreSandbox>();

			try
			{
				var sandbox = new CoreSandbox(CoreId.New(), config);
				var initOperation = sandbox.InitializeAsync();

				initOperation.Completed += op =>
				{
					if (op.HasError)
					{
						operation.SetException(op.Error);
						return;
					}
					operation.SetResult(sandbox);
				};
			}
			catch (Exception ex)
			{
				operation.SetException(ex);
			}

			return operation;
		}

		private IAsyncOperation<bool> InitializeAsync()
		{
			var operation = new AsyncOperation<bool>();

			try
			{
				if (_isInitialized)
				{
					operation.SetException(new InvalidOperationException("CoreSandbox is already initialized"));
					return operation;
				}

				// 기본 서비스 초기화
				InitializeBaseServices();

				// 서비스 초기화
				var servicesOperation = InitializeServicesAsync();
				servicesOperation.Completed += op =>
				{
					if (op.HasError)
					{
						operation.SetException(op.Error);
						return;
					}

					_isInitialized = true;
					GetService<ILogger>()?.LogInfo($"CoreSandbox {_id} initialized");
					operation.SetResult(true);
				};
			}
			catch (Exception ex)
			{
				GetService<ILogger>()?.LogError($"Failed to initialize CoreSandbox: {ex.Message}");
				operation.SetException(ex);
			}

			return operation;
		}

		private void InitializeBaseServices()
		{
			// 로거 Appender 설정
			var appenders = new List<ILogAppender>();

			// 콘솔 Appender 추가
			appenders.Add(new ConsoleLogAppender(_config.LogConfig));

			// 파일 Appender 추가 (설정이 있는 경우)
			if (!string.IsNullOrEmpty(_config.LogConfig.LogFilePath))
			{
				appenders.Add(new FileLogAppender(_config.LogConfig));
			}

			// LogService 초기화
			var logger = new LogService(this, _config.LogConfig, appenders.ToArray());
			RegisterService<ILogger>(logger);

			// EventBus 초기화
			var eventBus = new EventBus(this);
			RegisterService<IEventBus>(eventBus);
		}

		private IAsyncOperation<bool> InitializeServicesAsync()
		{
			var operation = new AsyncOperation<bool>();

			try
			{
				List<IService> services;
				lock (_servicesLock)
				{
					services = _services.Values.ToList();
				}

				var currentIndex = 0;
				void InitializeNext()
				{
					if (currentIndex >= services.Count)
					{
						operation.SetResult(true);
						return;
					}

					try
					{
						var service = services[currentIndex++];
						service.Initialize();
						operation.SetProgress((float)currentIndex / services.Count);
						InitializeNext();
					}
					catch (Exception ex)
					{
						operation.SetException(ex);
					}
				}

				InitializeNext();
			}
			catch (Exception ex)
			{
				operation.SetException(ex);
			}

			return operation;
		}

		public T GetService<T>() where T : class, IService
		{
			lock (_servicesLock)
			{
				return _services.TryGetValue(typeof(T), out var service) ? service as T : null;
			}
		}

		public void RegisterService<T>(T service) where T : class, IService
		{
			if (service == null)
				throw new ArgumentNullException(nameof(service));

			if (service.OwnerId != Id)
				throw new InvalidOperationException($"Service belongs to different CoreSandbox: {service.OwnerId}");

			lock (_servicesLock)
			{
				var type = typeof(T);
				if (_services.ContainsKey(type))
					throw new InvalidOperationException($"Service of type {type.Name} is already registered");

				_services[type] = service;

				if (_isInitialized)
				{
					EnqueueMainThread(() => service.Initialize());
				}
			}
		}

		public void EnqueueMainThread(Action action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			if (Thread.CurrentThread.ManagedThreadId == _mainThreadId)
			{
				action();
				return;
			}

			if (!_mainThreadActions.TryEnqueue(action))
			{
				GetService<ILogger>()?.LogWarning("Main thread action queue is full");
			}
		}

		public void Update()
		{
			if (_isDisposed)
				return;

			if (Thread.CurrentThread.ManagedThreadId != _mainThreadId)
				throw new InvalidOperationException("Update must be called from the main thread");

			try
			{
				ProcessMainThreadActions();
				UpdateServices();
			}
			catch (Exception ex)
			{
				GetService<ILogger>()?.LogError($"Error in CoreSandbox update: {ex.Message}");
			}
		}

		private void ProcessMainThreadActions()
		{
			int processedCount = 0;
			while (processedCount < MAX_ACTIONS_PER_UPDATE &&
				   _mainThreadActions.TryDequeue(out var action))
			{
				try
				{
					action();
				}
				catch (Exception ex)
				{
					GetService<ILogger>()?.LogError($"Error executing main thread action: {ex.Message}");
				}
				processedCount++;
			}
		}

		private void UpdateServices()
		{
			List<IService> services;
			lock (_servicesLock)
			{
				services = _services.Values.ToList();
			}

			foreach (var service in services)
			{
				if (service is IUpdatableService updatable)
				{
					try
					{
						updatable.Update();
					}
					catch (Exception ex)
					{
						GetService<ILogger>()?.LogError(
							$"Error updating service {service.GetType().Name}: {ex.Message}");
					}
				}
			}
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;

			List<IService> services;
			lock (_servicesLock)
			{
				services = _services.Values.ToList();
				_services.Clear();
			}

			foreach (var service in services.Reverse<IService>())
			{
				try
				{
					service.Cleanup();
				}
				catch (Exception ex)
				{
					GetService<ILogger>()?.LogError(
						$"Error cleaning up service {service.GetType().Name}: {ex.Message}");
				}
			}

			_isDisposed = true;
			GetService<ILogger>()?.LogInfo($"CoreSandbox {_id} disposed");
		}
	}
}
