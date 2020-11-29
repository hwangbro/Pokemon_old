using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Pokemon {

    public class Debug {

        private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long GetTime() {
            return (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        public static void Assert(bool condition, string str, params object[] args) {
            if(!condition) {
                Console.WriteLine("Assertion Failed: {0}", string.Format(str, args));
                System.Diagnostics.Debugger.Break();
            }
        }
    }

    static class Extensions {

        public static T GetKeyByValue<T, W>(this Dictionary<T, W> dict, W val) {
            T key = default;
            foreach(KeyValuePair<T, W> pair in dict) {
                if(EqualityComparer<W>.Default.Equals(pair.Value, val)) {
                    key = pair.Key;
                    break;
                }
            }
            return key;
        }

        public static int FindFirstNotOf(this string source, string chars, int start = 0) {
            for(int i = start; i < source.Length; i++) {
                if(chars.IndexOf(source[i]) == -1) return i;
            }
            return -1;
        }

        public static T[] Subarray<T>(this T[] source, int index, int length) {
            T[] subarray = new T[length];
            Array.Copy(source, index, subarray, 0, length);
            return subarray;
        }

        public static T[] Subarray<T>(this T[] source, ref int index, int length) {
            T[] ret = Subarray(source, index, length);
            index += length;
            return ret;
        }

        public static T[] Subarray<T>(this T[] source, RomAddress index, int length) {
            return Subarray(source, (int) index.Value, length);
        }

        public static ushort ReadWord(this byte[] value, int index) {
            return (ushort) ((value[index + 1] << 8) | value[index]);
        }

        public static ushort ReadWord(this byte[] value, ref int pointer) {
            ushort ret = value.ReadWord(pointer);
            pointer += 2;
            return ret;
        }

        public static uint ReadBW(this byte[] value, ref int pointer) {
            uint ret = value.ReadDWord(pointer);
            pointer += 4;
            return ret;
        }

        public static uint ReadDWord(this byte[] value, int index) {
            return (uint) ((value[index + 3] << 24) | (value[index + 2] << 16) | (value[index + 1] << 8) | value[index]);
        }

        public static uint ReadDWord(this byte[] value, ref int pointer) {
            uint ret = value.ReadDWord(pointer);
            pointer += 4;
            return ret;
        }

        public static T[] Add<T>(this T[] source, params T[] value) {
            T[] result = new T[source.Length + value.Length];
            Array.Copy(source, 0, result, 0, source.Length);
            Array.Copy(value, 0, result, source.Length, value.Length);
            return result;
        }

        public static byte[] ReadUntil(this byte[] source, byte b, ref int pointer) {
            byte[] ret = new byte[0];
            byte readByte;
            while((readByte = source[pointer++]) != b) ret = ret.Add(readByte);
            return ret;
        }

        public static T Consume<T>(this byte[] array, ref int pointer) where T : unmanaged {
            T ret = ReadStruct<T>(array, pointer);
            pointer += Marshal.SizeOf<T>();
            return ret;
        }

        public static T ReadStruct<T>(this byte[] array, int pointer) where T : unmanaged {
            int structSize = Marshal.SizeOf<T>();
            IntPtr ptr = Marshal.AllocHGlobal(structSize);
            Marshal.Copy(array, pointer, ptr, structSize);
            T str = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);
            return str;
        }

        public static byte[] ToBytes<T>(this T str) where T : unmanaged {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }
    }
}