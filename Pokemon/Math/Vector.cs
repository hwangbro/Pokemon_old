using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Pokemon {

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct Vec2 {

        public static readonly Vec2 Zero = new Vec2(0);
        public static readonly Vec2 One =  new Vec2(1);

        public float X { get; set; }
        public float Y { get; set; }

        public float R {
            get { return X; }
        }

        public float G {
            get { return Y; }
        }

        public Vec2(float value) => (X, Y) = (value, value);
        public Vec2(float[] array) => (X, Y) = (array[0], array[1]);
        public Vec2(float x, float y) => (X, Y) = (x, y);
        public Vec2(Vec2 r) => (X, Y) = (r.X, r.Y);

        public float this[int index] {
            get {
                Debug.Assert(index >= 0 && index < 2, "Out of range!");
                if(index == 0) return X;
                else return Y;
            }
            set {
                Debug.Assert(index >= 0 && index < 2, "Out of range!");
                if(index == 0) X = value;
                else Y = value;
            }
        }

        public float Dot(Vec2 r) {
            return X * r.X + Y * r.Y;
        }

        public float LengthSquared() {
            return Dot(this);
        }

        public float Length() {
            return MathF.Sqrt(LengthSquared());
        }

        public Vec2 Normalized() {
            return this / Length();
        }

        public static Vec2 operator +(Vec2 lhs, Vec2 rhs) {
            return new Vec2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        public static Vec2 operator +(Vec2 lhs, float rhs) {
            return new Vec2(lhs.X + rhs, lhs.Y + rhs);
        }

        public static Vec2 operator -(Vec2 lhs, Vec2 rhs) {
            return new Vec2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Vec2 operator -(Vec2 lhs, float rhs) {
            return new Vec2(lhs.X - rhs, lhs.Y - rhs);
        }

        public static Vec2 operator *(Vec2 self, float s) {
            return new Vec2(self.X * s, self.Y * s);
        }

        public static Vec2 operator *(float lhs, Vec2 rhs) {
            return new Vec2(rhs.X * lhs, rhs.Y * lhs);
        }

        public static Vec2 operator *(Vec2 lhs, Vec2 rhs) {
            return new Vec2(rhs.X * lhs.X, rhs.Y * lhs.Y);
        }

        public static Vec2 operator /(Vec2 lhs, float rhs) {
            return new Vec2(lhs.X / rhs, lhs.Y / rhs);
        }

        public static bool operator ==(Vec2 v1, Vec2 v2) {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vec2 v1, Vec2 v2) {
            return !v1.Equals(v2);
        }

        public float[] ToArray() {
            return new float[] { X, Y };
        }

        public override bool Equals(object obj) {
            if(obj.GetType() == typeof(Vec2)) {
                Vec2 vec = (Vec2) obj;
                if(X == vec.X && Y == vec.Y) {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode() {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override string ToString() {
            return string.Format("[{0}, {1}]", X, Y);
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct Vec3 {

        public static readonly Vec3 Zero = new Vec3(0);
        public static readonly Vec3 One =  new Vec3(1);

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public float R {
            get { return X; }
        }

        public float G {
            get { return Y; }
        }

        public float B {
            get { return Z; }
        }

        public Vec3(float value) => (X, Y, Z) = (value, value, value);
        public Vec3(float[] array) => (X, Y, Z) = (array[0], array[1], array[2]);
        public Vec3(float x, float y, float z) => (X, Y, Z) = (x, y, z);
        public Vec3(Vec2 r, float z) => (X, Y, Z) = (r.X, r.Y, z);
        public Vec3(Vec3 r) => (X, Y, Z) = (r.X, r.Y, r.Z);

        public float this[int index] {
            get {
                Debug.Assert(index >= 0 && index < 3, "Out of range!");
                if(index == 0) return X;
                else if(index == 1) return Y;
                else return Z;
            }
            set {
                Debug.Assert(index >= 0 && index < 3, "Out of range!");
                if(index == 0) X = value;
                else if(index == 1) Y = value;
                else Z = value;
            }
        }

        public float Dot(Vec3 r) {
            return X * r.X + Y * r.Y + Z * r.Z;
        }

        public float LengthSquared() {
            return Dot(this);
        }

        public float Length() {
            return MathF.Sqrt(LengthSquared());
        }

        public Vec3 Normalized() {
            return this / Length();
        }

        public Vec3 Cross(Vec3 r) {
            return new Vec3(
                Y * r.Z - r.Y * Z,
                Z * r.X - r.Z * X,
                X * r.Y - r.X * Y);
        }

        public static Vec3 operator +(Vec3 lhs, Vec3 rhs) {
            return new Vec3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static Vec3 operator +(Vec3 lhs, float rhs) {
            return new Vec3(lhs.X + rhs, lhs.Y + rhs, lhs.Z + rhs);
        }

        public static Vec3 operator -(Vec3 lhs, Vec3 rhs) {
            return new Vec3(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vec3 operator -(Vec3 lhs, float rhs) {
            return new Vec3(lhs.X - rhs, lhs.Y - rhs, lhs.Z - rhs);
        }

        public static Vec3 operator *(Vec3 self, float s) {
            return new Vec3(self.X * s, self.Y * s, self.Z * s);
        }
        public static Vec3 operator *(float lhs, Vec3 rhs) {
            return new Vec3(rhs.X * lhs, rhs.Y * lhs, rhs.Z * lhs);
        }

        public static Vec3 operator /(Vec3 lhs, float rhs) {
            return new Vec3(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs);
        }

        public static Vec3 operator *(Vec3 lhs, Vec3 rhs) {
            return new Vec3(rhs.X * lhs.X, rhs.Y * lhs.Y, rhs.Z * lhs.Z);
        }

        public static bool operator ==(Vec3 v1, Vec3 v2) {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vec3 v1, Vec3 v2) {
            return !v1.Equals(v2);
        }

        public float[] ToArray() {
            return new float[] { X, Y, Z };
        }

        public override bool Equals(object obj) {
            if(obj.GetType() == typeof(Vec3)) {
                Vec3 vec = (Vec3) obj;
                if(X == vec.X && Y == vec.Y && Z == vec.Z) {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode() {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public override string ToString() {
            return string.Format("[{0}, {1}, {2}]", X, Y, Z);
        }
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct Vec4 {

        public static readonly Vec4 Zero = new Vec4(0);
        public static readonly Vec4 One =  new Vec4(1);

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public float R {
            get { return X; }
        }

        public float G {
            get { return Y; }
        }

        public float B {
            get { return Z; }
        }

        public float A {
            get { return W; }
        }

        public Vec4(float value) => (X, Y, Z, W) = (value, value, value, value);
        public Vec4(float[] array) => (X, Y, Z, W) = (array[0], array[1], array[2], array[3]);
        public Vec4(float x, float y, float z, float w) => (X, Y, Z, W) = (x, y, z, w);
        public Vec4(Vec3 r, float w) => (X, Y, Z, W) = (r.X, r.Y, r.Z, w);
        public Vec4(Vec4 r) => (X, Y, Z, W) = (r.X, r.Y, r.Z, r.W);

        public float Dot(Vec4 r) {
            return X * r.X + Y * r.Y + Z * r.Z + W * r.W;
        }

        public float LengthSquared() {
            return Dot(this);
        }

        public float Length() {
            return MathF.Sqrt(LengthSquared());
        }

        public Vec4 Normalized() {
            return this / Length();
        }

        public Vec4 Lerp(Vec4 r, float lerpFactor) {
            return (r - this) * lerpFactor + this;
        }

        public float this[int index] {
            get {
                Debug.Assert(index >= 0 && index < 4, "Out of range!");
                if(index == 0) return X;
                else if(index == 1) return Y;
                else if(index == 2) return Z;
                else return W;
            }
            set {
                Debug.Assert(index >= 0 && index < 4, "Out of range!");
                if(index == 0) X = value;
                else if(index == 1) Y = value;
                else if(index == 2) Z = value;
                else W = value;
            }
        }

        public static Vec4 operator +(Vec4 lhs, Vec4 rhs) {
            return new Vec4(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.W + rhs.W);
        }

        public static Vec4 operator +(Vec4 lhs, float rhs) {
            return new Vec4(lhs.X + rhs, lhs.Y + rhs, lhs.Z + rhs, lhs.W + rhs);
        }

        public static Vec4 operator -(Vec4 lhs, float rhs) {
            return new Vec4(lhs.X - rhs, lhs.Y - rhs, lhs.Z - rhs, lhs.W - rhs);
        }

        public static Vec4 operator -(Vec4 lhs, Vec4 rhs) {
            return new Vec4(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z, lhs.W - rhs.W);
        }

        public static Vec4 operator *(Vec4 self, float s) {
            return new Vec4(self.X * s, self.Y * s, self.Z * s, self.W * s);
        }

        public static Vec4 operator *(float lhs, Vec4 rhs) {
            return new Vec4(rhs.X * lhs, rhs.Y * lhs, rhs.Z * lhs, rhs.W * lhs);
        }

        public static Vec4 operator *(Vec4 lhs, Vec4 rhs) {
            return new Vec4(rhs.X * lhs.X, rhs.Y * lhs.Y, rhs.Z * lhs.Z, rhs.W * lhs.W);
        }

        public static Vec4 operator /(Vec4 lhs, float rhs) {
            return new Vec4(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs, lhs.W / rhs);
        }

        public static bool operator ==(Vec4 v1, Vec4 v2) {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vec4 v1, Vec4 v2) {
            return !v1.Equals(v2);
        }

        public float[] ToArray() {
            return new float[] { X, Y, Z, Z };
        }

        public override bool Equals(object obj) {
            if(obj.GetType() == typeof(Vec4)) {
                Vec4 vec = (Vec4) obj;
                if(X == vec.X && Y == vec.Y && Z == vec.Z && W == vec.W) {
                    return true;
                }
            }
            return false;
        }

        public override int GetHashCode() {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        public override string ToString() {
            return string.Format("[{0}, {1}, {2}, {3}]", X, Y, Z, W);
        }
    }
}
