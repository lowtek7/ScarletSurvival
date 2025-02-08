using System;
using Scarlet.Core.Math.Factories;

namespace Scarlet.Core.Math
{
	/// <summary>
	/// 벡터 팩토리의 전역 접근을 관리하는 정적 클래스입니다.
	/// 이 클래스는 애플리케이션에서 사용될 IVectorFactory 인스턴스를 중앙에서 관리합니다.
	///
	/// 주요 기능:
	/// - 벡터 생성 팩토리의 전역 접근점 제공
	/// - 환경에 따른 적절한 벡터 구현체 제공 (예: Unity, 순수 C#)
	/// - 벡터 팩토리 초기화 상태 관리
	///
	/// 사용 예시:
	/// <code>
	/// // 초기화 (애플리케이션 시작 시 한 번)
	/// VectorFactoryProvider.Initialize(new UnityVectorFactory()); // Unity 환경
	/// // 또는
	/// VectorFactoryProvider.Initialize(new CoreVectorFactory());  // 순수 C# 환경
	///
	/// // 사용
	/// IVector3 newVector = VectorFactoryProvider.Current.CreateVector3(1f, 2f, 3f);
	/// </code>
	/// </summary>
	public static class VectorFactoryProvider
	{
		/// <summary>
		/// 현재 설정된 벡터 팩토리 인스턴스입니다.
		/// null일 경우 InvalidOperationException이 발생합니다.
		/// </summary>
		private static IVectorFactory instance;

		/// <summary>
		/// 현재 설정된 벡터 팩토리에 대한 접근자입니다.
		/// 팩토리가 초기화되지 않은 상태에서 접근하면 InvalidOperationException이 발생합니다.
		///
		/// 예외:
		/// - InvalidOperationException: 벡터 팩토리가 초기화되지 않은 경우
		/// </summary>
		public static IVectorFactory Current
		{
			get
			{
				if (instance == null)
				{
					throw new InvalidOperationException(
						"VectorFactory is not initialized. need call Initialize()");
				}

				return instance;
			}
		}

		/// <summary>
		/// 벡터 팩토리를 초기화합니다.
		/// 애플리케이션 시작 시 반드시 한 번 호출되어야 합니다.
		///
		/// 매개변수:
		/// - factory: 사용할 벡터 팩토리 인스턴스
		///
		/// 예외:
		/// - ArgumentNullException: factory가 null인 경우
		///
		/// 사용 예시:
		/// <code>
		/// // Unity 환경에서의 초기화
		/// public class GameInitializer : MonoBehaviour
		/// {
		///     private void Awake()
		///     {
		///         VectorFactoryProvider.Initialize(new UnityVectorFactory());
		///     }
		/// }
		///
		/// // 테스트 환경에서의 초기화
		/// [TestFixture]
		/// public class VectorTests
		/// {
		///     [OneTimeSetUp]
		///     public void SetUp()
		///     {
		///         VectorFactoryProvider.Initialize(new CoreVectorFactory());
		///     }
		/// }
		/// </code>
		/// </summary>
		/// <param name="factory">사용할 IVectorFactory 구현체</param>
		public static void Initialize(IVectorFactory factory)
		{
			instance = factory ?? throw new ArgumentNullException(nameof(factory),
				"VectorFactory Instance is can not null.");
		}

		/// <summary>
		/// 현재 설정된 벡터 팩토리를 초기화합니다.
		/// 주로 테스트나 리소스 정리 시에 사용됩니다.
		///
		/// 사용 예시:
		/// <code>
		/// [TestFixture]
		/// public class VectorTests
		/// {
		///     [TearDown]
		///     public void Cleanup()
		///     {
		///         VectorFactoryProvider.Reset();
		///     }
		/// }
		/// </code>
		/// </summary>
		public static void Reset()
		{
			instance = null;
		}
	}
}
