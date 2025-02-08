using System;
using Scarlet.Core.Logging.Enums;

namespace Scarlet.Core.Logging
{
	public readonly struct LogMessage
	{
		public LogLevel Level { get; }
		public string Message { get; }
		public DateTime Timestamp { get; }

		public LogMessage(LogLevel level, string message, DateTime timestamp)
		{
			Level = level;
			Message = message;
			Timestamp = timestamp;
		}
	}
}
