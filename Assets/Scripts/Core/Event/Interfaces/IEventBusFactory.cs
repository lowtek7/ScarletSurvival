namespace Scarlet.Core.Event.Interfaces
{
	/// <summary>
	/// EventBus 생성을 위한 팩토리 인터페이스
	/// </summary>
	public interface IEventBusFactory
	{
		/// <summary>
		/// 새로운 EventBus 인스턴스를 생성합니다.
		/// </summary>
		/// <param name="bufferSize">이벤트 버퍼 크기 (2의 제곱수여야 함)</param>
		/// <returns>생성된 EventBus 인스턴스</returns>
		IEventBus Create(int bufferSize = 4096);
	}
}
