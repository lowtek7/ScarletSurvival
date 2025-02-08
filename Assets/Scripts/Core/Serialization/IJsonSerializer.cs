namespace Scarlet.Core.Serialization
{
	public interface IJsonSerializer
	{
		T Deserialize<T>(string json) where T : class;
		string Serialize<T>(T value) where T : class;
	}

}
