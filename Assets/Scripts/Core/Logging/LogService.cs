using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Scarlet.Core.Async;
using Scarlet.Core.Collection;
using Scarlet.Core.Logging.Appenders;
using Scarlet.Core.Logging.Enums;
using Scarlet.Core.Logging.Interfaces;
using Scarlet.Core.Math.Scarlet.Core.Math;
using Scarlet.Core.Services;

namespace Scarlet.Core.Logging
{
	public class LogService : ServiceBase, ILogger
	{
		private const int BUFFER_SIZE = 8192;
		private const int BATCH_SIZE = 1000;
		private readonly LogConfiguration _config;
		private readonly ILogAppender[] _appenders;
		private readonly CancellationTokenSource _cts;

		// 이중 버퍼 시스템
		private readonly RingBuffer<LogMessage>[] _buffers;
		private readonly AtomicCounter _activeBufferIndex;
		private readonly ManualResetEventSlim _flushEvent;
		private readonly AtomicCounter _messageCount;

		private AsyncOperation<bool> _currentBatchOperation;
		private volatile bool _isProcessing;

		public LogService(CoreSandbox core, LogConfiguration config, params ILogAppender[] appenders)
			: base(core)
		{
			_config = config;
			_appenders = appenders ?? Array.Empty<ILogAppender>();
			_cts = new CancellationTokenSource();

			// 이중 버퍼 초기화
			_buffers = new[]
			{
				new RingBuffer<LogMessage>(BUFFER_SIZE),
				new RingBuffer<LogMessage>(BUFFER_SIZE)
			};
			_activeBufferIndex = new AtomicCounter(0);
			_flushEvent = new ManualResetEventSlim(false);
			_messageCount = new AtomicCounter(0);

			if (_config.EnableAsyncLogging)
			{
				StartAsyncProcessing();
			}
		}

		// 고성능 카운터 구현
		private class AtomicCounter
		{
			private int value;

			public AtomicCounter(int initialValue = 0)
			{
				value = initialValue;
			}

			public int Increment()
			{
				return Interlocked.Increment(ref value);
			}

			public int Decrement()
			{
				return Interlocked.Decrement(ref value);
			}

			public void Add(int value)
			{
				Interlocked.Add(ref this.value, value);
			}

			public int Value
			{
				get => Interlocked.CompareExchange(ref value, 0, 0);
				set => Interlocked.Exchange(ref this.value, value);
			}
		}

		private void StartAsyncProcessing()
		{
			if (_isProcessing)
				return;

			_isProcessing = true;
			ProcessLogsAsync();
		}

		private void ProcessLogsAsync()
		{
			if (_currentBatchOperation != null && !_currentBatchOperation.IsCompleted)
				return;

			_currentBatchOperation = new AsyncOperation<bool>();

			try
			{
				while (!_cts.Token.IsCancellationRequested)
				{
					// 배치 크기에 도달하거나 타임아웃 발생 시까지 대기
					_flushEvent.Wait(_config.AsyncFlushInterval);
					_flushEvent.Reset();

					SwapAndProcessBuffers();
				}

				_currentBatchOperation.SetResult(true);
			}
			catch (Exception ex)
			{
				_currentBatchOperation.SetException(ex);
			}
		}

		private void SwapAndProcessBuffers()
		{
			// 버퍼 스왑
			var oldIndex = _activeBufferIndex.Value & 1;
			_activeBufferIndex.Increment();
			var processingBuffer = _buffers[oldIndex];

			// 메시지 처리
			var processedCount = 0;
			while (processingBuffer.TryDequeue(out var message))
			{
				foreach (var appender in _appenders)
				{
					try
					{
						appender.Append(message);
					}
					catch (Exception ex)
					{
						// 에러 로깅 (최소화된 버전)
						Console.Error.WriteLine(ex.Message);
					}
				}

				processedCount++;
			}

			// 카운터 안전하게 조정
			_messageCount.Add(-processedCount);

			if (_currentBatchOperation != null)
			{
				_currentBatchOperation.SetProgress(1.0f);
			}
		}

		public void Log(LogLevel level, string message)
		{
			if (level < _config.MinLogLevel)
				return;

			var logMessage = new LogMessage(level, message, DateTime.UtcNow);

			if (_config.CustomFilter?.Invoke(logMessage) == false)
				return;

			// 현재 활성 버퍼에 로그 추가
			var currentBuffer = _buffers[_activeBufferIndex.Value & 1];
			if (currentBuffer.TryEnqueue(logMessage))
			{
				if (_messageCount.Increment() >= BATCH_SIZE)
				{
					_flushEvent.Set();
				}
			}
		}

		public void LogTrace(string message)
		{
			Log(LogLevel.Trace, message);
		}

		public void LogDebug(string message)
		{
			Log(LogLevel.Debug, message);
		}

		public void LogInfo(string message)
		{
			Log(LogLevel.Info, message);
		}

		public void LogWarning(string message)
		{
			Log(LogLevel.Warning, message);
		}

		public void LogError(string message)
		{
			Log(LogLevel.Error, message);
		}

		public void LogFatal(string message)
		{
			Log(LogLevel.Fatal, message);
		}

		public void LogException(Exception exception, string context = null)
		{
			if (exception == null)
				return;

			var builder = StringBuilderPool.Rent();
			try
			{
				FormatException(builder, exception, context);
				Log(LogLevel.Error, builder.ToString());
			}
			finally
			{
				StringBuilderPool.Return(builder);
			}
		}

		private void FormatException(StringBuilder builder, Exception exception, string context, int depth = 0)
		{
			const int maxDepth = 10; // 재귀적 예외 처리의 최대 깊이
			if (depth >= maxDepth) return;

			if (depth > 0)
			{
				builder.AppendLine()
					.Append(' ', depth * 2)
					.Append("Inner Exception (")
					.Append(depth)
					.Append("): ");
			}

			builder.Append(exception.GetType().Name)
				.Append(": ")
				.Append(exception.Message);

			if (!string.IsNullOrEmpty(context) && depth == 0)
			{
				builder.Append(" | Context: ")
					.Append(context);
			}

			if (_config.IncludeStackTraceForError && exception.StackTrace != null)
			{
				builder.AppendLine()
					.Append(' ', depth * 2)
					.Append("StackTrace: ")
					.AppendLine()
					.Append(exception.StackTrace);
			}

			// 재귀적으로 내부 예외 처리
			if (exception.InnerException != null)
			{
				FormatException(builder, exception.InnerException, null, depth + 1);
			}
		}

		// 형식화된 문자열을 사용하는 로그 메서드들
		public void LogTrace(string format, params object[] args)
		{
			if (LogLevel.Trace < _config.MinLogLevel) return;
			Log(LogLevel.Trace, string.Format(format, args));
		}

		public void LogDebug(string format, params object[] args)
		{
			if (LogLevel.Debug < _config.MinLogLevel) return;
			Log(LogLevel.Debug, string.Format(format, args));
		}

		public void LogInfo(string format, params object[] args)
		{
			if (LogLevel.Info < _config.MinLogLevel) return;
			Log(LogLevel.Info, string.Format(format, args));
		}

		public void LogWarning(string format, params object[] args)
		{
			if (LogLevel.Warning < _config.MinLogLevel) return;
			Log(LogLevel.Warning, string.Format(format, args));
		}

		public void LogError(string format, params object[] args)
		{
			if (LogLevel.Error < _config.MinLogLevel) return;
			Log(LogLevel.Error, string.Format(format, args));
		}

		public void LogFatal(string format, params object[] args)
		{
			if (LogLevel.Fatal < _config.MinLogLevel) return;
			Log(LogLevel.Fatal, string.Format(format, args));
		}

		// 추가적인 편의 메서드
		public void LogErrorWithException(string message, Exception exception)
		{
			if (LogLevel.Error < _config.MinLogLevel) return;

			var builder = StringBuilderPool.Rent();
			try
			{
				builder.Append(message);
				if (exception != null)
				{
					builder.AppendLine();
					FormatException(builder, exception, null);
				}

				Log(LogLevel.Error, builder.ToString());
			}
			finally
			{
				StringBuilderPool.Return(builder);
			}
		}

		// StringBuilder 풀 구현
		private static class StringBuilderPool
		{
			private static readonly ConcurrentQueue<StringBuilder> _pool = new();
			private const int INITIAL_CAPACITY = 1024;
			private const int MAX_POOL_SIZE = 32;

			public static StringBuilder Rent()
			{
				return _pool.TryDequeue(out var sb) ? sb.Clear() : new StringBuilder(INITIAL_CAPACITY);
			}

			public static void Return(StringBuilder sb)
			{
				if (sb == null || sb.Capacity > INITIAL_CAPACITY * 2 || _pool.Count >= MAX_POOL_SIZE)
					return;

				_pool.Enqueue(sb);
			}
		}

		public override void Cleanup()
		{
			_cts.Cancel();
			_isProcessing = false;

			// 남은 로그 처리
			SwapAndProcessBuffers();

			foreach (var appender in _appenders)
			{
				try
				{
					appender.Flush();
				}
				catch (Exception ex)
				{
					Console.Error.WriteLine(ex.Message);
				}
			}

			_flushEvent.Dispose();
			_cts.Dispose();
			base.Cleanup();
		}
	}
}
