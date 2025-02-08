using System.Collections.Generic;

namespace Scarlet.Core.DI.Interfaces
{
	/// <summary>
	/// 서비스 컬렉션 인터페이스를 정의합니다.
	/// </summary>
	public interface IServiceCollection : IList<ServiceDescriptor>
	{
		/// <summary>
		/// 서비스 등록 여부를 확인합니다.
		/// </summary>
		bool IsRegistered<T>();

		/// <summary>
		/// 등록된 서비스 설명자를 가져옵니다.
		/// </summary>
		IServiceDescriptor GetDescriptor<T>();
	}
}
