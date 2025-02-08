using System;
using Scarlet.Core.Logging.Enums;
using Scarlet.Core.Services.Interfaces;

namespace Scarlet.Core.Logging.Interfaces
{
	public interface ILogger : IService
	{
		void Log(LogLevel level, string message);
		void LogTrace(string message);
		void LogDebug(string message);
		void LogInfo(string message);
		void LogWarning(string message);
		void LogError(string message);
		void LogFatal(string message);

		// 형식화된 문자열 버전
		void LogTrace(string format, params object[] args);
		void LogDebug(string format, params object[] args);
		void LogInfo(string format, params object[] args);
		void LogWarning(string format, params object[] args);
		void LogError(string format, params object[] args);
		void LogFatal(string format, params object[] args);

		// 예외 처리
		void LogException(Exception exception, string context = null);
		void LogErrorWithException(string message, Exception exception);
	}
}
