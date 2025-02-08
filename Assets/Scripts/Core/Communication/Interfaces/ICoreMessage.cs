namespace Scarlet.Core.Communication.Interfaces
{
	public interface ICoreMessage
	{
		string MessageType { get; }
		byte[] Serialize();
		void Deserialize(byte[] data);
	}
}
