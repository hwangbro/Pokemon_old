using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public static class SaveStateUtils {

        private const int HeaderSize = 3;
        private const int SizeSize = 3;

        public static Dictionary<string, uint> GetLabels(byte[] fileData) {
            Dictionary<string, uint> labels = new Dictionary<string, uint>();

            int offset = HeaderSize;
            int size = ReadSize(fileData, offset);
            offset += SizeSize + size;

            while(offset < fileData.Length) {
                string key = ReadString(fileData, offset);
                offset += key.Length + 1;
                size = ReadSize(fileData, offset);
                offset += SizeSize;

                labels[key] = (uint) offset;

                offset += size;
            }

            return labels;
        }

        private static string ReadString(byte[] fileData, int offset) {
            string str = "";

            while(fileData[offset] != 0x00) {
                str += Convert.ToChar(fileData[offset++]);
            }

            return str;
        }

        private static int ReadSize(byte[] fileData, int offset) {
            return ((fileData[offset + 0] << 16) | (fileData[offset + 1] << 8) | fileData[offset + 2]);
        }
    }
}
