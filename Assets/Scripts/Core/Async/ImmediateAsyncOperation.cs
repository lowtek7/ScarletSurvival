using System;
using System.Threading.Tasks;
using Scarlet.Core.Async.Enums;
using Scarlet.Core.Async.Interfaces;

namespace Scarlet.Core.Async
{
	// 즉시 완료되는 작업을 위한 유틸리티 클래스
	public sealed class ImmediateAsyncOperation<T> : AsyncOperationBase<T>
	{
		public ImmediateAsyncOperation(T result)
		{
			Result = result;
			Progress = 1f;
			Status = AsyncOperationStatus.Completed;
		}

		private ImmediateAsyncOperation()
		{
		}

		public static ImmediateAsyncOperation<T> FromResult(T result)
		{
			return new ImmediateAsyncOperation<T>(result);
		}

		public static ImmediateAsyncOperation<T> FromError(Exception error)
		{
			return new ImmediateAsyncOperation<T>
			{
				Error = error,
				Status = AsyncOperationStatus.Failed
			};
		}
	}
}
