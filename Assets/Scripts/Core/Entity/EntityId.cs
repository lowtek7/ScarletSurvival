using System;

namespace Scarlet.Core.Entity
{
	public readonly struct EntityId : IEquatable<EntityId>
	{
		// 비트 할당
		private const int TYPE_BITS = 16; // 타입에 16비트 할당 (0 ~ 65,535)
		private const int WORLD_BITS = 16; // 월드 ID에 16비트 할당 (0 ~ 65,535)
		private const int INSTANCE_BITS = 32; // 인스턴스에 32비트 할당 (0 ~ 4,294,967,295)

		// 비트 시프트 위치
		private const int TYPE_SHIFT = INSTANCE_BITS + WORLD_BITS; // 48비트
		private const int WORLD_SHIFT = INSTANCE_BITS; // 32비트
		private const int INSTANCE_SHIFT = 0; // 0비트

		// 비트 마스크
		private const long TYPE_MASK = ((1L << TYPE_BITS) - 1) << TYPE_SHIFT;
		private const long WORLD_MASK = ((1L << WORLD_BITS) - 1) << WORLD_SHIFT;
		private const long INSTANCE_MASK = (1L << INSTANCE_BITS) - 1;

		public ushort Type { get; }
		public ushort WorldId { get; }
		public uint Instance { get; }

		public EntityId(ushort type, ushort worldId, uint instance)
		{
			Type = type;
			WorldId = worldId;
			Instance = instance;
		}

		// 64비트 정수로 압축
		public long ToInt64()
		{
			long result = 0;
			result |= ((long)Type << TYPE_SHIFT) & TYPE_MASK;
			result |= ((long)WorldId << WORLD_SHIFT) & WORLD_MASK;
			result |= ((long)Instance << INSTANCE_SHIFT) & INSTANCE_MASK;
			return result;
		}

		// 64비트 정수에서 EntityId 생성
		public static EntityId FromInt64(long value)
		{
			ushort type = (ushort)((value & TYPE_MASK) >> TYPE_SHIFT);
			ushort worldId = (ushort)((value & WORLD_MASK) >> WORLD_SHIFT);
			uint instance = (uint)(value & INSTANCE_MASK);

			return new EntityId(type, worldId, instance);
		}

		// ToString 오버라이드로 디버깅 지원
		public override string ToString()
		{
			return $"EntityId[Type: {Type}, World: {WorldId}, Instance: {Instance}]";
		}

		// 64비트 정수 형태로 출력
		public string ToHexString()
		{
			return $"0x{ToInt64():X16}";
		}

		// Equals 및 GetHashCode 구현
		public bool Equals(EntityId other)
		{
			return Type == other.Type &&
			       WorldId == other.WorldId &&
			       Instance == other.Instance;
		}

		public override bool Equals(object obj)
		{
			return obj is EntityId other && Equals(other);
		}

		public override int GetHashCode()
		{
			return ToInt64().GetHashCode();
		}

		// 연산자 오버로딩
		public static bool operator ==(EntityId left, EntityId right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(EntityId left, EntityId right)
		{
			return !left.Equals(right);
		}

		// 유효성 검사 메서드
		public static bool IsValid(long value)
		{
			var type = (ushort)((value & TYPE_MASK) >> TYPE_SHIFT);
			var worldId = (ushort)((value & WORLD_MASK) >> WORLD_SHIFT);
			var instance = (uint)(value & INSTANCE_MASK);

			return type != 0; // 타입 0은 Invalid로 취급하거나 다른 유효성 검사 조건 추가
		}
	}
}
