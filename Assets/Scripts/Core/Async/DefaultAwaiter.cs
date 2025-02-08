using System;
using System.Runtime.CompilerServices;
using Scarlet.Core.Async.Interfaces;

namespace Scarlet.Core.Async
{
	public class DefaultAwaiter<T> : INotifyCompletion
	{
		private readonly IAsyncOperation<T> operation;
		private Action continuation;

		public DefaultAwaiter(IAsyncOperation<T> operation)
		{
			this.operation = operation;
		}

		public bool IsCompleted => operation.IsCompleted;

		public T GetResult()
		{
			if (operation.HasError)
				throw operation.Error;
			return operation.Result;
		}

		public void OnCompleted(Action continuationAction)
		{
			this.continuation = continuationAction;
			if (operation.IsCompleted)
			{
				continuationAction();
			}
		}

		public void Complete()
		{
			continuation?.Invoke();
		}
	}
}
