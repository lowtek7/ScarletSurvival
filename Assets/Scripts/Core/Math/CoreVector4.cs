using System;
using Scarlet.Core.Math.Interfaces;

namespace Scarlet.Core.Math
{
    public struct CoreVector4 : IVector4
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public float Magnitude => MathF.Sqrt(SqrMagnitude);
        public float SqrMagnitude => X * X + Y * Y + Z * Z + W * W;

        public IVector Normalized
        {
            get
            {
                float mag = Magnitude;
                if (mag > 1E-05f)
                    return new CoreVector4(X / mag, Y / mag, Z / mag, W / mag);
                return new CoreVector4();
            }
        }

        public CoreVector4(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public void Normalize()
        {
            float mag = Magnitude;
            if (mag > 1E-05f)
            {
                X /= mag;
                Y /= mag;
                Z /= mag;
                W /= mag;
            }
            else
            {
                X = 0;
                Y = 0;
                Z = 0;
                W = 0;
            }
        }

        // 연산자 오버로딩
        public static CoreVector4 operator +(CoreVector4 a, CoreVector4 b)
            => new CoreVector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);

        public static CoreVector4 operator -(CoreVector4 a, CoreVector4 b)
            => new CoreVector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);

        public static CoreVector4 operator *(CoreVector4 a, float d)
            => new CoreVector4(a.X * d, a.Y * d, a.Z * d, a.W * d);

        public static CoreVector4 operator /(CoreVector4 a, float d)
            => new CoreVector4(a.X / d, a.Y / d, a.Z / d, a.W / d);

        public static CoreVector4 operator -(CoreVector4 a)
            => new CoreVector4(-a.X, -a.Y, -a.Z, -a.W);

        // 상수 벡터들
        public static IVector4 Zero => new CoreVector4(0f, 0f, 0f, 0f);
        public static IVector4 One => new CoreVector4(1f, 1f, 1f, 1f);

        public override string ToString()
            => $"({X:F1}, {Y:F1}, {Z:F1}, {W:F1})";
    }
}
