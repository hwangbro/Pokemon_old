using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Pokemon {

    internal static class BMP {

        public const ushort BMPSignature = 0x4D42;
        public static readonly ComponentMapping BMPComponentMapping = ComponentMapping.BGRA;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BMPHeader {

            public ushort Signature;
            public uint FileSize;
            public ushort Reserved1;
            public ushort Reserved2;
            public uint DataOffset;
            public uint Size;
            public uint Width;
            public uint Height;
            public ushort Planes;
            public ushort BitsPerPixel;
            public uint Compression;
            public uint ImageSize;
            public uint HorzResolution;
            public uint VertResolution;
            public uint NumColors;
            public uint NumImportantColors;
        }

        internal static BitmapLoadingInfo Load(byte[] data) {
            int pointer = 0;
            BMPHeader header = data.Consume<BMPHeader>(ref pointer);
            Debug.Assert(header.Signature == BMPSignature, "Tried to load an invalid BMP file!");
            Debug.Assert(header.Compression == 0 || header.Compression == 3, "Only uncompressed BMP files with are currently supported!");

            int offset = (int) header.DataOffset;
            int width = (int) header.Width;
            int height = (int) header.Height;
            int numChannels = header.BitsPerPixel >> 3;

            return new BitmapLoadingInfo() {
                Width = width,
                Height = height,
                SrcData = data.Subarray(offset, width * height * numChannels),
                SrcComponentMapping = BMPComponentMapping,
                xFlip = false,
                yFlip = true,
            };
        }

        internal static byte[] Write(Bitmap bitmap) {
            int numChannels = bitmap.Data.Length / (bitmap.Width * bitmap.Height);

            uint headerSize = (uint) Marshal.SizeOf<BMPHeader>();
            BMPHeader header = new BMPHeader() {
                Signature = BMPSignature,
                FileSize = (uint) (headerSize + bitmap.Data.Length),
                Reserved1 = 0x0000,
                Reserved2 = 0x0000,
                DataOffset = headerSize,
                Size = 0x28,
                Width = (uint) bitmap.Width,
                Height = (uint) bitmap.Height,
                Planes = 1,
                BitsPerPixel = (ushort) (numChannels << 3),
                Compression = 0,
                ImageSize = 0,
                VertResolution = 0,
                HorzResolution = 0,
                NumColors = 0,
                NumImportantColors = 0,
            };

            byte[] pixels = new byte[bitmap.Width * bitmap.Height * numChannels];
            Bitmap.RemapComponents(pixels, 0, BMPComponentMapping, bitmap.Data, 0, ComponentMapping.RGBA, bitmap.Width, bitmap.Height, false, true);
            return header.ToBytes().Add(pixels);
        }
    }

    public struct ComponentMapping {

        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public static readonly ComponentMapping RGBA = new ComponentMapping() { R = 0, G = 1, B = 2, A = 3 };
        public static readonly ComponentMapping BGRA = new ComponentMapping() { B = 0, G = 1, R = 2, A = 3 };
    }

    internal struct BitmapLoadingInfo {

        public int Width;
        public int Height;
        public byte[] SrcData;
        public bool xFlip;
        public bool yFlip;
        public ComponentMapping SrcComponentMapping;
    }

    public class Bitmap {

        public static ComponentMapping BitmapComponentMapping = ComponentMapping.RGBA;

        public static void RemapComponents(byte[] dest, int destOffset, ComponentMapping destComponentMapping, byte[] src, int srcOffset, ComponentMapping srcComponentMapping, int width, int height, bool xFlip, bool yFlip) {
            for(int i = 0; i < width * height; i++, srcOffset += 4) {
                int x = i % width;
                if(xFlip) x = width - 1 - x;
                int y = i / width;
                if(yFlip) y = height - 1 - y;
                int pixelOffset = (x + y * width) * 4 + destOffset;
                dest[pixelOffset + destComponentMapping.R] = src[srcOffset + srcComponentMapping.R];
                dest[pixelOffset + destComponentMapping.G] = src[srcOffset + srcComponentMapping.G];
                dest[pixelOffset + destComponentMapping.B] = src[srcOffset + srcComponentMapping.B];
                dest[pixelOffset + destComponentMapping.A] = src[srcOffset + srcComponentMapping.A];
            }
        }

        public int Width;
        public int Height;
        public byte[] Data;

        public Bitmap(string file) {
            string extension = Path.GetExtension(file);
            byte[] data = File.ReadAllBytes(file);

            BitmapLoadingInfo info = default;
            switch(extension) {
                case ".bmp": {
                    info = BMP.Load(data);
                } break;
                default:
                    Debug.Assert(false, "Tried to load an unsupported image file format: {0}", extension);
                    break;
            }

            InitializeBitmap(info.Width, info.Height, info.SrcData, info.xFlip, info.yFlip, info.SrcComponentMapping);
        }

        public Bitmap(int width, int height) : this(width, height, new byte[width * height * 4]) { }

        public Bitmap(int width, int height, byte[] data, bool xFlip = false, bool yFlip = false) : this(width, height, data, xFlip, yFlip, BitmapComponentMapping) { }

        public Bitmap(int width, int height, byte[] data, bool xFlip, bool yFlip, ComponentMapping componentMapping) {
            InitializeBitmap(width, height, data, xFlip, yFlip, componentMapping);
        }

        private void InitializeBitmap(int width, int height, byte[] srcData, bool xFlip, bool yFlip, ComponentMapping srcComponentMapping) {
            Width = width;
            Height = height;
            Data = new byte[width * height * 4];
            RemapComponents(Data, 0, BitmapComponentMapping, srcData, 0, srcComponentMapping, width, height, xFlip, yFlip);
        }

        public void Save(string file) {
            string extension = Path.GetExtension(file);

            byte[] fileContents = null;
            switch(extension) {
                case ".bmp": {
                    fileContents = BMP.Write(this);
                }
                break;
                default:
                    Debug.Assert(false, "Tried to write in an unsupported image file format: {0}", extension);
                    break;
            }

            Debug.Assert(fileContents != null, "Error writing {0} file!", extension);
            File.WriteAllBytes(file, fileContents);
        }

        public void Clear(byte shade) {
            Array.Fill(Data, shade);
        }

        public void CopyPixel(int destX, int destY, int srcX, int srcY, Bitmap src) {
            int destIndex = (destX + destY * Width) * 4;
            int srcIndex = (srcX + srcY * src.Width) * 4;
            Data[destIndex] = src.Data[srcIndex];
            Data[destIndex + 1] = src.Data[srcIndex + 1];
            Data[destIndex + 2] = src.Data[srcIndex + 2];
            Data[destIndex + 3] = src.Data[srcIndex + 3];
        }

        public void SetPixel(int x, int y, byte r, byte g, byte b, byte a) {
            int destIndex = (x + y * Width) * 4;
            Data[destIndex] = r;
            Data[destIndex + 1] = g;
            Data[destIndex + 2] = b;
            Data[destIndex + 3] = a;
        }

        public void DrawRect(int xOffset, int yOffset, int width, int height, byte r, byte g, byte b, byte a) {
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    SetPixel(xOffset + x , yOffset + y, r, g, b, a);
                }
            }
        }

        public void CopyBitmap(int destX, int destY, int srcX, int srcY, Bitmap src) {
            for(int x = 0; x < src.Width; x++) {
                for(int y = 0; y < src.Height; y++) {
                    CopyPixel(destX + x, destY + y, srcX + x, srcY + y, src);
                }
            }
        }

        public Bitmap SubImage(int xStart, int yStart, int bitmapWidth, int bitmapHeight) {
            return SubImage(new Bitmap(bitmapWidth, bitmapHeight), xStart, yStart);
        }

        public Bitmap SubImage(Bitmap dest, int xStart, int yStart) {
            for(int x = 0; x < dest.Width; x++) {
                for(int y = 0; y < dest.Height; y++) {
                    dest.CopyPixel(x, y, x + xStart, y + yStart, this);
                }
            }

            return dest;
        }
    }
}
