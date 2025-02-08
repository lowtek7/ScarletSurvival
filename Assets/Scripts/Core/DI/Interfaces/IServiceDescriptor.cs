using System;
using Scarlet.Core.DI.Enums;

namespace Scarlet.Core.DI.Interfaces
{
	/// <summary>
	/// 서비스 등록 정보를 정의합니다.
	/// </summary>
	public interface IServiceDescriptor
	{
		/// <summary>
		/// 서비스의 타입
		/// </summary>
		Type ServiceType { get; }

		/// <summary>
		/// 구현체의 타입
		/// </summary>
		Type ImplementationType { get; }

		/// <summary>
		/// 서비스의 생명주기
		/// </summary>
		ServiceLifetime Lifetime { get; }

		/// <summary>
		/// 서비스 인스턴스 또는 팩토리
		/// </summary>
		object ImplementationInstance { get; }
	}
}
