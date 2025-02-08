using System;
using Scarlet.Core.Math.Interfaces;

namespace Scarlet.Core.Math
{
	public struct CoreVector3 : IVector3
	{
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }

		public float Magnitude => MathF.Sqrt(SqrMagnitude);
		public float SqrMagnitude => X * X + Y * Y + Z * Z;

		public IVector Normalized
		{
			get
			{
				float mag = Magnitude;
				if (mag > 1E-05f)
					return new CoreVector3(X / mag, Y / mag, Z / mag);
				return new CoreVector3();
			}
		}

		public CoreVector3(float x, float y, float z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void Normalize()
		{
			float mag = Magnitude;
			if (mag > 1E-05f)
			{
				X /= mag;
				Y /= mag;
				Z /= mag;
			}
			else
			{
				X = 0;
				Y = 0;
				Z = 0;
			}
		}

		// 연산자 오버로딩
		public static CoreVector3 operator +(CoreVector3 a, CoreVector3 b)
			=> new CoreVector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

		public static CoreVector3 operator -(CoreVector3 a, CoreVector3 b)
			=> new CoreVector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

		public static CoreVector3 operator *(CoreVector3 a, float d)
			=> new CoreVector3(a.X * d, a.Y * d, a.Z * d);

		public static CoreVector3 operator /(CoreVector3 a, float d)
			=> new CoreVector3(a.X / d, a.Y / d, a.Z / d);

		public static CoreVector3 operator -(CoreVector3 a)
			=> new CoreVector3(-a.X, -a.Y, -a.Z);

		// 상수 벡터들
		public static IVector3 Zero => new CoreVector3(0f, 0f, 0f);
		public static IVector3 One => new CoreVector3(1f, 1f, 1f);
		public static IVector3 Up => new CoreVector3(0f, 1f, 0f);
		public static IVector3 Down => new CoreVector3(0f, -1f, 0f);
		public static IVector3 Left => new CoreVector3(-1f, 0f, 0f);
		public static IVector3 Right => new CoreVector3(1f, 0f, 0f);
		public static IVector3 Forward => new CoreVector3(0f, 0f, 1f);
		public static IVector3 Back => new CoreVector3(0f, 0f, -1f);

		public override string ToString()
			=> $"({X:F1}, {Y:F1}, {Z:F1})";
	}
}
