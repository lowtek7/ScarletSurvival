namespace Scarlet.Core.Pooling.Interfaces
{
	public interface IObjectPool<T> where T : IPoolable
	{
		T Acquire();
		void Release(T item);
		void PreWarm(int count);
		int ActiveCount { get; }
		int PoolSize { get; }
	}
}
