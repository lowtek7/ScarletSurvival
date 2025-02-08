namespace Scarlet.Core.Event.Interfaces
{
	public interface IGameEvent
	{
		EventId Id { get; }
		void Reset();
	}
}
