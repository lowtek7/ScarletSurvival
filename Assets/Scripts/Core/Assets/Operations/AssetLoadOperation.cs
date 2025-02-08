using System;
using Scarlet.Core.Async;
using Scarlet.Core.Services.Results;

namespace Scarlet.Core.Assets.Operations
{
	public class AssetLoadOperation<T> : AsyncOperationBase<ServiceResult<T>> where T : class
	{
		private readonly AssetId assetId;
		private readonly IProgress<float> progress;

		public AssetLoadOperation(AssetId assetId, IProgress<float> progress = null)
		{
			this.assetId = assetId;
			this.progress = progress;
		}

		protected void ReportProgress(float inProgress)
		{
			UpdateProgress(inProgress);
			this.progress?.Report(inProgress);
		}
	}
}
