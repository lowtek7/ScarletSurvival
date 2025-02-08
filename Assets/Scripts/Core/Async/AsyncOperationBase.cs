using System;
using System.Threading;
using Scarlet.Core.Async.Enums;
using Scarlet.Core.Async.Interfaces;

namespace Scarlet.Core.Async
{
	public abstract class AsyncOperationBase<T> : IAsyncOperation<T>, IDisposable
	{
		private int status;
		protected event Action<IAsyncOperation<T>> CompletedEvent;

		protected T Result { get; set; }
		protected Exception Error { get; set; }
		protected float Progress { get; set; }

		protected AsyncOperationBase(AsyncOperationStatus initialStatus = AsyncOperationStatus.Running)
		{
			Interlocked.Exchange(ref status, (int)initialStatus);
		}

		public virtual AsyncOperationStatus Status
		{
			get => (AsyncOperationStatus)Interlocked.CompareExchange(ref status, 0, 0);
			protected set => Interlocked.Exchange(ref status, (int)value);
		}


		public bool IsCompleted => Status >= AsyncOperationStatus.Completed;
		public bool HasError => Status == AsyncOperationStatus.Failed;
		T IAsyncOperation<T>.Result => Result;
		Exception IAsyncOperation<T>.Error => Error;
		float IAsyncOperation<T>.Progress => Progress;

		public event Action<IAsyncOperation<T>> Completed
		{
			add
			{
				if (IsCompleted)
				{
					value?.Invoke(this);
				}
				else
				{
					CompletedEvent += value;
				}
			}
			remove => CompletedEvent -= value;
		}

		protected virtual void OnCompleted()
		{
			var handler = GetAndClearCompletedEvent();
			handler?.Invoke(this);
		}

		public virtual void Dispose()
		{
			CompletedEvent = null;
			Interlocked.Exchange(ref status, (int)AsyncOperationStatus.Disposed);
		}

		// 이벤트 핸들러를 안전하게 가져오고 초기화하는 protected 메서드 추가
		protected Action<IAsyncOperation<T>> GetAndClearCompletedEvent()
		{
			var handler = CompletedEvent;
			CompletedEvent = null;
			return handler;
		}

		public virtual object GetAwaiter() => new DefaultAwaiter<T>(this);
	}
}
