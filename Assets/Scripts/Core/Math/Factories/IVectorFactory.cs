using Scarlet.Core.Math.Interfaces;

namespace Scarlet.Core.Math.Factories
{
	public interface IVectorFactory
	{
		IVector2 CreateVector2(float x, float y);
		IVector3 CreateVector3(float x, float y, float z);
		IVector4 CreateVector4(float x, float y, float z, float w);

		// 자주 사용되는 상수 벡터들
		// Vector2 상수
		IVector2 Zero2 { get; }
		IVector2 One2 { get; }
		IVector2 Up2 { get; }
		IVector2 Down2 { get; }
		IVector2 Left2 { get; }
		IVector2 Right2 { get; }

		// Vector3 상수
		IVector3 Zero3 { get; }
		IVector3 One3 { get; }
		IVector3 Up3 { get; }
		IVector3 Down3 { get; }
		IVector3 Left3 { get; }
		IVector3 Right3 { get; }
		IVector3 Forward3 { get; }
		IVector3 Back3 { get; }

		// Vector4 상수
		IVector4 Zero4 { get; }
		IVector4 One4 { get; }
	}
}
