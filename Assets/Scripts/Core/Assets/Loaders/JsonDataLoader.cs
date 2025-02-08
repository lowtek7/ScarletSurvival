using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Scarlet.Core.Assets.Interfaces;
using Scarlet.Core.Async;
using Scarlet.Core.Async.Interfaces;
using Scarlet.Core.Serialization;

namespace Scarlet.Core.Assets.Loaders
{
	public class JsonDataLoader : IAssetLoader
	{
		private readonly IJsonSerializer jsonSerializer;
		private readonly TextDataLoader textLoader;

		public JsonDataLoader(IJsonSerializer jsonSerializer)
		{
			this.jsonSerializer = jsonSerializer;
			textLoader = new TextDataLoader();
		}

		public bool CanLoad<T>(AssetLoadContext context) where T : class
		{
			return typeof(T).IsClass;
		}

		public IAsyncOperation<T> LoadAsync<T>(AssetLoadContext context) where T : class
		{
			return new LoaderAsyncOperation<T>(context, async loadContext =>
			{
				// TextLoader를 사용하여 JSON 문자열 로드
				var textOperation = textLoader.LoadAsync<string>(loadContext);
				var json = await textOperation.Task;

				loadContext.Progress?.Report(0.5f);

				// JSON 역직렬화
				var result = jsonSerializer.Deserialize<T>(json);

				loadContext.Progress?.Report(1f);
				return result;
			});
		}
	}
}
