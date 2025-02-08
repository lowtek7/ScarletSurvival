using Scarlet.Core.Event.Enums;

namespace Scarlet.Core.Event
{
	/// <summary>
	/// EventBus 설정을 위한 구성 클래스
	/// </summary>
	public class EventBusConfiguration
	{
		/// <summary>
		/// 이벤트 버퍼의 크기 (2의 제곱수여야 함)
		/// </summary>
		public int BufferSize { get; set; } = 4096;

		/// <summary>
		/// 이벤트 타입당 최대 핸들러 수
		/// </summary>
		public int MaxHandlersPerEvent { get; set; } = 32;

		/// <summary>
		/// 스핀 대기 최대 횟수
		/// </summary>
		public int SpinWaitMaxCount { get; set; } = 10;

		/// <summary>
		/// 버퍼 가득 참 정책
		/// </summary>
		public BufferFullPolicy BufferFullPolicy { get; set; } = BufferFullPolicy.DropEvent;
	}
}
