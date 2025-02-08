using System;

namespace Scarlet.Core
{
	public readonly struct CoreId : IEquatable<CoreId>
	{
		private readonly Guid value;

		public CoreId(Guid value) => this.value = value;
		public static CoreId New() => new(Guid.NewGuid());
		public static CoreId Parse(string value) => new(Guid.Parse(value));

		public override int GetHashCode() => value.GetHashCode();
		public bool Equals(CoreId other) => value.Equals(other.value);
		public override bool Equals(object obj) => obj is CoreId other && Equals(other);
		public override string ToString() => value.ToString("N");

		public static bool operator ==(CoreId left, CoreId right) => left.Equals(right);
		public static bool operator !=(CoreId left, CoreId right) => !(left == right);
	}
}
