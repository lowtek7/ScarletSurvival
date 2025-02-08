using Scarlet.Core.Entity.Interfaces;
using Scarlet.Core.Event.Interfaces;
using Scarlet.Core.Logging.Enums;
using Scarlet.Core.Logging.Interfaces;

namespace Scarlet.Core.Entity
{
	public abstract class Entity : IEntity
	{
		public EntityId Id { get; }
		public bool IsActive { get; private set; }

		protected ILogger Logger { get; }
		protected IEventBus EventBus { get; }

		protected Entity(EntityId id, ILogger logger, IEventBus eventBus)
		{
			Id = id;
			Logger = logger;
			EventBus = eventBus;
		}

		public virtual void Initialize()
		{
			IsActive = true;
			Logger.Log(LogLevel.Debug, $"Entity {Id} initialized");
		}

		public virtual void Cleanup()
		{
			IsActive = false;
			Logger.Log(LogLevel.Debug, $"Entity {Id} cleaned up");
		}
	}
}
