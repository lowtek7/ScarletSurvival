namespace Scarlet.Core.Logging.Interfaces
{
	public interface ILogAppender
	{
		void Append(LogMessage message);
		void Flush();
	}
}
