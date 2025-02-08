using System;
using Scarlet.Core.Logging.Enums;

namespace Scarlet.Core.Logging
{
	public class LogConfiguration
	{
		// 로그 레벨 설정
		public LogLevel MinLogLevel { get; set; } = LogLevel.Info;

		// 큐 설정
		public int MaxQueueSize { get; set; } = 1000;
		public int BatchSize { get; set; } = 100;

		// 성능 설정
		public bool EnableAsyncLogging { get; set; } = true;
		public int AsyncFlushInterval { get; set; } = 1000; // 밀리초

		// 포맷 설정
		public bool IncludeTimestamp { get; set; } = true;
		public bool IncludeLogLevel { get; set; } = true;
		public string TimestampFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";

		// 스택트레이스 설정
		public bool IncludeStackTrace { get; set; } = false;
		public bool IncludeStackTraceForError { get; set; } = true;

		// 필터 설정
		public Func<LogMessage, bool> CustomFilter { get; set; }

		public string LogFilePath { get; set; }
	}
}
