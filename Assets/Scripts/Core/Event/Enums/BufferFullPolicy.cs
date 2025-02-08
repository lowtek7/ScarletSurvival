namespace Scarlet.Core.Event.Enums
{
	/// <summary>
	/// 버퍼 가득 참 처리 정책
	/// </summary>
	public enum BufferFullPolicy
	{
		/// <summary>
		/// 새 이벤트를 드롭
		/// </summary>
		DropEvent,

		/// <summary>
		/// 공간이 생길 때까지 대기
		/// </summary>
		WaitForSpace,

		/// <summary>
		/// 예외 발생
		/// </summary>
		ThrowException
	}
}
