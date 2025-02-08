using Scarlet.Core.Math.Scarlet.Core.Math;

namespace Scarlet.Core.Math.Extensions
{
	public static class MathExtensions
	{
		// 부동소수점 비교를 위한 확장 메서드들
		public static bool ApproximatelyEquals(this float value, float other, float epsilon = CoreMathf.NormalEpsilon)
		{
			return CoreMathf.Approximately(value, other, epsilon);
		}

		public static bool IsZero(this float value, float epsilon = CoreMathf.NormalEpsilon)
		{
			return CoreMathf.IsZero(value, epsilon);
		}

		public static bool IsLessThan(this float value, float other, float epsilon = CoreMathf.NormalEpsilon)
		{
			return CoreMathf.IsLessThan(value, other, epsilon);
		}

		public static bool IsGreaterThan(this float value, float other, float epsilon = CoreMathf.NormalEpsilon)
		{
			return CoreMathf.IsGreaterThan(value, other, epsilon);
		}

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

		public static bool IsInRange(this float value, float min, float max, float epsilon = CoreMathf.NormalEpsilon)
		{
			return CoreMathf.IsInRange(value, min, max, epsilon);
		}

		public static bool IsInRange(this int value, int min, int max)
		{
			return value >= min && value <= max;
		}
	}
}
