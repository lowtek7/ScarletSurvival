namespace Scarlet.Core.DI.Enums
{
	/// <summary>
	/// 서비스의 생명주기를 정의합니다.
	/// </summary>
	public enum ServiceLifetime
	{
		/// <summary>
		/// 싱글톤: 애플리케이션 전체에서 하나의 인스턴스 공유
		/// </summary>
		Singleton,

		/// <summary>
		/// 스코프: 같은 스코프 내에서 인스턴스 공유
		/// </summary>
		Scoped,

		/// <summary>
		/// 트랜지언트: 매번 새로운 인스턴스 생성
		/// </summary>
		Transient
	}
}
