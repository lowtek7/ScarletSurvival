namespace Scarlet.Core.Entity.Interfaces
{
	public interface IEntity
	{
		EntityId Id { get; }
		bool IsActive { get; }
		void Initialize();
		void Cleanup();
	}
}
