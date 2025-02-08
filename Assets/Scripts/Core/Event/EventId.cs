using System;

namespace Scarlet.Core.Event
{
	public readonly struct EventId : IEquatable<EventId>
	{
		private readonly uint value;

		// 상위 16비트: 카테고리
		// 하위 16비트: ID
		public ushort Category => (ushort)(value >> 16);
		public ushort Id => (ushort)value;

		// 미리 정의된 카테고리
		public static class Categories
		{
			public const ushort System = 0;
			public const ushort Entity = 1;
			public const ushort Combat = 2;
			public const ushort Item = 3;
			public const ushort UI = 4;
			public const ushort Custom = 0xF000;  // 사용자 정의 카테고리 시작점
		}

		private EventId(uint value)
		{
			this.value = value;
		}

		public EventId(ushort category, ushort id)
		{
			value = ((uint)category << 16) | id;
		}

		// 미리 정의된 시스템 이벤트들
		public static class System
		{
			public static readonly EventId None = new(Categories.System, 0);
			public static readonly EventId Initialize = new(Categories.System, 1);
			public static readonly EventId Shutdown = new(Categories.System, 2);
			// ... 기타 시스템 이벤트
		}

		public static class Entity
		{
			public static readonly EventId Created = new(Categories.Entity, 1);
			public static readonly EventId Destroyed = new(Categories.Entity, 2);
			public static readonly EventId Spawned = new(Categories.Entity, 3);
			// ... 기타 엔티티 이벤트
		}

		public bool Equals(EventId other) => value == other.value;
		public override bool Equals(object obj) => obj is EventId other && Equals(other);
		public override int GetHashCode() => (int)value;

		public static bool operator ==(EventId left, EventId right) => left.Equals(right);
		public static bool operator !=(EventId left, EventId right) => !left.Equals(right);

		public override string ToString() => $"{Category:X4}:{Id:X4}";
	}
}
