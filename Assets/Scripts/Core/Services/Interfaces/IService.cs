namespace Scarlet.Core.Services.Interfaces
{
	public interface IService
	{
		CoreId OwnerId { get; }
		void Initialize();
		void Cleanup();
	}

	public interface IUpdatableService
	{
		void Update();
	}
}
