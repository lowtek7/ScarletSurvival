using System;
using Scarlet.Core.Assets.Interfaces;
using Scarlet.Core.Async;
using Scarlet.Core.Services.Results;

namespace Scarlet.Core.Assets.Operations
{
	public class NativeMemoryAssetLoadOperation<T> : AsyncOperationBase<ServiceResult<T>> where T : class
	{
		private readonly AssetId assetId;
		private readonly IAssetLoader loader;
		private readonly AssetLoadContext loadContext;

		public NativeMemoryAssetLoadOperation(
			AssetId assetId,
			IAssetLoader loader,
			byte[] data,
			IProgress<float> progress = null)
		{
			this.assetId = assetId;
			this.loader = loader;
			loadContext = CreateLoadContext(assetId, data, progress);
			ExecuteAsync();
		}

		private AssetLoadContext CreateLoadContext(AssetId assetIdOnLoad, byte[] data, IProgress<float> progress)
		{
			var context = AssetLoadContext.FromMemory(data, progress);
			context.SetParameter("assetType", assetIdOnLoad.Type);
			return context;
		}

		private async void ExecuteAsync()
		{
			try
			{
				if (!loader.CanLoad<T>(loadContext))
				{
					SetResult(ServiceResult<T>.Failure(
						$"Loader {loader.GetType().Name} cannot load type {typeof(T).Name} from memory"));
					return;
				}

				var operation = loader.LoadAsync<T>(loadContext);
				var result = await operation.Task;

				if (result != null)
				{
					SetResult(ServiceResult<T>.Success(result));
				}
				else
				{
					SetResult(ServiceResult<T>.Failure(
						$"Failed to load asset from memory: {assetId.Value}"));
				}
			}
			catch (Exception ex)
			{
				SetResult(ServiceResult<T>.Failure(
					$"Error loading asset from memory {assetId.Value}: {ex.Message}"));
			}
		}
	}
}
