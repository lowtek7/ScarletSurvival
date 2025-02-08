using System;

namespace Scarlet.Core.Math.Adapters
{
	/// <summary>
	/// 플랫폼 독립적인 수학 연산을 제공하는 인터페이스입니다.
	/// 모든 수학 관련 연산은 이 인터페이스를 통해 제공됩니다.
	/// </summary>
	public interface IMathProvider
	{
		#region Constants

		/// <summary>
		/// 원주율 (π)
		/// </summary>
		float PI { get; }

		/// <summary>
		/// 작은 수의 비교에 사용되는 엡실론 값
		/// </summary>
		float Epsilon { get; }

		/// <summary>
		/// 도(degree)를 라디안으로 변환하는 승수
		/// </summary>
		float Deg2Rad { get; }

		/// <summary>
		/// 라디안을 도(degree)로 변환하는 승수
		/// </summary>
		float Rad2Deg { get; }

		#endregion

		#region Basic Operations

		float Abs(float value);
		int Abs(int value);
		float Sign(float value);
		float Clamp(float value, float min, float max);
		float Clamp01(float value);

		#endregion

		#region Trigonometry

		float Sin(float f);
		float Cos(float f);
		float Tan(float f);
		float Asin(float f);
		float Acos(float f);
		float Atan(float f);
		float Atan2(float y, float x);

		#endregion

		#region Powers and Roots

		float Pow(float f, float p);
		float Sqrt(float f);

		#endregion

		#region Rounding

		float Round(float f);
		float Ceil(float f);
		float Floor(float f);
		int RoundToInt(float f);
		int CeilToInt(float f);
		int FloorToInt(float f);

		#endregion

		#region Interpolation

		float Lerp(float a, float b, float t);
		float LerpUnclamped(float a, float b, float t);
		float InverseLerp(float a, float b, float value);
		float SmoothStep(float from, float to, float t);

		#endregion

		#region Angles

		float DeltaAngle(float current, float target);
		float MoveTowardsAngle(float current, float target, float maxDelta);

		#endregion

		#region Min/Max

		float Min(float a, float b);
		float Max(float a, float b);
		float Min(params float[] values);
		float Max(params float[] values);

		#endregion

		#region Approximately

		bool Approximately(float a, float b);

		#endregion
	}

	/// <summary>
	/// IMathProvider의 기본 구현체입니다.
	/// 순수 C# 환경에서 사용할 수 있는 수학 연산을 제공합니다.
	/// </summary>
	public class CoreMathProvider : IMathProvider
	{
		#region Constants

		public float PI => 3.14159265359f;
		public float Epsilon => 1e-5f;
		public float Deg2Rad => PI * 2f / 360f;
		public float Rad2Deg => 360f / (PI * 2f);

		#endregion

		#region Basic Operations

		public float Abs(float value) => value < 0f ? -value : value;
		public int Abs(int value) => value < 0 ? -value : value;

		public float Sign(float value) => value >= 0f ? 1f : -1f;

		public float Clamp(float value, float min, float max)
		{
			if (value < min) return min;
			if (value > max) return max;
			return value;
		}

		public float Clamp01(float value) => Clamp(value, 0f, 1f);

		#endregion

		#region Trigonometry

		public float Sin(float f) => (float)System.Math.Sin(f);
		public float Cos(float f) => (float)System.Math.Cos(f);
		public float Tan(float f) => (float)System.Math.Tan(f);
		public float Asin(float f) => (float)System.Math.Asin(f);
		public float Acos(float f) => (float)System.Math.Acos(f);
		public float Atan(float f) => (float)System.Math.Atan(f);
		public float Atan2(float y, float x) => (float)System.Math.Atan2(y, x);

		#endregion

		#region Powers and Roots

		public float Pow(float f, float p) => (float)System.Math.Pow(f, p);
		public float Sqrt(float f) => (float)System.Math.Sqrt(f);

		#endregion

		#region Rounding

		public float Round(float f) => (float)System.Math.Round(f);
		public float Ceil(float f) => (float)System.Math.Ceiling(f);
		public float Floor(float f) => (float)System.Math.Floor(f);
		public int RoundToInt(float f) => (int)System.Math.Round(f);
		public int CeilToInt(float f) => (int)System.Math.Ceiling(f);
		public int FloorToInt(float f) => (int)System.Math.Floor(f);

		#endregion

		#region Interpolation

		public float Lerp(float a, float b, float t)
		{
			t = Clamp01(t);
			return a + (b - a) * t;
		}

		public float LerpUnclamped(float a, float b, float t)
		{
			return a + (b - a) * t;
		}

		public float InverseLerp(float a, float b, float value)
		{
			if (Approximately(a, b))
				return 0f;

			return Clamp01((value - a) / (b - a));
		}

		public float SmoothStep(float from, float to, float t)
		{
			t = Clamp01(t);
			t = t * t * (3f - 2f * t);
			return to * t + from * (1f - t);
		}

		#endregion

		#region Angles

		public float DeltaAngle(float current, float target)
		{
			float delta = Repeat(target - current, 360f);
			if (delta > 180f)
				delta -= 360f;
			return delta;
		}

		public float MoveTowardsAngle(float current, float target, float maxDelta)
		{
			float deltaAngle = DeltaAngle(current, target);
			if (-maxDelta < deltaAngle && deltaAngle < maxDelta)
				return target;
			target = current + deltaAngle;
			return MoveTowards(current, target, maxDelta);
		}

		private float MoveTowards(float current, float target, float maxDelta)
		{
			if (Abs(target - current) <= maxDelta)
				return target;
			return current + Sign(target - current) * maxDelta;
		}

		private float Repeat(float t, float length)
		{
			return Clamp(t - Floor(t / length) * length, 0f, length);
		}

		#endregion

		#region Min/Max

		public float Min(float a, float b) => a < b ? a : b;

		public float Max(float a, float b) => a > b ? a : b;

		public float Min(params float[] values)
		{
			if (values == null || values.Length == 0)
				throw new ArgumentException("At least one value must be provided");

			float min = values[0];
			for (int i = 1; i < values.Length; i++)
			{
				if (values[i] < min)
					min = values[i];
			}

			return min;
		}

		public float Max(params float[] values)
		{
			if (values == null || values.Length == 0)
				throw new ArgumentException("At least one value must be provided");

			float max = values[0];
			for (int i = 1; i < values.Length; i++)
			{
				if (values[i] > max)
					max = values[i];
			}

			return max;
		}

		#endregion

		#region Approximately

		public bool Approximately(float a, float b)
		{
			return Abs(b - a) < Max(1E-06f * Max(Abs(a), Abs(b)), Epsilon * 8f);
		}

		#endregion
	}
}
