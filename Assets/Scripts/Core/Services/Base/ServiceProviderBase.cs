using System;
using System.Collections.Generic;
using System.Threading;
using Scarlet.Core.Logging;
using Scarlet.Core.Logging.Interfaces;
using Scarlet.Core.Services.Enums;
using Scarlet.Core.Services.Interfaces;
using Scarlet.Core.Services.Results;

namespace Scarlet.Core.Services.Base
{
	/// <summary>
	/// 서비스 제공자의 기본 구현을 위한 추상 클래스
	/// </summary>
	/// <typeparam name="TService">서비스 인터페이스 타입</typeparam>
	/// <typeparam name="TConfig">서비스 설정 타입</typeparam>
	public abstract class ServiceProviderBase<TService, TConfig>
		where TService : class, IService
		where TConfig : class
	{
		private struct ServiceState
		{
			public TService Instance;
			public bool IsInitialized;
			public TConfig Config;
		}

		private class StateContainer
		{
			public ServiceState State;
		}

		private static volatile StateContainer _currentState = new();
		private static readonly object _initLock = new();
		private static readonly List<Action<TService>> _initializationCallbacks = new(4);
		private static readonly List<Action> _resetCallbacks = new(4);
		private static readonly AsyncLocal<bool> _isLockHeld = new();

		// 로깅을 위한 정적 속성
		protected static ILogger Logger { get; }

		public readonly struct ServiceResult
		{
			public readonly TService Value { get; }
			public readonly bool Success { get; }
			public readonly string Error { get; }

			private ServiceResult(TService value, bool success, string error = null)
			{
				Value = value;
				Success = success;
				Error = error;
			}

			public static ServiceResult Ok(TService service) => new(service, true);
			public static ServiceResult Fail(string error) => new(null, false, error);
		}

		public static ServiceResult Get()
		{
			var state = _currentState.State;
			if (!state.IsInitialized)
			{
				return ServiceResult.Fail("Service is not initialized");
			}

			return ServiceResult.Ok(state.Instance);
		}

		public static ServiceResult Initialize(TService service, TConfig config = null)
		{
			if (service == null)
				return ServiceResult.Fail("Service instance cannot be null");

			if (_currentState.State.IsInitialized)
				return ServiceResult.Ok(_currentState.State.Instance);

			bool lockTaken = false;
			try
			{
				if (_isLockHeld.Value)
					return ServiceResult.Fail("Recursive initialization detected");

				Monitor.Enter(_initLock, ref lockTaken);
				_isLockHeld.Value = true;

				if (_currentState.State.IsInitialized)
					return ServiceResult.Ok(_currentState.State.Instance);

				var newState = new ServiceState
				{
					Instance = service,
					Config = config,
					IsInitialized = true
				};

				try
				{
					service.Initialize();
				}
				catch (Exception ex)
				{
					Logger.LogError($"Service initialization failed: {ex.Message}");
					return ServiceResult.Fail($"Service initialization failed: {ex.Message}");
				}

				var oldContainer = _currentState;
				var newContainer = new StateContainer { State = newState };
				if (Interlocked.CompareExchange(ref _currentState, newContainer, oldContainer) == oldContainer)
				{
					foreach (var callback in _initializationCallbacks)
					{
						try
						{
							callback(service);
						}
						catch (Exception ex)
						{
							Logger.LogError($"Initialization callback failed: {ex.Message}");
						}
					}

					return ServiceResult.Ok(service);
				}

				return ServiceResult.Fail("Concurrent initialization detected");
			}
			finally
			{
				if (lockTaken)
				{
					_isLockHeld.Value = false;
					Monitor.Exit(_initLock);
				}
			}
		}

		public static void Reset()
		{
			bool lockTaken = false;
			try
			{
				if (_isLockHeld.Value)
					return;

				Monitor.Enter(_initLock, ref lockTaken);
				_isLockHeld.Value = true;

				var oldState = _currentState.State;
				if (!oldState.IsInitialized)
					return;

				foreach (var callback in _resetCallbacks)
				{
					try
					{
						callback();
					}
					catch (Exception ex)
					{
						Logger.LogError($"Reset callback failed: {ex.Message}");
					}
				}

				try
				{
					oldState.Instance?.Cleanup();
				}
				catch (Exception ex)
				{
					Logger.LogError($"Service cleanup failed: {ex.Message}");
				}

				Interlocked.Exchange(ref _currentState, new StateContainer
				{
					State = new ServiceState()
				});
			}
			finally
			{
				if (lockTaken)
				{
					_isLockHeld.Value = false;
					Monitor.Exit(_initLock);
				}
			}
		}

		public static void RegisterInitializationCallback(Action<TService> callback)
		{
			if (callback == null)
				return;

			lock (_initializationCallbacks)
			{
				_initializationCallbacks.Add(callback);

				var state = _currentState.State;
				if (state.IsInitialized)
				{
					try
					{
						callback(state.Instance);
					}
					catch (Exception ex)
					{
						Logger.LogError($"Initialization callback failed: {ex.Message}");
					}
				}
			}
		}

		public static void RegisterResetCallback(Action callback)
		{
			if (callback == null)
				return;

			lock (_resetCallbacks)
			{
				_resetCallbacks.Add(callback);
			}
		}

		public static bool IsInitialized => _currentState.State.IsInitialized;

		protected static TConfig GetConfig() => _currentState.State.Config;
	}
}
