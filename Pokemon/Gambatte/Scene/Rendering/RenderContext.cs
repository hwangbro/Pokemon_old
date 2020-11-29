using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static Pokemon.Windows;

/*
 * Rendering Routine Philosophy:
 *  - RenderContext is static, therefore can be used anywhere without having to pass a reference.
 *  - The RenderContext holds a reference to a RenderDevice, which will handle all the Graphics-API specific calls, and therefore by replacing the RenderDevice it should be easy to support multiple Graphics APIs.
 *  - "Render Object" (Meshes, Textures, etc.) are created through the RenderContext so that the RenderDevice can be kept internal to the RenderContext.
 *
 */

namespace Pokemon {

    public delegate void RenderPassCallback(uint renderTarget);

    public class VertexArray {

        public uint Handle;
        public RenderDevice Device;

        internal VertexArray(RenderDevice device, float[][] data, int[] dataSizes, uint[] indices, BufferUsage usage) {
            Handle = device.CreateVertexArray(data, dataSizes, indices, usage);
            Device = device;
        }

        public void UpdateVertexBuffer(uint bufferIndex, float[] data) {
            Device.UpdateVertexArrayBuffer(Handle, bufferIndex, data);
        }

        public static implicit operator uint(VertexArray vao) { return vao.Handle; }
    }

    public class Texture2D {

        public uint Handle;
        public RenderDevice Device;

        public int Width;
        public int Height;

        internal Texture2D(RenderDevice device, int width, int height, byte[] data, PixelFormat internalFormat) {
            Handle = device.CreateTexture2D(width, height, data, PixelFormat.RGBA, internalFormat);
            Device = device;
            Width = width;
            Height = height;
        }

        public byte[] ReadContents() {
            byte[] destBuffer = new byte[Width * Height * 4];
            ReadContents(destBuffer, PixelFormat.RGBA);
            return destBuffer;
        }

        public void ReadContents(byte[] dest, PixelFormat format) {
            Device.ReadTexture2DContents(Handle, dest, format);
        }

        public void UpdateContents(byte[] pixels, PixelFormat format) {
            Device.UpdateTexture2DContents(Handle, 0, 0, Width, Height, pixels, format);
        }

        public static implicit operator uint(Texture2D texture) { return texture.Handle; }
    }

    public class Font : Dictionary<char, Texture2D> {

        public Font(Bitmap bitmap, int charWidth, int charHeight, string chars) {
            int charCount = 0;
            for(int y = 0; y < bitmap.Height; y += charHeight) {
                for(int x = 0; x < bitmap.Width; x += charWidth) {
                    char currentChar = chars[charCount++];
                    if(!ContainsKey(currentChar)) {
                        this[currentChar] = RenderContext.CreateTexture2D(bitmap.SubImage(x, y, charWidth, charHeight));
                    }
                }
            }
        }
    }

    public class PixelBuffer {

        public uint Handle;
        public RenderDevice Device;

        internal PixelBuffer(RenderDevice device, int size, BufferUsage usage) {
            Handle = device.CreatePixelBuffer(size, usage);
            Device = device;
        }

        public void ReadTextureContents(Texture2D texture, PixelFormat format, byte[] dest) {
            Device.ReadTexture2DContents(Handle, texture, dest, format);
        }

        public static implicit operator uint(PixelBuffer pixelBuffer) { return pixelBuffer.Handle; }
    }

    public class Shader {

        public uint Handle;
        public RenderDevice Device;

        internal Shader(RenderDevice device, string code) {
            Handle = device.CreateShader(code);
            Device = device;
        }

        public void SetUniform(string name, float v0) {
            Device.SetUniform(Handle, name, v0);
        }

        public void SetUniform(string name, float v0, float v1) {
            Device.SetUniform(Handle, name, v0, v1);
        }

        public void SetUniform(string name, float v0, float v1, float v2) {
            Device.SetUniform(Handle, name, v0, v1, v2);
        }

        public void SetUniform(string name, float v0, float v1, float v2, float v3) {
            Device.SetUniform(Handle, name, v0, v1, v2, v3);
        }

        public void SetUniform(string name, int v0) {
            Device.SetUniform(Handle, name, v0);
        }

        public void SetUniform(string name, int v0, int v1) {
            Device.SetUniform(Handle, name, v0, v1);
        }

        public void SetUniform(string name, int v0, int v1, int v2) {
            Device.SetUniform(Handle, name, v0, v1, v2);
        }

        public void SetUniform(string name, int v0, int v1, int v2, int v3) {
            Device.SetUniform(Handle, name, v0, v1, v2, v3);
        }

        public void SetUniform(string name, bool transpose, float[] mat) {
            Device.SetUniformMat4(Handle, name, transpose, mat);
        }

        public static implicit operator uint(Shader shader) { return shader.Handle; }
    }

    public class RenderTarget {

        public uint Handle;
        public RenderDevice Device;

        public RenderTarget(RenderDevice device, Texture2D texture, int width, int height, FramebufferAttachment attachment, uint attachmentNumber) {
            Handle = device.CreateRenderTarget(texture, width, height, attachment, attachmentNumber);
            Device = device;
        }

        public static implicit operator uint(RenderTarget renderTarget) { return renderTarget.Handle; }
    }

    public class RenderPass {

        public Texture2D Texture;
        public RenderTarget RenderTarget;
        public RenderPassCallback Callback;

        public RenderPass(RenderDevice device, int width, int height, RenderPassCallback callback) {
            Texture = new Texture2D(device, width, height, null, PixelFormat.RGB);
            RenderTarget = new RenderTarget(device, Texture, width, height, FramebufferAttachment.Color, 0);
            Callback = callback;
        }
    }

    public static class RenderContext {

        private static RenderDevice Device;
        public static int ScreenBufferWidth;
        public static int ScreenBufferHeight;
        public static byte[] OffscreenBuffer;
        public static Dictionary<string, RenderPass> RenderPasses;
        public static DrawParams DrawParams;

        private static VertexArray QuadVAO;
        private static VertexArray FullscreenQuadVAO;
        public static Mat4 Projection;

        public static Shader Shader;
        public static Shader BlurFilter;
        public static Shader GrayscaleFliter;

        public static Font Font;

        public static void Create(IntPtr window, bool vsync) {
            ScreenBufferWidth = Window.GetWidth(window);
            ScreenBufferHeight = Window.GetHeight(window);
            OffscreenBuffer = new byte[ScreenBufferWidth * ScreenBufferHeight * 4];

            IntPtr dc = GetDC(window);
            Device = new RenderDevice(dc, ScreenBufferWidth, ScreenBufferHeight, vsync);
            ReleaseDC(window, dc);

            RenderPasses = new Dictionary<string, RenderPass>();
            DrawParams = new DrawParams() {
                PrimitiveType = PrimitiveType.Triangles,
                FaceCulling = FaceCulling.None,
                DepthFunc = DrawFunc.Always,
                ShouldWriteDepth = false,
                SourceBlend = BlendFunc.None,
                DestBlend = BlendFunc.None,
            };

            QuadVAO = CreateVertexArray(new float[][] { new float[] { 0.0f, 0.0f,
                                                                      0.0f, 1.0f,
                                                                      1.0f, 1.0f,
                                                                      1.0f, 0.0f,}, new float[] { 0.0f, 0.0f,
                                                                                                  0.0f, 1.0f,
                                                                                                  1.0f, 1.0f,
                                                                                                  1.0f, 0.0f,} }, new int[] { 2, 2, }, new uint[] { 0, 1, 2, 2, 3, 0 }, BufferUsage.StaticDraw);

            FullscreenQuadVAO = CreateVertexArray(new float[][] { new float[] { 0.0f, 0.0f,
                                                                                0.0f, 1.0f,
                                                                                1.0f, 1.0f,
                                                                                1.0f, 0.0f,}, new float[] { 0.0f, 1.0f,
                                                                                                            0.0f, 0.0f,
                                                                                                            1.0f, 0.0f,
                                                                                                            1.0f, 1.0f,} }, new int[] { 2, 2, }, new uint[] { 0, 1, 2, 2, 3, 0 }, BufferUsage.StaticDraw);


            Projection = Mat4.Orthographic(0, ScreenBufferWidth, ScreenBufferHeight, 0, -1000.0f, 1000.0f);

            Shader = CreateShader("assets/shaders/shader.glsl");
            BlurFilter = CreateShader("assets/shaders/blur.glsl");
            GrayscaleFliter = CreateShader("assets/shaders/grayscale.glsl");

            Font = new Font(new Bitmap("assets/textures/monospace.bmp"), 8, 8, "ABCDEFGHIJKLMNOP" +
                                                                               "QRSTUVWXYZ():;[]" +
                                                                               "abcdefghijklmnop" +
                                                                               "qrstuvwxyz______" +
                                                                               "                " +
                                                                               "                " +
                                                                               "___-__?!._______" +
                                                                               "_*./,_0123456789");
        }

        public static RenderPass RegisterRenderPass(string name, RenderPassCallback callback) {
            RenderPass renderPass = new RenderPass(Device, ScreenBufferWidth, ScreenBufferHeight, callback);
            RenderPasses[name] = renderPass;
            return renderPass;
        }

        public static void Clear(uint renderTarget, bool shouldClearColor, bool shouldClearDepth, float r, float g, float b, float a) {
            Device.Clear(renderTarget, shouldClearColor, shouldClearDepth, r, g, b, a);
        }

        public static VertexArray CreateVertexArray(float[][] data, int[] dataSizes, uint[] indices, BufferUsage usage = BufferUsage.StaticDraw) {
            return new VertexArray(Device, data, dataSizes, indices, usage);
        }

        public static Texture2D CreateTexture2D(int width, int height, byte[] data, PixelFormat internalFormat = PixelFormat.RGBA) {
            return new Texture2D(Device, width, height, data, internalFormat);
        }

        public static Texture2D CreateTexture2D(string fileName) {
            return CreateTexture2D(new Bitmap(fileName));
        }

        public static Texture2D CreateTexture2D(Bitmap bitmap) {
            return new Texture2D(Device, bitmap.Width, bitmap.Height, bitmap.Data, PixelFormat.RGBA);
        }

        public static PixelBuffer CreatePixelBuffer(int size, BufferUsage usage = BufferUsage.StreamCopy) {
            return new PixelBuffer(Device, size, usage);
        }

        public static Shader CreateShaderFromSource(string code) {
            return new Shader(Device, code);
        }

        public static Shader CreateShader(string fileName) {
            return new Shader(Device, File.ReadAllText(fileName));
        }

        public static Shader CreateShaderFromCode(string code) {
            return new Shader(Device, code);
        }

        public static RenderTarget CreateRenderTarget(Texture2D texture, int width, int height, FramebufferAttachment attachment, uint attachmentNumber = 0) {
            return new RenderTarget(Device, texture, width, height, attachment, attachmentNumber);
        }

        public static void Render() {
            for(int i = 0; i < RenderPasses.Count; i++) {
                RenderPass renderPass = RenderPasses.ElementAt(i).Value;
                renderPass.Callback(i == RenderPasses.Count - 1 ? 0 : renderPass.RenderTarget.Handle);
            }
        }

        public static void DrawQuad(uint renderTarget, Texture2D texture, float x, float y, float z, float width, float height, float r = 1.0f, float g = 1.0f, float b = 1.0f, float a = 1.0f) {
            DrawInternal(renderTarget, Shader, texture, QuadVAO, x, y, z, width, height, r, g, b, a);
        }

        public static void DrawString(uint renderTarget, Font font, string text, float x, float y, float z, float charWidth, float charHeight, float r = 1.0f, float g = 1.0f, float b = 1.0f, float a = 1.0f) {
            float xOffset = 0.0f;
            float yOffset = 0.0f;
            for(int i = 0; i < text.Length; i++) {
                char c = text[i];
                switch(c) {
                    case '\n': {
                        xOffset = 0.0f;
                        yOffset += charHeight;
                    } break;
                    default: {
                        DrawQuad(renderTarget, font[c], x + xOffset, y + yOffset, z, charWidth, charHeight, r, g, b, a);
                        xOffset += charWidth;
                    } break;
                }
            }
        }

        private static void DrawInternal(uint renderTarget, Shader shader, Texture2D texture, VertexArray vao, float x, float y, float z, float width, float height, float r, float g, float b, float a) {
            Device.SetShader(shader);
            Device.SetTexture2D(texture, "diffuse");
            Mat4 transform = Mat4.Translate(new Vec3(x, y, z)) * Mat4.Scale(new Vec3(width, height, 1.0f));
            Mat4 mvp = Projection * transform;
            shader.SetUniform("MVP", false, mvp.ToArray());
            shader.SetUniform("color", r, g, b, a);
            Device.Draw(renderTarget, shader, vao, DrawParams);
        }

        public static void DrawFullscreenQuad(uint renderTarget, Shader shader, Texture2D texture) {
            Device.SetShader(shader);
            Device.SetTexture2D(texture, "diffuse");
            Mat4 transform = Mat4.Translate(new Vec3(0, 0, 0)) * Mat4.Scale(new Vec3(ScreenBufferWidth, ScreenBufferHeight, 1.0f));
            Mat4 mvp = Projection * transform;
            shader.SetUniform("MVP", false, mvp.ToArray());
            shader.SetUniform("color", 1.0f, 1.0f, 1.0f, 1.0f);
            Device.Draw(renderTarget, shader, FullscreenQuadVAO, DrawParams);
        }

        public static void CopyToOffscreenBuffer(PixelFormat format) {
            Device.ReadPixels(0, 0, ScreenBufferWidth, ScreenBufferHeight, format, OffscreenBuffer);
        }
    }
}
