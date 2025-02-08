using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Scarlet.Core.Assets.Interfaces;
using Scarlet.Core.Async;
using Scarlet.Core.Async.Interfaces;

namespace Scarlet.Core.Assets.Loaders
{
	public class BinaryDataLoader : IAssetLoader
	{
		public bool CanLoad<T>(AssetLoadContext context) where T : class
		{
			return typeof(T) == typeof(byte[]);
		}

		public IAsyncOperation<T> LoadAsync<T>(AssetLoadContext context) where T : class
		{
			return new LoaderAsyncOperation<T>(context, async loadContext =>
			{
				loadContext.Progress?.Report(0f);

				byte[] data;
				switch (loadContext.Source)
				{
					case AssetLoadSource.File:
						data = await File.ReadAllBytesAsync(loadContext.Path);
						break;

					case AssetLoadSource.Memory:
						data = loadContext.Data;
						break;

					default:
						throw new ArgumentException($"Unsupported load source: {loadContext.Source}");
				}

				loadContext.Progress?.Report(1f);
				return data as T;
			});
		}
	}
}
