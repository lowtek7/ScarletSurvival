namespace Scarlet.Core.Event
{
	/// <summary>
	/// 이벤트 우선순위 상수
	/// </summary>
	public static class EventPriority
	{
		public const int Highest = int.MinValue;
		public const int High = -1000;
		public const int Normal = 0;
		public const int Low = 1000;
		public const int Lowest = int.MaxValue;
	}
}
