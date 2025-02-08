using System;
using Scarlet.Core.Math.Adapters;

namespace Scarlet.Core.Math
{
	/// <summary>
	/// 수학 연산 제공자에 대한 전역 접근을 관리하는 정적 클래스입니다.
	/// 애플리케이션에서 사용될 IMathProvider 인스턴스를 중앙에서 관리합니다.
	///
	/// 주요 기능:
	/// - 수학 연산 제공자의 전역 접근점 제공
	/// - 환경에 따른 적절한 수학 연산 구현체 제공
	/// - 수학 연산 제공자의 초기화 상태 관리
	///
	/// 사용 예시:
	/// <code>
	/// // 초기화 (애플리케이션 시작 시 한 번)
	/// MathService.Initialize(new UnityMathProvider()); // Unity 환경
	/// // 또는
	/// MathService.Initialize(new CoreMathProvider());  // 순수 C# 환경
	///
	/// // 사용
	/// float sinValue = MathService.Current.Sin(angle);
	/// </code>
	/// </summary>
	public static class MathService
	{
		private static IMathProvider instance;

		/// <summary>
		/// 현재 설정된 수학 연산 제공자에 대한 접근자입니다.
		/// 제공자가 초기화되지 않은 상태에서 접근하면 InvalidOperationException이 발생합니다.
		/// </summary>
		public static IMathProvider Current
		{
			get
			{
				if (instance == null)
				{
					throw new InvalidOperationException(
						"수학 연산 제공자가 초기화되지 않았습니다. Initialize()를 먼저 호출해주세요.");
				}
				return instance;
			}
		}

		/// <summary>
		/// 수학 연산 제공자를 초기화합니다.
		/// 애플리케이션 시작 시 반드시 한 번 호출되어야 합니다.
		/// </summary>
		/// <param name="provider">사용할 IMathProvider 구현체</param>
		/// <exception cref="ArgumentNullException">provider가 null인 경우</exception>
		public static void Initialize(IMathProvider provider)
		{
			instance = provider ?? throw new ArgumentNullException(nameof(provider),
				"수학 연산 제공자는 null일 수 없습니다.");
		}

		/// <summary>
		/// 현재 설정된 수학 연산 제공자를 초기화합니다.
		/// 주로 테스트나 리소스 정리 시에 사용됩니다.
		/// </summary>
		public static void Reset()
		{
			instance = null;
		}
	}
}
