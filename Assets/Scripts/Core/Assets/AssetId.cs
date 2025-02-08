using System;
using Scarlet.Core.Assets.Enums;

namespace Scarlet.Core.Assets
{
	public readonly struct AssetId : IEquatable<AssetId>
	{
		public string Value { get; }
		public AssetType Type { get; }

		public AssetId(string value, AssetType type)
		{
			Value = value;
			Type = type;
		}

		public bool Equals(AssetId other)
		{
			return Value == other.Value && Type == other.Type;
		}

		public override bool Equals(object obj)
		{
			return obj is AssetId other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Value, (int)Type);
		}

		public static bool operator ==(AssetId left, AssetId right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(AssetId left, AssetId right)
		{
			return !left.Equals(right);
		}
	}
}
