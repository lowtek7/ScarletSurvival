namespace Scarlet.Core.Async.Enums
{
	public enum AsyncOperationStatus : byte
	{
		None = 0,
		Running = 1,
		Completed = 2,
		Failed = 3,
		Canceled = 4,
		Disposed = 5
	}
}
