using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Scarlet.Core.Assets.Interfaces;
using Scarlet.Core.Async;
using Scarlet.Core.Async.Interfaces;

namespace Scarlet.Core.Assets.Loaders
{
	public class TextDataLoader : IAssetLoader
	{
		private readonly System.Text.Encoding encoding;

		public TextDataLoader(System.Text.Encoding encoding = null)
		{
			this.encoding = encoding ?? Encoding.Default;
		}

		public bool CanLoad<T>(AssetLoadContext context) where T : class
		{
			return typeof(T) == typeof(string);
		}

		public IAsyncOperation<T> LoadAsync<T>(AssetLoadContext context) where T : class
		{
			return new LoaderAsyncOperation<T>(context, async loadContext =>
			{
				loadContext.Progress?.Report(0f);

				string text;
				switch (loadContext.Source)
				{
					case AssetLoadSource.File:
						text = await File.ReadAllTextAsync(loadContext.Path, encoding);
						break;

					case AssetLoadSource.Memory:
						text = encoding.GetString(loadContext.Data);
						break;

					default:
						throw new ArgumentException($"Unsupported load source: {loadContext.Source}");
				}

				loadContext.Progress?.Report(1f);
				return text as T;
			});
		}
	}
}
