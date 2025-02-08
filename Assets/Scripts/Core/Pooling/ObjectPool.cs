using System;
using System.Collections.Generic;
using Scarlet.Core.Math.Scarlet.Core.Math;
using Scarlet.Core.Pooling.Interfaces;

namespace Scarlet.Core.Pooling
{
	public class ObjectPool<T> : IObjectPool<T> where T : class, IPoolable
	{
		private readonly Stack<T> pool;
		private readonly Func<T> factory;
		private readonly Action<T> onAcquire;
		private readonly Action<T> onRelease;
		private readonly int maxSize;
		private int activeCount;

		public int ActiveCount => activeCount;
		public int PoolSize => pool.Count;
		public int TotalSize => activeCount + PoolSize;

		/// <summary>
		/// 객체 풀 생성자
		/// </summary>
		/// <param name="factory">새 객체를 생성하는 팩토리 함수</param>
		/// <param name="onAcquire">객체가 풀에서 획득될 때 실행될 액션</param>
		/// <param name="onRelease">객체가 풀로 반환될 때 실행될 액션</param>
		/// <param name="initialSize">초기 풀 크기</param>
		/// <param name="maxSize">최대 풀 크기</param>
		public ObjectPool(
			Func<T> factory,
			Action<T> onAcquire = null,
			Action<T> onRelease = null,
			int initialSize = 0,
			int maxSize = 1000)
		{
			this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
			this.onAcquire = onAcquire;
			this.onRelease = onRelease;
			this.maxSize = maxSize;
			pool = new Stack<T>(initialSize);

			if (initialSize > 0)
			{
				PreWarm(initialSize);
			}
		}

		/// <summary>
		/// 풀을 지정된 수만큼 미리 채웁니다.
		/// </summary>
		/// <param name="count">미리 생성할 객체 수</param>
		/// <exception cref="ArgumentException">count가 0보다 작은 경우</exception>
		public void PreWarm(int count)
		{
			if (count < 0)
				throw new ArgumentException("PreWarm count must be non-negative.", nameof(count));

			// 최대 크기를 초과하지 않도록 함
			int itemsToCreate = CoreMathf.RoundToInt(CoreMathf.Min(count, maxSize - pool.Count));

			for (int i = 0; i < itemsToCreate; i++)
			{
				var item = factory();

				try
				{
					// 초기화 호출
					item.OnDespawn();
					onRelease?.Invoke(item);
				}
				catch (Exception e)
				{
					//Debug.LogError($"Error during PreWarm initialization: {e}");
				}

				pool.Push(item);
			}
		}

		/// <summary>
		/// 풀에서 객체를 가져옵니다.
		/// </summary>
		/// <returns>활성화된 객체</returns>
		public T Acquire()
		{
			T item;

			if (pool.Count > 0)
			{
				item = pool.Pop();
			}
			else
			{
				if (TotalSize >= maxSize)
				{
					//Debug.LogWarning($"Object pool has reached its maximum size of {_maxSize}");
					return null;
				}

				item = factory();
			}

			activeCount++;

			try
			{
				item.OnSpawn();
				onAcquire?.Invoke(item);
			}
			catch (Exception e)
			{
				//Debug.LogError($"Error during item initialization: {e}");
				Release(item);
				return null;
			}

			return item;
		}

		/// <summary>
		/// 객체를 풀로 반환합니다.
		/// </summary>
		/// <param name="item">반환할 객체</param>
		public void Release(T item)
		{
			if (item == null)
			{
				// Debug.LogWarning("Attempting to release null item to pool.");
				return;
			}

			try
			{
				item.OnDespawn();
				onRelease?.Invoke(item);
			}
			catch (Exception e)
			{
				//Debug.LogError($"Error during item release: {e}");
			}

			if (pool.Count < maxSize)
			{
				pool.Push(item);
			}

			activeCount--;
		}

		/// <summary>
		/// 현재 풀의 상태 정보를 반환합니다.
		/// </summary>
		public string GetPoolInfo()
		{
			return $"Pool Status: Active={ActiveCount}, Available={PoolSize}, " +
			       $"Total={TotalSize}, Max={maxSize}";
		}

		/// <summary>
		/// 풀을 초기화하고 모든 객체를 제거합니다.
		/// </summary>
		public void Clear()
		{
			pool.Clear();
			activeCount = 0;
		}
	}
}
