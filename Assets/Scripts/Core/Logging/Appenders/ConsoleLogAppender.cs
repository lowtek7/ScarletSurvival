using System;
using Scarlet.Core.Logging.Enums;
using Scarlet.Core.Logging.Interfaces;

namespace Scarlet.Core.Logging.Appenders
{
	public class ConsoleLogAppender : LogAppenderBase
	{
		private readonly object consoleLock = new();

		public ConsoleLogAppender(LogConfiguration config) : base(config)
		{
		}

		public override void Append(LogMessage message)
		{
			lock (consoleLock)
			{
				FormatMessage(message, StringBuilder);
				// 로그 레벨에 따른 콘솔 색상 설정
				Console.ForegroundColor = GetColorForLogLevel(message.Level);
				Console.Write(StringBuilder.ToString());
				Console.ResetColor();
			}
		}

		public override void Flush()
		{
			// Console은 별도의 Flush 필요 없음
		}

		private static ConsoleColor GetColorForLogLevel(LogLevel level)
		{
			return level switch
			{
				LogLevel.Debug => ConsoleColor.Gray,
				LogLevel.Info => ConsoleColor.White,
				LogLevel.Warning => ConsoleColor.Yellow,
				LogLevel.Error => ConsoleColor.Red,
				LogLevel.Fatal => ConsoleColor.DarkRed,
				_ => ConsoleColor.White
			};
		}
	}
}
