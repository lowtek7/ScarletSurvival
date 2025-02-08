using System;
using System.Collections.Generic;
using System.Linq;
using Scarlet.Core.Assets;
using Scarlet.Core.Assets.Interfaces;
using Scarlet.Core.Async;
using Scarlet.Core.Async.Interfaces;
using Scarlet.Core.Services.Results;
using UnityEngine;
using ILogger = Scarlet.Core.Logging.Interfaces.ILogger;
using Object = UnityEngine.Object;

namespace Scarlet.UnityCore.Assets
{
	// public class UnityAssetService : IAssetService
	// {
	// 	private readonly Dictionary<AssetId, CachedAsset> loadedAssets = new();
	// 	private readonly ILogger logger;
	//
	// 	public UnityAssetService(ILogger logger)
	// 	{
	// 		this.logger = logger;
	// 	}
	//
	// 	// IAssetService 구현 - 일반 객체용
	// 	IAsyncOperation<ServiceResult<T>> IAssetService.LoadAsync<T>(AssetId id, IProgress<float> progress)
	// 	{
	// 		// 일반 타입에 대한 처리
	// 		if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
	// 		{
	// 			logger.LogError($"Use LoadUnityAssetAsync for Unity asset types: {typeof(T).Name}");
	// 			return new ImmediateAsyncOperation<ServiceResult<T>>(
	// 				ServiceResult<T>.Failure("Invalid asset type for this loader"));
	// 		}
	//
	// 		// 일반 데이터 로드 로직 (예: JSON, 텍스트 등)
	// 		return new ImmediateAsyncOperation<ServiceResult<T>>(
	// 			ServiceResult<T>.Failure("Not implemented for non-Unity assets"));
	// 	}
	//
	// 	// Unity 에셋 전용 메서드 구현
	// 	public IAsyncOperation<ServiceResult<T>> LoadUnityAssetAsync<T>(AssetId id, IProgress<float> progress = null)
	// 		where T : UnityEngine.Object
	// 	{
	// 		try
	// 		{
	// 			if (loadedAssets.TryGetValue(id, out var cached))
	// 			{
	// 				if (cached.Asset is T asset)
	// 				{
	// 					cached.AddReference();
	// 					return new ImmediateAsyncOperation<ServiceResult<T>>(
	// 						ServiceResult<T>.Success(asset));
	// 				}
	// 			}
	//
	// 			var operation = new UnityAssetLoadOperation<T>(id, progress);
	//
	// 			operation.Task.ContinueWith(task =>
	// 			{
	// 				if (task.IsCompletedSuccessfully && task.Result.IsSuccess)
	// 				{
	// 					loadedAssets[id] = new CachedAsset(task.Result.Value);
	// 				}
	// 			});
	//
	// 			return operation;
	// 		}
	// 		catch (Exception ex)
	// 		{
	// 			logger.LogException(ex, $"Failed to start load operation for asset: {id}");
	// 			return new ImmediateAsyncOperation<ServiceResult<T>>(
	// 				ServiceResult<T>.Failure(ex.Message));
	// 		}
	// 	}
	//
	// 	public void UnloadAsset(AssetId id)
	// 	{
	// 		if (loadedAssets.TryGetValue(id, out var cached) && cached.RemoveReference())
	// 		{
	// 			if (cached.Asset is UnityEngine.Object unityObject)
	// 			{
	// 				Resources.UnloadAsset(unityObject);
	// 			}
	// 			loadedAssets.Remove(id);
	// 		}
	// 	}
	//
	// 	public bool IsLoaded(AssetId id) => loadedAssets.ContainsKey(id);
	//
	// 	public void UnloadUnusedAssets()
	// 	{
	// 		var unusedAssets = loadedAssets
	// 			.Where(kvp => kvp.Value.ReferenceCount <= 0)
	// 			.ToList();
	//
	// 		foreach (var asset in unusedAssets)
	// 		{
	// 			UnloadAsset(asset.Key);
	// 		}
	// 	}
	//
	// 	public void Initialize()
	// 	{
	// 	}
	//
	// 	public void Cleanup()
	// 	{
	// 	}
	//
	// 	public void Update()
	// 	{
	//
	// 	}
	// }
}
