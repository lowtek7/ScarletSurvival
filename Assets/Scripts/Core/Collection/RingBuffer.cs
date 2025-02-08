namespace Scarlet.Core.Collection
{
	public class RingBuffer<T>
	{
		private readonly T[] buffer;
		private readonly int mask;
		private long writePosition;
		private long readPosition;

		public RingBuffer(int size)
		{
			// size를 2의 제곱수로 조정
			size = NextPowerOfTwo(size);
			buffer = new T[size];
			mask = size - 1;
		}

		public bool TryEnqueue(T item)
		{
			long currentWrite = writePosition;
			long currentRead = readPosition;
			long newWrite = currentWrite + 1;

			if ((newWrite - currentRead) > mask)
				return false; // 버퍼 가득 참

			buffer[currentWrite & mask] = item;
			writePosition = newWrite;
			return true;
		}

		public bool TryDequeue(out T item)
		{
			long currentRead = readPosition;
			long currentWrite = writePosition;

			if (currentRead >= currentWrite)
			{
				item = default;
				return false;
			}

			item = buffer[currentRead & mask];
			readPosition = currentRead + 1;
			return true;
		}

		private static int NextPowerOfTwo(int value)
		{
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value++;
			return value;
		}
	}
}
