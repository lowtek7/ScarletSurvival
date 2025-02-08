using System;
using Scarlet.Core.Math.Interfaces;

namespace Scarlet.Core.Math
{
	public struct CoreVector2 : IVector2
	{
		public float X { get; set; }
		public float Y { get; set; }

		public float Magnitude => MathF.Sqrt(SqrMagnitude);
		public float SqrMagnitude => X * X + Y * Y;

		public IVector Normalized
		{
			get
			{
				float mag = Magnitude;
				if (mag > 1E-05f)
					return new CoreVector2(X / mag, Y / mag);
				return new CoreVector2();
			}
		}

		public CoreVector2(float x, float y)
		{
			X = x;
			Y = y;
		}

		public void Normalize()
		{
			float mag = Magnitude;
			if (mag > 1E-05f)
			{
				X /= mag;
				Y /= mag;
			}
			else
			{
				X = 0;
				Y = 0;
			}
		}

		// 연산자 오버로딩
		public static CoreVector2 operator +(CoreVector2 a, CoreVector2 b)
			=> new CoreVector2(a.X + b.X, a.Y + b.Y);

		public static CoreVector2 operator -(CoreVector2 a, CoreVector2 b)
			=> new CoreVector2(a.X - b.X, a.Y - b.Y);

		public static CoreVector2 operator *(CoreVector2 a, float d)
			=> new CoreVector2(a.X * d, a.Y * d);

		public static CoreVector2 operator /(CoreVector2 a, float d)
			=> new CoreVector2(a.X / d, a.Y / d);

		public static CoreVector2 operator -(CoreVector2 a)
			=> new CoreVector2(-a.X, -a.Y);

		// 상수 벡터들
		public static IVector2 Zero => new CoreVector2(0f, 0f);
		public static IVector2 One => new CoreVector2(1f, 1f);
		public static IVector2 Up => new CoreVector2(0f, 1f);
		public static IVector2 Down => new CoreVector2(0f, -1f);
		public static IVector2 Left => new CoreVector2(-1f, 0f);
		public static IVector2 Right => new CoreVector2(1f, 0f);

		// ToString 오버라이드
		public override string ToString()
			=> $"({X:F1}, {Y:F1})";
	}
}
