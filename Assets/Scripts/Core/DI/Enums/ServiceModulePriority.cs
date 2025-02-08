namespace Scarlet.Core.DI.Enums
{
	/// <summary>
	/// 서비스 모듈의 초기화 우선순위를 정의합니다.
	/// 낮은 숫자가 더 높은 우선순위를 가집니다.
	/// </summary>
	public enum ServiceModulePriority
	{
		/// <summary>
		/// 가장 높은 우선순위. 핵심 시스템 및 기반 서비스용
		/// </summary>
		Highest = 0,

		/// <summary>
		/// 높은 우선순위. 주요 게임 시스템용
		/// </summary>
		High = 100,

		/// <summary>
		/// 일반 우선순위. 대부분의 게임 서비스용
		/// </summary>
		Normal = 200,

		/// <summary>
		/// 낮은 우선순위. 부가 기능용
		/// </summary>
		Low = 300,

		/// <summary>
		/// 가장 낮은 우선순위. 디버깅, 로깅 등 보조 시스템용
		/// </summary>
		Lowest = 400
	}
}
