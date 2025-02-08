using System.Collections.Generic;

namespace Scarlet.Core.Event
{
	public class EventRegistry
	{
		private readonly Dictionary<string, EventId> namedEvents = new();
		private readonly Dictionary<EventId, string> eventNames = new();
		private readonly Dictionary<ushort, ushort> nextIds = new();
		private readonly object @lock = new();

		public EventRegistry()
		{
			// 기본 이벤트들 등록
			RegisterBuiltInEvents();
		}

		private void RegisterBuiltInEvents()
		{
			// 시스템 이벤트 등록
			Register("System.None", EventId.System.None);
			Register("System.Initialize", EventId.System.Initialize);
			Register("System.Shutdown", EventId.System.Shutdown);

			// 엔티티 이벤트 등록
			Register("Entity.Created", EventId.Entity.Created);
			Register("Entity.Destroyed", EventId.Entity.Destroyed);
			Register("Entity.Spawned", EventId.Entity.Spawned);
		}

		public void Register(string eventName, EventId eventId)
		{
			lock (@lock)
			{
				namedEvents[eventName] = eventId;
				eventNames[eventId] = eventName;
			}
		}

		public EventId RegisterCustomEvent(string eventName, ushort category)
		{
			lock (@lock)
			{
				if (!nextIds.TryGetValue(category, out var nextId))
				{
					nextId = 1;
				}

				var eventId = new EventId(category, nextId++);
				nextIds[category] = nextId;

				Register(eventName, eventId);
				return eventId;
			}
		}

		public bool TryGetEventId(string eventName, out EventId eventId)
		{
			return namedEvents.TryGetValue(eventName, out eventId);
		}

		public bool TryGetEventName(EventId eventId, out string eventName)
		{
			return eventNames.TryGetValue(eventId, out eventName);
		}
	}
}
