using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Scarlet.Core.Collection;
using Scarlet.Core.Event.Interfaces;
using Scarlet.Core.Services;

namespace Scarlet.Core.Event
{
	public class EventBus : ServiceBase, IEventBus
	{
		private readonly Dictionary<Type, SortedDictionary<int, List<Subscription>>> subscriptions;
		private readonly RingBuffer<PendingEvent> eventQueue;
		private readonly Dictionary<Type, ProcessEventDelegate> eventProcessors;
		private readonly object subscriptionLock = new();
		private volatile bool isProcessing;

		private const int DEFAULT_BUFFER_SIZE = 4096;
		private const int MAX_EVENTS_PER_UPDATE = 128;

		private delegate void ProcessEventDelegate(IGameEvent evt);

		private class Subscription
		{
			public int Priority { get; }
			public Delegate Handler { get; }

			public Subscription(int priority, Delegate handler)
			{
				Priority = priority;
				Handler = handler;
			}
		}

		private struct PendingEvent
		{
			public IGameEvent Event;
			public long Timestamp;
		}

		public EventBus(CoreSandbox core) : base(core)
		{
			subscriptions = new Dictionary<Type, SortedDictionary<int, List<Subscription>>>();
			eventQueue = new RingBuffer<PendingEvent>(DEFAULT_BUFFER_SIZE);
			eventProcessors = new Dictionary<Type, ProcessEventDelegate>();
		}

		public void Subscribe<T>(Action<T> handler, int priority = 0) where T : class, IGameEvent
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			var eventType = typeof(T);
			lock (subscriptionLock)
			{
				if (!eventProcessors.ContainsKey(eventType))
				{
					eventProcessors[eventType] = (evt) => ProcessEvent((T)evt);
				}

				if (!subscriptions.TryGetValue(eventType, out var priorityMap))
				{
					priorityMap = new SortedDictionary<int, List<Subscription>>();
					subscriptions[eventType] = priorityMap;
				}

				if (!priorityMap.TryGetValue(priority, out var handlers))
				{
					handlers = new List<Subscription>();
					priorityMap[priority] = handlers;
				}

				if (!handlers.Any(s => s.Handler == (Delegate)handler))
				{
					handlers.Add(new Subscription(priority, handler));
					Logger.LogDebug($"Subscribed to {eventType.Name} with priority {priority}");
				}
			}
		}

		public void Unsubscribe<T>(Action<T> handler) where T : class, IGameEvent
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			var eventType = typeof(T);
			lock (subscriptionLock)
			{
				if (!subscriptions.TryGetValue(eventType, out var priorityMap))
					return;

				var emptyPriorities = new List<int>();
				foreach (var priority in priorityMap.Keys)
				{
					var handlers = priorityMap[priority];
					handlers.RemoveAll(s => s.Handler == (Delegate)handler);

					if (handlers.Count == 0)
						emptyPriorities.Add(priority);
				}

				foreach (var priority in emptyPriorities)
				{
					priorityMap.Remove(priority);
				}

				if (priorityMap.Count == 0)
				{
					subscriptions.Remove(eventType);
					eventProcessors.Remove(eventType);
				}
			}
		}

		public bool Publish<T>(T gameEvent) where T : class, IGameEvent
		{
			if (gameEvent == null)
				throw new ArgumentNullException(nameof(gameEvent));

			var pending = new PendingEvent
			{
				Event = gameEvent,
				Timestamp = Stopwatch.GetTimestamp()
			};

			return eventQueue.TryEnqueue(pending);
		}

		public void Clear()
		{
			lock (subscriptionLock)
			{
				subscriptions.Clear();
				eventProcessors.Clear();
			}

			while (eventQueue.TryDequeue(out _)) { }
		}

		private void ProcessEvent<T>(T gameEvent) where T : class, IGameEvent
		{
			var eventType = typeof(T);
			List<Subscription> subscriptions;

			lock (subscriptionLock)
			{
				if (!this.subscriptions.TryGetValue(eventType, out var priorityMap))
					return;

				subscriptions = priorityMap.Values
					.SelectMany(x => x)
					.ToList();
			}

			foreach (var subscription in subscriptions)
			{
				try
				{
					((Action<T>)subscription.Handler)(gameEvent);
				}
				catch (Exception ex)
				{
					Logger.LogError($"Error handling event {eventType.Name}: {ex.Message}");
				}
			}
		}

		public override void Update()
		{
			if (isProcessing)
				return;

			try
			{
				isProcessing = true;
				int processedCount = 0;

				while (processedCount < MAX_EVENTS_PER_UPDATE &&
					   eventQueue.TryDequeue(out var pending))
				{
					if (eventProcessors.TryGetValue(pending.Event.GetType(), out var processor))
					{
						try
						{
							processor(pending.Event);
						}
						catch (Exception ex)
						{
							Logger.LogError($"Error processing event: {ex.Message}");
						}
					}

					processedCount++;
				}
			}
			finally
			{
				isProcessing = false;
			}
		}

		public override void Cleanup()
		{
			Clear();
			base.Cleanup();
		}
	}
}
