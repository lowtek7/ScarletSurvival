using Scarlet.Core.Event.Interfaces;

namespace Scarlet.Core.Pooling.Interfaces
{
	public interface IEventPool
	{
		T Get<T>() where T : class, IGameEvent, new();
		void Return(IGameEvent gameEvent);
	}
}
