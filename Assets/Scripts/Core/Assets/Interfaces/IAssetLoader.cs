using System;
using System.Threading.Tasks;
using Scarlet.Core.Async.Interfaces;

namespace Scarlet.Core.Assets.Interfaces
{
	public interface IAssetLoader
	{
		IAsyncOperation<T> LoadAsync<T>(AssetLoadContext context) where T : class;
		bool CanLoad<T>(AssetLoadContext context) where T : class;
	}
}
