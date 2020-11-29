using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Pokemon {

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct Mat4 {

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private Vec4[] Cols;

        public Mat4(float scale) {
            Cols = new[] {
                new Vec4(scale, 0.0f, 0.0f, 0.0f),
                new Vec4(0.0f, scale, 0.0f, 0.0f),
                new Vec4(0.0f, 0.0f, scale, 0.0f),
                new Vec4(0.0f, 0.0f, 0.0f, scale),
            };
        }

        public Mat4(Vec4[] cols) {
            Cols = new Vec4[] {
                cols[0],
                cols[1],
                cols[2],
                cols[3]
            };
        }

        public Mat4(Vec4 a, Vec4 b, Vec4 c, Vec4 d) {
            Cols = new Vec4[] {
                a, b, c, d
            };
        }

        public static Mat4 Identity() {
            return new Mat4(
                    new Vec4(1, 0, 0, 0),
                    new Vec4(0, 1, 0, 0),
                    new Vec4(0, 0, 1, 0),
                    new Vec4(0, 0, 0, 1)
                );
        }

        public static Mat4 Frustum(float left, float right, float bottom, float top, float nearVal, float farVal) {
            Mat4 result = Identity();
            result[0, 0] = (2.0f * nearVal) / (right - left);
            result[1, 1] = (2.0f * nearVal) / (top - bottom);
            result[2, 0] = (right + left) / (right - left);
            result[2, 1] = (top + bottom) / (top - bottom);
            result[2, 2] = -(farVal + nearVal) / (farVal - nearVal);
            result[2, 3] = -1.0f;
            result[3, 2] = -(2.0f * farVal * nearVal) / (farVal - nearVal);
            return result;
        }

        public static Mat4 InfinitePerspective(float fovy, float aspect, float zNear) {
            float range = MathF.Tan(fovy / (2f)) * zNear;

            float left = -range * aspect;
            float right = range * aspect;
            float bottom = -range;
            float top = range;

            Mat4 result = new Mat4(0);
            result[0, 0] = ((2f) * zNear) / (right - left);
            result[1, 1] = ((2f) * zNear) / (top - bottom);
            result[2, 2] = -(1f);
            result[2, 3] = -(1f);
            result[3, 2] = -(2f) * zNear;
            return result;
        }

        public static Mat4 LookAt(Vec3 eye, Vec3 center, Vec3 up) {
            Vec3 f = (center - eye).Normalized();
            Vec3 s = f.Cross(up).Normalized();
            Vec3 u = s.Cross(f).Normalized();

            Mat4 result = Identity();
            result[0, 0] = s.X;
            result[1, 0] = s.Y;
            result[2, 0] = s.Z;
            result[0, 1] = u.X;
            result[1, 1] = u.Y;
            result[2, 1] = u.Z;
            result[0, 2] = -f.X;
            result[1, 2] = -f.Y;
            result[2, 2] = -f.Z;
            result[3, 0] = -s.Dot(eye);
            result[3, 1] = -u.Dot(eye);
            result[3, 2] =  f.Dot(eye);
            return result;
        }

        public static Mat4 Orthographic(float left, float right, float bottom, float top, float zNear, float zFar) {
            Mat4 result = Identity();
            result[0, 0] = (2f) / (right - left);
            result[1, 1] = (2f) / (top - bottom);
            result[2, 2] = -(2f) / (zFar - zNear);
            result[3, 0] = -(right + left) / (right - left);
            result[3, 1] = -(top + bottom) / (top - bottom);
            result[3, 2] = -(zFar + zNear) / (zFar - zNear);
            return result;
        }

        public static Mat4 Orthographic(float left, float right, float bottom, float top) {
            Mat4 result = Identity();
            result[0, 0] = (2f) / (right - left);
            result[1, 1] = (2f) / (top - bottom);
            result[2, 2] = -(1f);
            result[3, 0] = -(right + left) / (right - left);
            result[3, 1] = -(top + bottom) / (top - bottom);
            return result;
        }

        public static Mat4 Perspective(float fovy, float aspect, float zNear, float zFar) {
            float tanHalfFovy = MathF.Tan(fovy / 2.0f);

            Mat4 result = Identity();
            result[0, 0] = 1.0f / (aspect * tanHalfFovy);
            result[1, 1] = 1.0f / (tanHalfFovy);
            result[2, 2] = -(zFar + zNear) / (zFar - zNear);
            result[2, 3] = -1.0f;
            result[3, 2] = -(2.0f * zFar * zNear) / (zFar - zNear);
            result[3, 3] = 0.0f;
            return result;
        }

        public static Mat4 PerspectiveFov(float fov, float width, float height, float zNear, float zFar) {
            var h = MathF.Cos((0.5f) * fov) / MathF.Sin((0.5f) * fov);
            var w = h * height / width;

            Mat4 result = new Mat4(0);
            result[0, 0] = w;
            result[1, 1] = h;
            result[2, 2] = -(zFar + zNear) / (zFar - zNear);
            result[2, 3] = -(1f);
            result[3, 2] = -((2f) * zFar * zNear) / (zFar - zNear);
            return result;
        }

        public static Mat4 Rotate(float angle, Vec3 v) {
            float c = MathF.Cos(angle);
            float s = MathF.Sin(angle);

            Vec3 axis = v.Normalized();
            Vec3 temp = (1.0f - c) * axis;

            Mat4 rotate = Identity();
            rotate[0, 0] = c + temp[0] * axis[0];
            rotate[0, 1] = 0 + temp[0] * axis[1] + s * axis[2];
            rotate[0, 2] = 0 + temp[0] * axis[2] - s * axis[1];

            rotate[1, 0] = 0 + temp[1] * axis[0] - s * axis[2];
            rotate[1, 1] = c + temp[1] * axis[1];
            rotate[1, 2] = 0 + temp[1] * axis[2] + s * axis[0];

            rotate[2, 0] = 0 + temp[2] * axis[0] + s * axis[1];
            rotate[2, 1] = 0 + temp[2] * axis[1] - s * axis[0];
            rotate[2, 2] = c + temp[2] * axis[2];

            Mat4 result = Identity();
            Mat4 m = Identity();
            result[0] = m[0] * rotate[0][0] + m[1] * rotate[0][1] + m[2] * rotate[0][2];
            result[1] = m[0] * rotate[1][0] + m[1] * rotate[1][1] + m[2] * rotate[1][2];
            result[2] = m[0] * rotate[2][0] + m[1] * rotate[2][1] + m[2] * rotate[2][2];
            result[3] = m[3];
            return result;
        }

        public static Mat4 Scale(Vec3 v) {
            Mat4 result = Identity();
            Mat4 m = Identity();
            result[0] = m[0] * v[0];
            result[1] = m[1] * v[1];
            result[2] = m[2] * v[2];
            result[3] = m[3];
            return result;
        }

        public static Mat4 Translate(Vec3 v) {
            Mat4 result = Identity();
            Mat4 m = Identity();
            result[3] = m[0] * v[0] + m[1] * v[1] + m[2] * v[2] + m[3];
            return result;
        }

        public Mat4 Inverse() {
            float coef00 = this[2][2] * this[3][3] - this[3][2] * this[2][3];
            float coef02 = this[1][2] * this[3][3] - this[3][2] * this[1][3];
            float coef03 = this[1][2] * this[2][3] - this[2][2] * this[1][3];

            float coef04 = this[2][1] * this[3][3] - this[3][1] * this[2][3];
            float coef06 = this[1][1] * this[3][3] - this[3][1] * this[1][3];
            float coef07 = this[1][1] * this[2][3] - this[2][1] * this[1][3];

            float coef08 = this[2][1] * this[3][2] - this[3][1] * this[2][2];
            float coef10 = this[1][1] * this[3][2] - this[3][1] * this[1][2];
            float coef11 = this[1][1] * this[2][2] - this[2][1] * this[1][2];

            float coef12 = this[2][0] * this[3][3] - this[3][0] * this[2][3];
            float coef14 = this[1][0] * this[3][3] - this[3][0] * this[1][3];
            float coef15 = this[1][0] * this[2][3] - this[2][0] * this[1][3];

            float coef16 = this[2][0] * this[3][2] - this[3][0] * this[2][2];
            float coef18 = this[1][0] * this[3][2] - this[3][0] * this[1][2];
            float coef19 = this[1][0] * this[2][2] - this[2][0] * this[1][2];

            float coef20 = this[2][0] * this[3][1] - this[3][0] * this[2][1];
            float coef22 = this[1][0] * this[3][1] - this[3][0] * this[1][1];
            float coef23 = this[1][0] * this[2][1] - this[2][0] * this[1][1];

            Vec4 fac0 = new Vec4(coef00, coef00, coef02, coef03);
            Vec4 fac1 = new Vec4(coef04, coef04, coef06, coef07);
            Vec4 fac2 = new Vec4(coef08, coef08, coef10, coef11);
            Vec4 fac3 = new Vec4(coef12, coef12, coef14, coef15);
            Vec4 fac4 = new Vec4(coef16, coef16, coef18, coef19);
            Vec4 fac5 = new Vec4(coef20, coef20, coef22, coef23);

            Vec4 vec0 = new Vec4(this[1][0], this[0][0], this[0][0], this[0][0]);
            Vec4 vec1 = new Vec4(this[1][1], this[0][1], this[0][1], this[0][1]);
            Vec4 vec2 = new Vec4(this[1][2], this[0][2], this[0][2], this[0][2]);
            Vec4 vec3 = new Vec4(this[1][3], this[0][3], this[0][3], this[0][3]);

            Vec4 inv0 = new Vec4(vec1 * fac0 - vec2 * fac1 + vec3 * fac2);
            Vec4 inv1 = new Vec4(vec0 * fac0 - vec2 * fac3 + vec3 * fac4);
            Vec4 inv2 = new Vec4(vec0 * fac1 - vec1 * fac3 + vec3 * fac5);
            Vec4 inv3 = new Vec4(vec0 * fac2 - vec1 * fac4 + vec2 * fac5);

            Vec4 signA = new Vec4(+1, -1, +1, -1);
            Vec4 signB = new Vec4(-1, +1, -1, +1);
            Mat4 inverse = new Mat4(inv0 * signA, inv1 * signB, inv2 * signA, inv3 * signB);

            Vec4 row0 = new Vec4(inverse[0][0], inverse[1][0], inverse[2][0], inverse[3][0]);

            Vec4 dot0 = new Vec4(this[0] * row0);
            float dot1 = (dot0.X + dot0.Y) + (dot0.Z + dot0.W);

            float oneOverDeterminant = (1f) / dot1;

            return inverse * oneOverDeterminant;
        }

        public float[] ToArray() {
            float[] result = new float[16];
            for(int i = 0; i < result.Length; i++) {
                result[i] = Cols[i / 4][i % 4];
            }
            return result;
        }

        public Vec4 this[int index] {
            get {
                Debug.Assert(index >= 0 && index < 4, "Out of range!");
                return Cols[index];
            }
            set {
                Debug.Assert(index >= 0 && index < 4, "Out of range!");
                Cols[index] = value;
            }
        }

        public float this[int column, int row] {
            get {
                Debug.Assert(column >= 0 && column < 4 && row >= 0 && row < 4, "Out of range!");
                return Cols[column][row];
            }
            set {
                Debug.Assert(column >= 0 && column < 4 && row >= 0 && row < 4, "Out of range!");
                Cols[column][row] = value;
            }
        }

        public static Vec4 operator *(Mat4 a, Vec4 b) {
            return new Vec4(
                a[0, 0] * b[0] + a[1, 0] * b[1] + a[2, 0] * b[2] + a[3, 0] * b[3],
                a[0, 1] * b[0] + a[1, 1] * b[1] + a[2, 1] * b[2] + a[3, 1] * b[3],
                a[0, 2] * b[0] + a[1, 2] * b[1] + a[2, 2] * b[2] + a[3, 2] * b[3],
                a[0, 3] * b[0] + a[1, 3] * b[1] + a[2, 3] * b[2] + a[3, 3] * b[3]
            );
        }

        public static Mat4 operator *(Mat4 a, Mat4 b) {
            return new Mat4(new Vec4[] {
                b[0][0] * a[0] + b[0][1] * a[1] + b[0][2] * a[2] + b[0][3] * a[3],
                b[1][0] * a[0] + b[1][1] * a[1] + b[1][2] * a[2] + b[1][3] * a[3],
                b[2][0] * a[0] + b[2][1] * a[1] + b[2][2] * a[2] + b[2][3] * a[3],
                b[3][0] * a[0] + b[3][1] * a[1] + b[3][2] * a[2] + b[3][3] * a[3]
            });
        }

        public static Mat4 operator *(Mat4 a, float s) {
            return new Mat4(new[] {
                a[0] * s,
                a[1] * s,
                a[2] * s,
                a[3] * s
            });
        }
    }
}
