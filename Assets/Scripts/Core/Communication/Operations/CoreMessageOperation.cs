using System;
using System.Threading;
using Scarlet.Core.Async;
using Scarlet.Core.Async.Enums;
using Scarlet.Core.Communication.Interfaces;
using Scarlet.Core.Logging.Interfaces;

namespace Scarlet.Core.Communication.Operations
{
	public class CoreMessageOperation : AsyncOperationBase<bool>
	{
		private readonly ICoreMessage _message;
		private readonly CoreId _targetId;
		private readonly ILogger _logger;
		private readonly Action<CoreId, ICoreMessage> _sendAction;
		private readonly int _maxRetries;
		private readonly int _retryDelayMilliseconds;
		private int _currentRetry; // volatile 제거
		private int _isRetrying; // bool을 int로 변경
		private readonly CancellationTokenSource _cts;

		public CoreMessageOperation(
			ICoreMessage message,
			CoreId targetId,
			ILogger logger,
			Action<CoreId, ICoreMessage> sendAction,
			int maxRetries = 3,
			int retryDelayMilliseconds = 100)
		{
			_message = message ?? throw new ArgumentNullException(nameof(message));
			_targetId = targetId;
			_logger = logger;
			_sendAction = sendAction ?? throw new ArgumentNullException(nameof(sendAction));
			_maxRetries = maxRetries;
			_retryDelayMilliseconds = retryDelayMilliseconds;
			_currentRetry = 0;
			_isRetrying = 0; // 0 = false, 1 = true
			_cts = new CancellationTokenSource();

			ExecuteAsync();
		}

		private void ExecuteAsync()
		{
			try
			{
				Progress = 0f;
				ExecuteWithRetry();
			}
			catch (Exception ex)
			{
				_logger?.LogError($"Message operation failed: {ex.Message}");
				HandleError(ex);
			}
		}

		private void ExecuteWithRetry()
		{
			do
			{
				try
				{
					_sendAction(_targetId, _message);
					Progress = 1f;
					HandleSuccess();
					return;
				}
				catch (Exception ex)
				{
					int currentRetry = Interlocked.Increment(ref _currentRetry);
					bool shouldRetry = currentRetry < _maxRetries;

					// isRetrying 상태 업데이트
					Interlocked.Exchange(ref _isRetrying, shouldRetry ? 1 : 0);

					if (shouldRetry)
					{
						_logger?.LogWarning(
							$"Failed to send message to {_targetId} (Attempt {currentRetry}/{_maxRetries}): {ex.Message}");
						Progress = (float)currentRetry / _maxRetries;
						Thread.Sleep(_retryDelayMilliseconds);
					}
					else
					{
						throw new CoreMessageException(
							$"Failed to send message to {_targetId} after {_maxRetries} attempts",
							ex);
					}
				}
			} while (Interlocked.CompareExchange(ref _isRetrying, 0, 0) == 1 && !_cts.Token.IsCancellationRequested);
		}

		// 현재 재시도 횟수를 안전하게 읽는 속성 추가
		public int CurrentRetryCount => Interlocked.CompareExchange(ref _currentRetry, 0, 0);

		// 재시도 중인지 여부를 안전하게 확인하는 속성 추가
		public bool IsRetrying => Interlocked.CompareExchange(ref _isRetrying, 0, 0) == 1;

		protected virtual void HandleSuccess()
		{
			Status = AsyncOperationStatus.Completed;
			Result = true;
			OnCompleted();
		}

		protected virtual void HandleError(Exception ex)
		{
			Status = AsyncOperationStatus.Failed;
			Error = ex;
			OnCompleted();
		}

		public override void Dispose()
		{
			_cts.Cancel();
			_cts.Dispose();
			base.Dispose();
		}
	}

	public class CoreMessageException : Exception
	{
		public CoreMessageException(string message) : base(message)
		{
		}

		public CoreMessageException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
