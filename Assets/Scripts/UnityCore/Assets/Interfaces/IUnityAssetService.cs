using System;
using Scarlet.Core.Assets;
using Scarlet.Core.Assets.Interfaces;
using Scarlet.Core.Async.Interfaces;
using Scarlet.Core.Services.Results;

namespace Scarlet.UnityCore.Assets.Interfaces
{
	public interface IUnityAssetService : IAssetService
	{
		// Unity 특화 메서드
		IAsyncOperation<ServiceResult<T>> LoadUnityAssetAsync<T>(AssetId id, IProgress<float> progress = null)
			where T : UnityEngine.Object;
	}
}
