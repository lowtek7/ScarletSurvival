using System;
using System.Threading;
using System.Threading.Tasks;
using Scarlet.Core.Assets;
using Scarlet.Core.Async.Enums;
using Scarlet.Core.Logging.Interfaces;
using Scarlet.Core.Math.Scarlet.Core.Math;

namespace Scarlet.Core.Async
{
	public class LoaderAsyncOperation<T> : AsyncOperationBase<T>
	{
		private readonly Func<IProgress<float>, CancellationToken, T> _loader;
		private readonly ILogger _logger;
		private readonly CancellationTokenSource _cts;
		private readonly Progress<float> _progress;
		private int _isStarted;
		private int _isCanceled;

		public bool IsCanceled => Interlocked.CompareExchange(ref _isCanceled, 0, 0) == 1;

		public LoaderAsyncOperation(
			Func<IProgress<float>, CancellationToken, T> loader,
			ILogger logger = null,
			CancellationTokenSource externalCts = null)
		{
			_loader = loader ?? throw new ArgumentNullException(nameof(loader));
			_logger = logger;
			_cts = CancellationTokenSource.CreateLinkedTokenSource(
				externalCts?.Token ?? CancellationToken.None);

			_progress = new Progress<float>(UpdateProgress);
			Status = AsyncOperationStatus.Running;
		}

		public void Start()
		{
			if (Interlocked.Exchange(ref _isStarted, 1) == 0)
			{
				try
				{
					ThreadPool.QueueUserWorkItem(_ => ExecuteLoader(), null);
				}
				catch (Exception ex)
				{
					HandleError(ex);
				}
			}
		}

		private void ExecuteLoader()
		{
			try
			{
				if (_cts.Token.IsCancellationRequested)
				{
					HandleCancellation();
					return;
				}

				var result = _loader(_progress, _cts.Token);
				if (_cts.Token.IsCancellationRequested)
				{
					HandleCancellation();
					return;
				}

				HandleSuccess(result);
			}
			catch (OperationCanceledException)
			{
				HandleCancellation();
			}
			catch (Exception ex)
			{
				HandleError(ex);
			}
		}

		private void UpdateProgress(float progress)
		{
			if (Status >= AsyncOperationStatus.Completed)
				return;

			progress = CoreMathf.Max(0f, CoreMathf.Min(1f, progress));
			Progress = progress;
		}

		public void Cancel()
		{
			if (Interlocked.Exchange(ref _isCanceled, 1) == 0)
			{
				_cts.Cancel();
				_logger?.LogDebug($"LoaderAsyncOperation cancelled: {typeof(T).Name}");
			}
		}

		private void HandleSuccess(T result)
		{
			if (Status >= AsyncOperationStatus.Completed)
				return;

			Result = result;
			Progress = 1f;
			Status = AsyncOperationStatus.Completed;
			OnCompleted();
		}

		private void HandleError(Exception ex)
		{
			if (Status >= AsyncOperationStatus.Completed)
				return;

			_logger?.LogError($"LoaderAsyncOperation failed: {ex.Message}");
			Error = ex;
			Status = AsyncOperationStatus.Failed;
			OnCompleted();
		}

		private void HandleCancellation()
		{
			if (Status >= AsyncOperationStatus.Completed)
				return;

			Status = AsyncOperationStatus.Canceled;
			OnCompleted();
		}

		public override void Dispose()
		{
			Cancel();
			_cts.Dispose();
			base.Dispose();
		}
	}
}
