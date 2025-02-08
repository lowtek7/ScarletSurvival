using Scarlet.Core.DI.Enums;

namespace Scarlet.Core.DI.Interfaces
{
	/// <summary>
	/// 서비스 모듈을 정의하는 인터페이스
	/// </summary>
	public interface IServiceModule
	{
		void RegisterServices(IServiceCollection services);
		ServiceModulePriority Priority { get; }
	}
}
