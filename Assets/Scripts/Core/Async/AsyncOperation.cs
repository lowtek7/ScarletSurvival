using System;
using System.Threading;
using Scarlet.Core.Async.Enums;
using Scarlet.Core.Async.Interfaces;
using Scarlet.Core.Math.Scarlet.Core.Math;

namespace Scarlet.Core.Async
{
	public class AsyncOperation<T> : AsyncOperationBase<T>
	{
		private readonly CancellationTokenSource _cts;
		private int _isStarted;
		private int _isCanceled;
		private float _progress;
		private readonly SpinWait _spinWait;

		public AsyncOperation() : base(AsyncOperationStatus.Running)
		{
			_cts = new CancellationTokenSource();
			_spinWait = new SpinWait();
		}

		public AsyncOperation(CancellationToken externalToken) : base(AsyncOperationStatus.Running)
		{
			_cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
			_spinWait = new SpinWait();
		}

		public bool IsCanceled => Interlocked.CompareExchange(ref _isCanceled, 0, 0) == 1;
		public CancellationToken CancellationToken => _cts.Token;
		private static class ProgressComparer
		{
			public static readonly float Tolerance = 1E-5f;

			public static bool AreEqual(float a, float b)
			{
				return CoreMathf.Abs(a - b) < Tolerance;
			}
		}

		public void SetResult(T result)
		{
			// 빠른 경로: 이미 완료된 상태면 반환
			if (Status >= AsyncOperationStatus.Completed)
				return;

			if (IsCanceled)
				return;

			Result = result;
			Interlocked.Exchange(ref _progress, 1f);
			Status = AsyncOperationStatus.Completed;  // 기본 클래스의 Status 속성 사용
			OnCompleted();
		}

		public void SetException(Exception error)
		{
			// 빠른 경로: 이미 완료된 상태면 반환
			if (Status >= AsyncOperationStatus.Completed)
				return;

			Error = error;
			Status = AsyncOperationStatus.Failed;  // 기본 클래스의 Status 속성 사용
			OnCompleted();
		}

		public void SetProgress(float progress)
		{
			// 빠른 경로: 이미 완료된 상태면 반환
			if (Status >= AsyncOperationStatus.Completed)
				return;

			if (IsCanceled)
				return;

			// 범위 검증
			progress = CoreMathf.Max(0f, CoreMathf.Min(1f, progress));

			// 현재 진행률과 비교하여 유의미한 변화가 있을 때만 업데이트
			float currentProgress;
			while (!ProgressComparer.AreEqual(progress,
				(currentProgress = Interlocked.CompareExchange(ref _progress, 0, 0))))
			{
				if (Interlocked.CompareExchange(ref _progress, progress, currentProgress) == currentProgress)
					break;

				_spinWait.SpinOnce();
			}
		}

		public void Cancel()
		{
			// 빠른 경로: 이미 취소되었거나 완료된 상태면 반환
			if (IsCanceled || Status >= AsyncOperationStatus.Completed)
				return;

			// CAS를 사용한 취소 상태 설정
			if (Interlocked.CompareExchange(ref _isCanceled, 1, 0) == 0)
			{
				try
				{
					_cts.Cancel();
					Status = AsyncOperationStatus.Canceled;
					OnCompleted();
				}
				catch (Exception ex)
				{
					SetException(ex);
				}
			}
		}

		public bool Start(Func<AsyncOperation<T>, CancellationToken, T> action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			if (Interlocked.CompareExchange(ref _isStarted, 1, 0) != 0)
				return false;

			ThreadPool.UnsafeQueueUserWorkItem(state =>
			{
				try
				{
					if (_cts.Token.IsCancellationRequested)
					{
						Cancel();
						return;
					}

					var result = action(this, _cts.Token);
					SetResult(result);
				}
				catch (OperationCanceledException)
				{
					Cancel();
				}
				catch (Exception ex)
				{
					SetException(ex);
				}
			}, null);

			return true;
		}

		public bool StartAsync(Func<AsyncOperation<T>, CancellationToken, T> action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			if (Interlocked.CompareExchange(ref _isStarted, 1, 0) != 0)
				return false;

			try
			{
				action(this, _cts.Token);
				return true;
			}
			catch (Exception ex)
			{
				SetException(ex);
				return false;
			}
		}

		protected override void OnCompleted()
		{
			var handler = GetAndClearCompletedEvent();
			if (handler != null)
			{
				ThreadPool.UnsafeQueueUserWorkItem(_ =>
				{
					try
					{
						handler.Invoke(this);
					}
					catch
					{
						// Completed 이벤트 핸들러의 예외는 무시
					}
				}, null);
			}
		}

		public override void Dispose()
		{
			if (Interlocked.CompareExchange(ref _isStarted, 1, 0) == 0)
			{
				Cancel();
			}

			_cts.Dispose();
			base.Dispose();  // 기본 클래스의 Dispose 호출
		}
	}

	// 고성능 확장 메서드
	public static class AsyncOperationExtensions
	{
		private sealed class ProgressTracker : IDisposable
		{
			private readonly IAsyncOperation<object> _operation;
			private readonly Action<float> _callback;
			private readonly Timer _timer;
			private float _lastProgress;
			private int _isDisposed;

			public ProgressTracker(AsyncOperation<object> operation, Action<float> callback, int intervalMs)
			{
				_operation = operation;
				_callback = callback;
				_timer = new Timer(CheckProgress, null, 0, intervalMs);
				_lastProgress = _operation.Progress;
			}

			private void CheckProgress(object state)
			{
				if (Interlocked.CompareExchange(ref _isDisposed, 0, 0) == 1)
					return;

				var currentProgress = _operation.Progress;
				if (!ProgressComparer.AreEqual(currentProgress, _lastProgress))
				{
					_lastProgress = currentProgress;
					ThreadPool.UnsafeQueueUserWorkItem(_ => _callback(currentProgress), null);
				}
			}

			public void Dispose()
			{
				if (Interlocked.Exchange(ref _isDisposed, 1) == 0)
				{
					_timer.Dispose();
				}
			}

			private static class ProgressComparer
			{
				public static readonly float Tolerance = 1E-5f;

				public static bool AreEqual(float a, float b)
				{
					return CoreMathf.Abs(a - b) < Tolerance;
				}
			}
		}

		public static AsyncOperation<T> OnProgress<T>(
			this AsyncOperation<T> operation,
			Action<float> progressCallback,
			int intervalMs = 16)
		{
			if (operation == null)
				throw new ArgumentNullException(nameof(operation));
			if (progressCallback == null)
				throw new ArgumentNullException(nameof(progressCallback));

			var tracker = new ProgressTracker(operation as AsyncOperation<object>, progressCallback, intervalMs);
			operation.Completed += _ => tracker.Dispose();

			return operation;
		}

		public static AsyncOperation<T> OnSuccess<T>(
			this AsyncOperation<T> operation,
			Action<T> successCallback)
		{
			if (operation == null)
				throw new ArgumentNullException(nameof(operation));
			if (successCallback == null)
				throw new ArgumentNullException(nameof(successCallback));

			operation.Completed += op =>
			{
				if (!op.HasError && !((AsyncOperation<T>)op).IsCanceled)
				{
					ThreadPool.UnsafeQueueUserWorkItem(_ => successCallback(op.Result), null);
				}
			};

			return operation;
		}

		public static AsyncOperation<T> OnError<T>(
			this AsyncOperation<T> operation,
			Action<Exception> errorCallback)
		{
			if (operation == null)
				throw new ArgumentNullException(nameof(operation));
			if (errorCallback == null)
				throw new ArgumentNullException(nameof(errorCallback));

			operation.Completed += op =>
			{
				if (op.HasError)
				{
					ThreadPool.UnsafeQueueUserWorkItem(_ => errorCallback(op.Error), null);
				}
			};

			return operation;
		}

		public static AsyncOperation<T> OnCanceled<T>(
			this AsyncOperation<T> operation,
			Action canceledCallback)
		{
			if (operation == null)
				throw new ArgumentNullException(nameof(operation));
			if (canceledCallback == null)
				throw new ArgumentNullException(nameof(canceledCallback));

			operation.Completed += op =>
			{
				if (((AsyncOperation<T>)op).IsCanceled)
				{
					ThreadPool.UnsafeQueueUserWorkItem(_ => canceledCallback(), null);
				}
			};

			return operation;
		}
	}
}
