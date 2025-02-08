using System.Text;
using Scarlet.Core.Logging.Interfaces;

namespace Scarlet.Core.Logging.Appenders
{
	public abstract class LogAppenderBase : ILogAppender
	{
		protected readonly LogConfiguration Config;
		protected readonly StringBuilder StringBuilder;

		protected LogAppenderBase(LogConfiguration config)
		{
			Config = config;
			StringBuilder = new StringBuilder(8192);
		}

		public abstract void Append(LogMessage message);
		public abstract void Flush();

		protected virtual void FormatMessage(LogMessage message, StringBuilder builder)
		{
			builder.Clear();

			if (Config.IncludeTimestamp)
			{
				builder.Append(message.Timestamp.ToString(Config.TimestampFormat));
				builder.Append(' ');
			}

			if (Config.IncludeLogLevel)
			{
				builder.Append('[');
				builder.Append(message.Level);
				builder.Append("] ");
			}

			builder.AppendLine(message.Message);
		}
	}
}
