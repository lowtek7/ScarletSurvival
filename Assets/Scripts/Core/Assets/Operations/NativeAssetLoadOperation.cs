using System;
using Scarlet.Core.Assets.Interfaces;
using Scarlet.Core.Async;
using Scarlet.Core.Services.Results;

namespace Scarlet.Core.Assets.Operations
{
	public class NativeAssetLoadOperation<T> : AsyncOperationBase<ServiceResult<T>> where T : class
	{
		private readonly AssetId assetId;
		private readonly IAssetLoader loader;
		private readonly AssetLoadContext assetLoadContext;

		public NativeAssetLoadOperation(
			AssetId assetId,
			IAssetLoader loader,
			IProgress<float> progress = null)
		{
			this.assetId = assetId;
			this.loader = loader;
			assetLoadContext = CreateLoadContext(assetId, progress);
			ExecuteAsync();
		}

		private AssetLoadContext CreateLoadContext(AssetId assetIdOnLoad, IProgress<float> progress)
		{
			// 기본적으로 파일 경로 기반으로 컨텍스트 생성
			var context = AssetLoadContext.FromPath(assetIdOnLoad.Value, progress);

			// 에셋 타입 정보 추가
			context.SetParameter("assetType", assetIdOnLoad.Type);

			return context;
		}

		private async void ExecuteAsync()
		{
			try
			{
				if (!loader.CanLoad<T>(assetLoadContext))
				{
					SetResult(ServiceResult<T>.Failure(
						$"Loader {loader.GetType().Name} cannot load type {typeof(T).Name}"));
					return;
				}

				var operation = loader.LoadAsync<T>(assetLoadContext);
				var result = await operation.Task;

				if (result != null)
				{
					SetResult(ServiceResult<T>.Success(result));
				}
				else
				{
					SetResult(ServiceResult<T>.Failure(
						$"Failed to load asset: {assetId.Value}"));
				}
			}
			catch (Exception ex)
			{
				SetResult(ServiceResult<T>.Failure(
					$"Error loading asset {assetId.Value}: {ex.Message}"));
			}
		}
	}
}
