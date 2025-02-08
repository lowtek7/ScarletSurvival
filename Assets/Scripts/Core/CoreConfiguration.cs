using Scarlet.Core.Event;
using Scarlet.Core.Logging;
using Scarlet.Core.Logging.Enums;

namespace Scarlet.Core
{
	public class CoreConfiguration
	{
		public LogConfiguration LogConfig { get; set; } = new()
		{
			// 로그 레벨 설정
			MinLogLevel = LogLevel.Info,

			// 큐 설정
			MaxQueueSize = 4096,
			BatchSize = 1000,

			// 성능 설정
			EnableAsyncLogging = true,
			AsyncFlushInterval = 50, // 밀리초

			// 포맷 설정
			IncludeTimestamp = true,
			IncludeLogLevel = true,
			TimestampFormat = "yyyy-MM-dd HH:mm:ss.fff",

			// 스택트레이스 설정
			IncludeStackTrace = false,
			IncludeStackTraceForError = true,

			// 파일 로깅 설정 (필요한 경우)
			LogFilePath = "logs/core.log",

			// 기본 필터 설정 (필요한 경우)
			CustomFilter = null // message => true
		};

		public EventBusConfiguration EventBusConfig { get; set; } = new()
		{
			BufferSize = 4096,
			MaxHandlersPerEvent = 32,
			SpinWaitMaxCount = 10,
			BufferFullPolicy = Event.Enums.BufferFullPolicy.DropEvent
		};

		// 로깅 기본 설정
		// 로그 레벨 설정
		// 큐 설정
		// 성능 설정
		// 밀리초
		// 포맷 설정
		// 스택트레이스 설정
		// 파일 로깅 설정 (필요한 경우)
		//LogFilePath = "logs/core.log",
		// 기본 필터 설정 (필요한 경우)
		// message => true
		// 이벤트 버스 기본 설정

		public static CoreConfiguration CreateDefault()
		{
			return new CoreConfiguration();
		}

		public static CoreConfiguration CreateDebug()
		{
			var config = new CoreConfiguration();

			// 디버그 환경에 맞는 로깅 설정
			config.LogConfig.MinLogLevel = LogLevel.Debug;
			config.LogConfig.IncludeStackTrace = true;
			config.LogConfig.BatchSize = 100; // 더 빠른 로그 출력을 위해 배치 크기 감소
			config.LogConfig.AsyncFlushInterval = 10; // 더 빠른 로그 플러시

			return config;
		}

		public static CoreConfiguration CreateRelease()
		{
			var config = new CoreConfiguration();

			// 릴리스 환경에 맞는 로깅 설정
			config.LogConfig.MinLogLevel = LogLevel.Info;
			config.LogConfig.IncludeStackTrace = false;
			config.LogConfig.BatchSize = 1000; // 성능 최적화를 위해 배치 크기 증가
			config.LogConfig.AsyncFlushInterval = 100; // 리소스 사용 최적화

			return config;
		}

		public static CoreConfiguration CreateMinimal()
		{
			var config = new CoreConfiguration();

			// 최소한의 로깅만 수행하는 설정
			config.LogConfig.MinLogLevel = LogLevel.Warning;
			config.LogConfig.IncludeTimestamp = false;
			config.LogConfig.IncludeStackTrace = false;
			config.LogConfig.IncludeStackTraceForError = false;
			config.LogConfig.BatchSize = 2000; // 큰 배치 크기로 오버헤드 최소화
			config.LogConfig.AsyncFlushInterval = 200; // 긴 플러시 간격

			return config;
		}

		public CoreConfiguration Clone()
		{
			return new CoreConfiguration
			{
				LogConfig = new LogConfiguration
				{
					MinLogLevel = this.LogConfig.MinLogLevel,
					MaxQueueSize = this.LogConfig.MaxQueueSize,
					BatchSize = this.LogConfig.BatchSize,
					EnableAsyncLogging = this.LogConfig.EnableAsyncLogging,
					AsyncFlushInterval = this.LogConfig.AsyncFlushInterval,
					IncludeTimestamp = this.LogConfig.IncludeTimestamp,
					IncludeLogLevel = this.LogConfig.IncludeLogLevel,
					TimestampFormat = this.LogConfig.TimestampFormat,
					IncludeStackTrace = this.LogConfig.IncludeStackTrace,
					IncludeStackTraceForError = this.LogConfig.IncludeStackTraceForError,
					LogFilePath = this.LogConfig.LogFilePath,
					CustomFilter = this.LogConfig.CustomFilter
				},
				EventBusConfig = new EventBusConfiguration
				{
					BufferSize = this.EventBusConfig.BufferSize,
					MaxHandlersPerEvent = this.EventBusConfig.MaxHandlersPerEvent,
					SpinWaitMaxCount = this.EventBusConfig.SpinWaitMaxCount,
					BufferFullPolicy = this.EventBusConfig.BufferFullPolicy
				}
			};
		}
	}
}
