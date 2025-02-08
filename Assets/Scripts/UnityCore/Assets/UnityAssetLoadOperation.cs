// using System;
// using Cysharp.Threading.Tasks;
// using Scarlet.Core.Assets;
// using Scarlet.Core.Assets.Operations;
// using Scarlet.Core.Services.Results;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using Object = UnityEngine.Object;
//
// namespace Scarlet.UnityCore.Assets
// {
// 	public class UnityAssetLoadOperation<T> : AssetLoadOperation<T> where T : Object
// 	{
// 		private readonly string path;
//
// 		public UnityAssetLoadOperation(AssetId assetId, IProgress<float> progress = null)
// 			: base(assetId, progress)
// 		{
// 			path = assetId.Value;
// 			ExecuteAsync().Forget();
// 		}
//
// 		private async UniTaskVoid ExecuteAsync()
// 		{
// 			try
// 			{
// 				var request = Resources.LoadAsync<T>(path);
//
// 				while (!request.isDone && !IsCancellationRequested)
// 				{
// 					ReportProgress(request.progress);
// 					await UniTask.Yield(PlayerLoopTiming.Update);
// 				}
//
// 				if (IsCancellationRequested)
// 				{
// 					return;
// 				}
//
// 				if (request.asset is T result)
// 				{
// 					SetResult(ServiceResult<T>.Success(result));
// 				}
// 				else
// 				{
// 					SetResult(ServiceResult<T>.Failure($"Failed to load asset: {path}"));
// 				}
// 			}
// 			catch (Exception ex)
// 			{
// 				SetException(ex);
// 			}
// 		}
//
// 		// Addressables 사용 시의 구현 예시
// 		private async UniTaskVoid LoadAddressableAsync()
// 		{
// 			try
// 			{
// 				var operation = Addressables.LoadAssetAsync<T>(path);
//
// 				while (!operation.IsDone && !IsCancellationRequested)
// 				{
// 					ReportProgress(operation.PercentComplete);
// 					await UniTask.Yield(PlayerLoopTiming.Update);
// 				}
//
// 				if (IsCancellationRequested)
// 				{
// 					await operation.Task; // 작업 완료 대기
// 					Addressables.Release(operation);
// 					return;
// 				}
//
// 				var result = await operation.Task;
// 				if (result != null)
// 				{
// 					SetResult(ServiceResult<T>.Success(result));
// 				}
// 				else
// 				{
// 					SetResult(ServiceResult<T>.Failure($"Failed to load addressable asset: {path}"));
// 				}
// 			}
// 			catch (Exception ex)
// 			{
// 				SetException(ex);
// 			}
// 		}
//
// 		// 번들 에셋 로드 예시
// 		private async UniTaskVoid LoadFromBundleAsync(AssetBundle bundle)
// 		{
// 			try
// 			{
// 				var request = bundle.LoadAssetAsync<T>(path);
//
// 				while (!request.isDone && !IsCancellationRequested)
// 				{
// 					ReportProgress(request.progress);
// 					await UniTask.Yield(PlayerLoopTiming.Update);
// 				}
//
// 				if (IsCancellationRequested)
// 				{
// 					return;
// 				}
//
// 				if (request.asset is T result)
// 				{
// 					SetResult(ServiceResult<T>.Success(result));
// 				}
// 				else
// 				{
// 					SetResult(ServiceResult<T>.Failure($"Failed to load bundle asset: {path}"));
// 				}
// 			}
// 			catch (Exception ex)
// 			{
// 				SetException(ex);
// 			}
// 		}
// 	}
// }
