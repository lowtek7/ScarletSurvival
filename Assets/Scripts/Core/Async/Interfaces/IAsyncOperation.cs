using System;
using System.Threading.Tasks;

namespace Scarlet.Core.Async.Interfaces
{
	public interface IAsyncOperation<T>
	{
		bool IsCompleted { get; }
		bool HasError { get; }
		Exception Error { get; }
		T Result { get; }
		float Progress { get; }
		event Action<IAsyncOperation<T>> Completed;

		object GetAwaiter();
	}
}
