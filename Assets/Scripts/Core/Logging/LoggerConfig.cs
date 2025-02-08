using Scarlet.Core.Logging.Enums;

namespace Scarlet.Core.Logging
{
	public class LoggerConfig
	{
		public LogLevel MinimumLevel { get; set; }
		public bool IncludeTimestamp { get; set; }
		public string LogFilePath { get; set; }
	}
}
