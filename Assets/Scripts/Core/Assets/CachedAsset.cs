namespace Scarlet.Core.Assets
{
	public class CachedAsset
	{
		public object Asset { get; }
		public int ReferenceCount { get; private set; }

		public CachedAsset(object asset)
		{
			Asset = asset;
			ReferenceCount = 1;
		}

		public void AddReference() => ReferenceCount++;
		public bool RemoveReference() => --ReferenceCount <= 0;
	}
}
