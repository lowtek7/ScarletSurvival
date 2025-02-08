using Scarlet.Core.Math.Interfaces;

namespace Scarlet.Core.Math.Factories
{
	/// <summary>
	/// 코어 구현부에서 사용할 벡터 팩토리 클래스
	/// </summary>
	public class CoreVectorFactory : IVectorFactory
	{
		public IVector2 CreateVector2(float x, float y) => new CoreVector2(x, y);
		public IVector3 CreateVector3(float x, float y, float z) => new CoreVector3(x, y, z);
		public IVector4 CreateVector4(float x, float y, float z, float w) => new CoreVector4(x, y, z, w);

		// Vector2 상수 구현
		public IVector2 Zero2 => new CoreVector2(0f, 0f);
		public IVector2 One2 => new CoreVector2(1f, 1f);
		public IVector2 Up2 => new CoreVector2(0f, 1f);
		public IVector2 Down2 => new CoreVector2(0f, -1f);
		public IVector2 Left2 => new CoreVector2(-1f, 0f);
		public IVector2 Right2 => new CoreVector2(1f, 0f);

		// Vector3 상수 구현
		public IVector3 Zero3 => new CoreVector3(0f, 0f, 0f);
		public IVector3 One3 => new CoreVector3(1f, 1f, 1f);
		public IVector3 Up3 => new CoreVector3(0f, 1f, 0f);
		public IVector3 Down3 => new CoreVector3(0f, -1f, 0f);
		public IVector3 Left3 => new CoreVector3(-1f, 0f, 0f);
		public IVector3 Right3 => new CoreVector3(1f, 0f, 0f);
		public IVector3 Forward3 => new CoreVector3(0f, 0f, 1f);
		public IVector3 Back3 => new CoreVector3(0f, 0f, -1f);

		// Vector4 상수 구현
		public IVector4 Zero4 => new CoreVector4(0f, 0f, 0f, 0f);
		public IVector4 One4 => new CoreVector4(1f, 1f, 1f, 1f);
	}
}
