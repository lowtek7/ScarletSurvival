using System;
using Scarlet.Core.Services.Interfaces;

namespace Scarlet.Core.Event.Interfaces
{
	/// <summary>
	/// 고성능 이벤트 처리를 위한 이벤트 버스 인터페이스
	/// </summary>
	public interface IEventBus : IService, IUpdatableService
	{
		/// <summary>
		/// 이벤트 핸들러를 등록합니다.
		/// </summary>
		/// <typeparam name="T">이벤트 타입</typeparam>
		/// <param name="handler">이벤트 핸들러</param>
		/// <param name="priority">핸들러 우선순위 (낮은 값이 높은 우선순위)</param>
		/// <exception cref="ArgumentNullException">handler가 null인 경우</exception>
		void Subscribe<T>(Action<T> handler, int priority = 0) where T : class, IGameEvent;

		/// <summary>
		/// 등록된 이벤트 핸들러를 제거합니다.
		/// </summary>
		/// <typeparam name="T">이벤트 타입</typeparam>
		/// <param name="handler">제거할 이벤트 핸들러</param>
		/// <exception cref="ArgumentNullException">handler가 null인 경우</exception>
		void Unsubscribe<T>(Action<T> handler) where T : class, IGameEvent;

		/// <summary>
		/// 이벤트를 발행합니다.
		/// </summary>
		/// <typeparam name="T">이벤트 타입</typeparam>
		/// <param name="gameEvent">발행할 이벤트</param>
		/// <returns>이벤트가 성공적으로 큐에 추가되었는지 여부</returns>
		/// <exception cref="ArgumentNullException">gameEvent가 null인 경우</exception>
		bool Publish<T>(T gameEvent) where T : class, IGameEvent;

		/// <summary>
		/// 모든 이벤트와 핸들러를 정리합니다.
		/// </summary>
		void Clear();
	}
}
