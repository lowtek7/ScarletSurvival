using System;
using System.Collections.Generic;

namespace Scarlet.Core.Assets
{
	public enum AssetLoadSource
	{
		File,
		Memory
	}

	public class AssetLoadContext
	{
		public string Path { get; private set; }
		public byte[] Data { get; private set; }
		public AssetLoadSource Source { get; private set; }
		public IProgress<float> Progress { get; private set; }
		public Dictionary<string, object> Parameters { get; private set; }

		private AssetLoadContext(AssetLoadSource source)
		{
			Source = source;
			Parameters = new Dictionary<string, object>();
		}

		public static AssetLoadContext FromPath(string path, IProgress<float> progress = null)
		{
			return new AssetLoadContext(AssetLoadSource.File)
			{
				Path = path,
				Progress = progress
			};
		}

		public static AssetLoadContext FromMemory(byte[] data, IProgress<float> progress = null)
		{
			return new AssetLoadContext(AssetLoadSource.Memory)
			{
				Data = data,
				Progress = progress
			};
		}

		public void SetParameter<T>(string key, T value)
		{
			Parameters[key] = value;
		}

		public T GetParameter<T>(string key, T defaultValue = default)
		{
			return Parameters.TryGetValue(key, out var value) && value is T typedValue
				? typedValue
				: defaultValue;
		}
	}
}
