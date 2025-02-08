namespace Scarlet.Core.Math
{
	namespace Scarlet.Core.Math
	{
		/// <summary>
		/// 플랫폼 독립적인 수학 유틸리티 클래스
		/// </summary>
		public static class CoreMathf
		{
			#region Constants

			public const float PI = 3.14159265359f;
			public const float Epsilon = 1e-5f;  // 일반적인 비교를 위한 epsilon
			public const float VerySmallEpsilon = 1e-7f;  // 더 정밀한 비교가 필요할 때
			public const float NormalEpsilon = 1e-3f;  // 일반적인 게임 로직에서 사용
			public const float Deg2Rad = PI * 2f / 360f;
			public const float Rad2Deg = 360f / (PI * 2f);
			public const float Infinity = float.PositiveInfinity;
			public const float NegativeInfinity = float.NegativeInfinity;

			#endregion

			#region Float Comparison

			/// <summary>
			/// 두 부동소수점 값이 거의 같은지 비교합니다.
			/// </summary>
			/// <param name="a">첫 번째 값</param>
			/// <param name="b">두 번째 값</param>
			/// <param name="epsilon">허용 오차 (기본값: NormalEpsilon)</param>
			public static bool Approximately(float a, float b, float epsilon = NormalEpsilon)
			{
				return Abs(b - a) < epsilon;
			}

			/// <summary>
			/// 부동소수점 값이 거의 0인지 확인합니다.
			/// </summary>
			public static bool IsZero(float value, float epsilon = NormalEpsilon)
			{
				return Abs(value) < epsilon;
			}

			/// <summary>
			/// 첫 번째 값이 두 번째 값보다 거의 작은지 비교합니다.
			/// </summary>
			public static bool IsLessThan(float a, float b, float epsilon = NormalEpsilon)
			{
				return a < b && !Approximately(a, b, epsilon);
			}

			/// <summary>
			/// 첫 번째 값이 두 번째 값보다 거의 큰지 비교합니다.
			/// </summary>
			public static bool IsGreaterThan(float a, float b, float epsilon = NormalEpsilon)
			{
				return a > b && !Approximately(a, b, epsilon);
			}

			/// <summary>
			/// 첫 번째 값이 두 번째 값보다 작거나 거의 같은지 비교합니다.
			/// </summary>
			public static bool IsLessThanOrApproximately(float a, float b, float epsilon = NormalEpsilon)
			{
				return a < b || Approximately(a, b, epsilon);
			}

			/// <summary>
			/// 첫 번째 값이 두 번째 값보다 크거나 거의 같은지 비교합니다.
			/// </summary>
			public static bool IsGreaterThanOrApproximately(float a, float b, float epsilon = NormalEpsilon)
			{
				return a > b || Approximately(a, b, epsilon);
			}

			/// <summary>
			/// 값이 범위 내에 있는지 확인합니다 (epsilon 값 적용).
			/// </summary>
			public static bool IsInRange(float value, float min, float max, float epsilon = NormalEpsilon)
			{
				return IsGreaterThanOrApproximately(value, min, epsilon) &&
					   IsLessThanOrApproximately(value, max, epsilon);
			}

			#endregion

			#region Basic Operations

			public static float Abs(float value) => value < 0f ? -value : value;

			public static int Abs(int value) => value < 0 ? -value : value;

			public static float Sign(float value) => value >= 0f ? 1f : -1f;

			public static int Sign(int value) => value >= 0 ? 1 : -1;

			public static float Clamp(float value, float min, float max)
			{
				if (value < min) return min;
				if (value > max) return max;
				return value;
			}

			public static int Clamp(int value, int min, int max)
			{
				if (value < min) return min;
				if (value > max) return max;
				return value;
			}

			public static float Clamp01(float value) => Clamp(value, 0f, 1f);

			#endregion

			#region Trigonometry

			public static float Sin(float f) => (float)System.Math.Sin(f);
			public static float Cos(float f) => (float)System.Math.Cos(f);
			public static float Tan(float f) => (float)System.Math.Tan(f);

			public static float Asin(float f) => (float)System.Math.Asin(f);
			public static float Acos(float f) => (float)System.Math.Acos(f);
			public static float Atan(float f) => (float)System.Math.Atan(f);
			public static float Atan2(float y, float x) => (float)System.Math.Atan2(y, x);

			#endregion

			#region Power and Root

			public static float Pow(float f, float p) => (float)System.Math.Pow(f, p);
			public static float Sqrt(float f) => (float)System.Math.Sqrt(f);
			public static float Cbrt(float f) => (float)System.Math.Cbrt(f);

			#endregion

			#region Rounding

			public static float Ceil(float f) => (float)System.Math.Ceiling(f);
			public static float Floor(float f) => (float)System.Math.Floor(f);
			public static float Round(float f) => (float)System.Math.Round(f);
			public static int CeilToInt(float f) => (int)System.Math.Ceiling(f);
			public static int FloorToInt(float f) => (int)System.Math.Floor(f);
			public static int RoundToInt(float f) => (int)System.Math.Round(f);

			#endregion

			#region Interpolation

			public static float Lerp(float a, float b, float t)
			{
				t = Clamp01(t);
				return a + (b - a) * t;
			}

			public static float LerpUnclamped(float a, float b, float t)
			{
				return a + (b - a) * t;
			}

			public static float InverseLerp(float a, float b, float value)
			{
				if (a != b)
					return Clamp01((value - a) / (b - a));
				return 0f;
			}

			public static float SmoothStep(float from, float to, float t)
			{
				t = Clamp01(t);
				t = t * t * (3f - 2f * t);
				return to * t + from * (1f - t);
			}

			#endregion

			#region Angles

			public static float DeltaAngle(float current, float target)
			{
				float delta = Repeat((target - current), 360f);
				if (delta > 180f)
					delta -= 360f;
				return delta;
			}

			public static float Repeat(float t, float length)
			{
				return Clamp(t - Floor(t / length) * length, 0f, length);
			}

			public static float MoveTowardsAngle(float current, float target, float maxDelta)
			{
				float deltaAngle = DeltaAngle(current, target);
				if (-maxDelta < deltaAngle && deltaAngle < maxDelta)
					return target;
				target = current + deltaAngle;
				return MoveTowards(current, target, maxDelta);
			}

			#endregion

			#region Movement

			public static float MoveTowards(float current, float target, float maxDelta)
			{
				if (Abs(target - current) <= maxDelta)
					return target;
				return current + Sign(target - current) * maxDelta;
			}

			public static float PingPong(float t, float length)
			{
				t = Repeat(t, length * 2f);
				return length - Abs(t - length);
			}

			#endregion

			#region Min/Max

			public static float Min(float a, float b) => a < b ? a : b;

			public static float Min(params float[] values)
			{
				int len = values.Length;
				if (len == 0) return 0f;
				float m = values[0];
				for (int i = 1; i < len; i++)
				{
					if (values[i] < m) m = values[i];
				}

				return m;
			}

			public static float Max(float a, float b) => a > b ? a : b;

			public static float Max(params float[] values)
			{
				int len = values.Length;
				if (len == 0) return 0f;
				float m = values[0];
				for (int i = 1; i < len; i++)
				{
					if (values[i] > m) m = values[i];
				}

				return m;
			}

			#endregion

			#region Conversion

			/// <summary>
			/// 라디안을 도(degree)로 변환
			/// </summary>
			public static float RadToDeg(float rad) => rad * Rad2Deg;

			/// <summary>
			/// 도(degree)를 라디안으로 변환
			/// </summary>
			public static float DegToRad(float deg) => deg * Deg2Rad;

			#endregion
		}

		// Core/Math/Extensions/MathExtensions.cs
		public static class MathExtensions
		{
			public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
			{
				float normalized = (value - fromMin) / (fromMax - fromMin);
				return CoreMathf.Lerp(toMin, toMax, normalized);
			}

			public static float RemapClamped(this float value, float fromMin, float fromMax, float toMin, float toMax)
			{
				float normalized = CoreMathf.InverseLerp(fromMin, fromMax, value);
				return CoreMathf.Lerp(toMin, toMax, normalized);
			}

			public static bool IsInRange(this float value, float min, float max)
			{
				return value >= min && value <= max;
			}

			public static bool IsInRange(this int value, int min, int max)
			{
				return value >= min && value <= max;
			}
		}
	}
}
