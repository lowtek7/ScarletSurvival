using System;

namespace Scarlet.Core.Math.Interfaces
{
	/// <summary>
	/// 모든 벡터 타입의 기본 인터페이스
	/// </summary>
	public interface IVector
	{
		float Magnitude { get; }
		float SqrMagnitude { get; }
		IVector Normalized { get; }
		void Normalize();
	}

	public interface IVector2 : IVector
	{
		float X { get; set; }
		float Y { get; set; }

		// 자주 사용되는 상수 벡터들
		static IVector2 Zero => throw new NotImplementedException();
		static IVector2 One => throw new NotImplementedException();
		static IVector2 Up => throw new NotImplementedException();
		static IVector2 Down => throw new NotImplementedException();
		static IVector2 Left => throw new NotImplementedException();
		static IVector2 Right => throw new NotImplementedException();
	}

	public interface IVector3 : IVector
	{
		float X { get; set; }
		float Y { get; set; }
		float Z { get; set; }

		// 자주 사용되는 상수 벡터들
		static IVector3 Zero => throw new NotImplementedException();
		static IVector3 One => throw new NotImplementedException();
		static IVector3 Up => throw new NotImplementedException();
		static IVector3 Down => throw new NotImplementedException();
		static IVector3 Left => throw new NotImplementedException();
		static IVector3 Right => throw new NotImplementedException();
		static IVector3 Forward => throw new NotImplementedException();
		static IVector3 Back => throw new NotImplementedException();
	}

	public interface IVector4 : IVector
	{
		float X { get; set; }
		float Y { get; set; }
		float Z { get; set; }
		float W { get; set; }

		static IVector4 Zero => throw new NotImplementedException();
		static IVector4 One => throw new NotImplementedException();
	}
}
