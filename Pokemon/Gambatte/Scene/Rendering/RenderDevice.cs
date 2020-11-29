using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using static Pokemon.OpenGL;
using static Pokemon.Windows;

namespace Pokemon {

    public enum BufferUsage : uint {

        StaticDraw = GL_STATIC_DRAW,
        DynamicDraw = GL_DYNAMIC_DRAW,
        StreamDraw = GL_STREAM_DRAW,
        StaticCopy = GL_STATIC_COPY,
        DynamicCopy = GL_DYNAMIC_COPY,
        StreamCopy = GL_STREAM_COPY,
        StaticRead = GL_STATIC_READ,
        DynamicRead = GL_DYNAMIC_READ,
        StreamRead = GL_STREAM_READ,
    }

    public enum PixelFormat : uint {

        R = GL_RED,
        RG = GL_RG,
        RGB = GL_RGB,
        BGR = GL_BGR,
        RGBA = GL_RGBA,
        BGRA = GL_BGRA,
        Depth = GL_DEPTH_COMPONENT,
    };

    public enum FramebufferAttachment : uint {

        Color = GL_COLOR_ATTACHMENT0,
        Depth = GL_DEPTH_ATTACHMENT,
    }

    public enum PrimitiveType : uint {

        Triangles = GL_TRIANGLES,
        Points = GL_POINTS,
        LineStrip = GL_LINE_STRIP,
        LineLoop = GL_LINE_LOOP,
        Lines = GL_LINES,
        LineStripAdjacency = GL_LINE_STRIP_ADJACENCY,
        TriangleStrip = GL_TRIANGLE_STRIP,
        TriangleFan = GL_TRIANGLE_FAN,
        TriangleStripAdjacency = GL_TRIANGLE_STRIP_ADJACENCY,
        TrianglesAdjacency = GL_TRIANGLES_ADJACENCY,
        Patches = GL_PATCHES,
    }

    public enum FaceCulling : uint {

        None,
        Back = GL_BACK,
        Front = GL_FRONT,
        FrontAndBack = GL_FRONT_AND_BACK,
    }

    public enum DrawFunc : uint {

        Never = GL_NEVER,
        Always = GL_ALWAYS,
        Less = GL_LESS,
        Greater = GL_GREATER,
        Lequal = GL_LEQUAL,
        Gequal = GL_GEQUAL,
        Equal = GL_EQUAL,
        NotEqual = GL_NOTEQUAL,
    }

    public enum BlendFunc : uint {

        None,
        One = GL_ONE,
        SrcAlpha = GL_SRC_ALPHA,
        DstAlpha = GL_DST_ALPHA,
        OneMinusSrcAlpha = GL_ONE_MINUS_SRC_ALPHA,
        OneMinusDstAlpha = GL_ONE_MINUS_DST_ALPHA,
    }

    public struct OpenGLInfo {

        public string Vendor;
        public string Renderer;
        public string Version;
        public string ShadingLanguageVersion;
        public string Extensions;
    }

    public struct VertexArrayStruct {

        public uint[] Buffers;
        public int[] BufferSizes;
        public int NumElements;
        public BufferUsage Usage;
    }

    public struct Texture2DStruct {

        public int Width;
        public int Height;
    }

    public struct RenderTargetStruct {

        public int Width;
        public int Height;
    }

    public struct ShaderStruct {

        public uint[] Shaders;
        public Dictionary<string, int> UniformMap;
        public Dictionary<string, int> SamplerMap;
    }

    public struct PixelBufferStruct {

        public int Size;
    }

    public struct DrawParams {

        public PrimitiveType PrimitiveType;
        public FaceCulling FaceCulling;
        public DrawFunc DepthFunc;
        public bool ShouldWriteDepth;
        public BlendFunc SourceBlend;
        public BlendFunc DestBlend;
    }

    public class RenderDevice {

        private static readonly uint[] WGL_CONTEXT_ATTRIBS = {
            WGL_CONTEXT_MAJOR_VERSION_ARB, 3,
            WGL_CONTEXT_MINOR_VERSION_ARB, 0,
            WGL_CONTEXT_FLAGS_ARB, 0,
            WGL_CONTEXT_PROFILE_MASK_ARB, WGL_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB,
            0,
        };

        private Dictionary<uint, VertexArrayStruct> VAOMap;
        private Dictionary<uint, Texture2DStruct> TextureMap;
        private Dictionary<uint, RenderTargetStruct> RenderTargetMap;
        private Dictionary<uint, ShaderStruct> ShaderMap;
        private Dictionary<uint, PixelBufferStruct> PixelBufferMap;

        private float CurrentClearColorR;
        private float CurrentClearColorG;
        private float CurrentClearColorB;
        private float CurrentClearColorA;

        private BlendFunc CurrentSourceBlend;
        private BlendFunc CurrentDestBlend;

        private FaceCulling CurrentFaceCulling;

        private bool ShouldWriteDepth;
        private DrawFunc CurrentDepthFunc;

        private uint BoundVAO;
        private uint BoundTexture;
        private uint BoundRenderTarget;
        private uint BoundShader;
        private uint BoundPixelBuffer;

        public RenderDevice(IntPtr windowDC, int windowWidth, int windowHeight, bool vsync) {
            CreateContext(windowDC, vsync);

            CurrentSourceBlend = BlendFunc.None;
            CurrentDestBlend = BlendFunc.None;
            CurrentFaceCulling = FaceCulling.None;
            ShouldWriteDepth = false;
            CurrentDepthFunc = DrawFunc.Always;

            VAOMap = new Dictionary<uint, VertexArrayStruct>();
            TextureMap = new Dictionary<uint, Texture2DStruct>();
            RenderTargetMap = new Dictionary<uint, RenderTargetStruct>();
            ShaderMap = new Dictionary<uint, ShaderStruct>();
            PixelBufferMap = new Dictionary<uint, PixelBufferStruct>();

            RenderTargetMap[0] = new RenderTargetStruct() {
                Width = windowWidth,
                Height = windowHeight,
            };

            glEnable(GL_DEPTH_TEST);
            glFrontFace(GL_CCW);
        }

        private static IntPtr CreateContext(IntPtr windowDC, bool vsync) {
            LoadWGLExtensions();

            IntPtr openglRC;
            if(OpenGL.wglCreateContextAttribsARB != null) {
                SetPixelFormat(windowDC);
                openglRC = wglCreateContextAttribsARB(windowDC, IntPtr.Zero, WGL_CONTEXT_ATTRIBS);
            } else {
                openglRC = wglCreateContext(windowDC);
            }

            if(wglMakeCurrent(windowDC, openglRC) != 0) {
                if(OpenGL.wglSwapIntervalEXT != null) wglSwapIntervalEXT(vsync ? 1 : 0);
                LoadGLExtensions();
            }

            return openglRC;
        }

        private static void SetPixelFormat(IntPtr windowDC) {
            int suggestedPixelFormatIndex = 0;
            uint extendedPick = 0;
            if(OpenGL.wglChoosePixelFormatARB != null) {
                int[] intAttribList = {
                    WGL_DRAW_TO_WINDOW_ARB, GL_TRUE,
                    WGL_ACCELERATION_ARB, WGL_FULL_ACCELERATION_ARB,
                    WGL_SUPPORT_OPENGL_ARB, GL_TRUE,
                    WGL_DOUBLE_BUFFER_ARB, GL_TRUE,
                    WGL_PIXEL_TYPE_ARB, WGL_TYPE_RGBA_ARB,
                    WGL_RED_BITS_ARB, 8,
                    WGL_GREEN_BITS_ARB, 8,
                    WGL_BLUE_BITS_ARB, 8,
                    WGL_ALPHA_BITS_ARB, 8,
                    WGL_DEPTH_BITS_ARB, 24,
                    0,
                };

                float[] floatAttribList = { };

                wglChoosePixelFormatARB(windowDC, intAttribList, floatAttribList, 1, out suggestedPixelFormatIndex, out extendedPick);
            }

            if(extendedPick == 0) {
                PIXELFORMATDESCRIPTOR desiredPixelFormat = new PIXELFORMATDESCRIPTOR(){ };
                desiredPixelFormat.nSize = (ushort) Marshal.SizeOf<PIXELFORMATDESCRIPTOR>();
                desiredPixelFormat.nVersion = 1;
                desiredPixelFormat.dwFlags = PFD_SUPPORT_OPENGL | PFD_DRAW_TO_WINDOW | PFD_DOUBLEBUFFER;
                desiredPixelFormat.cColorBits = 32;
                desiredPixelFormat.cAlphaBits = 8;
                desiredPixelFormat.cDepthBits = 24;
                desiredPixelFormat.iLayerType = (byte) PFD_MAIN_PLANE;

                suggestedPixelFormatIndex = ChoosePixelFormat(windowDC, ref desiredPixelFormat);
            }

            DescribePixelFormat(windowDC, suggestedPixelFormatIndex, (uint) Marshal.SizeOf<PIXELFORMATDESCRIPTOR>(), out PIXELFORMATDESCRIPTOR suggestedPixelFormat);
            Windows.SetPixelFormat(windowDC, suggestedPixelFormatIndex, ref suggestedPixelFormat);
        }

        private static void LoadWGLExtensions() {
            IntPtr instance = GetModuleHandleA(null);

            WNDCLASS windowClass = new WNDCLASS() {};
            windowClass.lpfnWndProc = DefWindowProcA;
            windowClass.hInstance = instance;
            windowClass.lpszClassName = "PokemonWGLLoader";
            if(RegisterClassA(ref windowClass) != 0) {
                IntPtr windowHandle = CreateWindowExA(0, windowClass.lpszClassName, "", 0, 50, 50, 800, 600, IntPtr.Zero, IntPtr.Zero, instance, IntPtr.Zero);
                if(windowHandle != IntPtr.Zero) {
                    IntPtr windowDC = GetDC(windowHandle);
                    SetPixelFormat(windowDC);
                    IntPtr openglRC = wglCreateContext(windowDC);
                    if(wglMakeCurrent(windowDC, openglRC) != 0) {
                        IntPtr fnPointer;
                        if((fnPointer = wglGetProcAddress("wglChoosePixelFormatARB")) != IntPtr.Zero) OpenGL.wglChoosePixelFormatARB = Marshal.GetDelegateForFunctionPointer<wglChoosePixelFormatARB>(fnPointer);
                        if((fnPointer = wglGetProcAddress("wglCreateContextAttribsARB")) != IntPtr.Zero) OpenGL.wglCreateContextAttribsARB = Marshal.GetDelegateForFunctionPointer<wglCreateContextAttribsARB>(fnPointer);
                        if((fnPointer = wglGetProcAddress("wglSwapIntervalEXT")) != IntPtr.Zero) OpenGL.wglSwapIntervalEXT = Marshal.GetDelegateForFunctionPointer<wglSwapIntervalEXT>(fnPointer);

                        SetPixelFormat(windowDC);
                        if(OpenGL.wglCreateContextAttribsARB != null) {
                            openglRC = wglCreateContextAttribsARB(windowDC, IntPtr.Zero, WGL_CONTEXT_ATTRIBS);
                        }
                        wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
                    }
                    wglDeleteContext(openglRC);
                    ReleaseDC(windowHandle, windowDC);
                    DestroyWindow(windowHandle);
                }
            }
        }

        public OpenGLInfo GetOpenGLInfo() {
            return new OpenGLInfo() {
                Vendor = glGetString(GL_VENDOR),
                Renderer = glGetString(GL_RENDERER),
                Version = glGetString(GL_VERSION),
                ShadingLanguageVersion = glGetString(GL_SHADING_LANGUAGE_VERSION),
                Extensions = glGetString(GL_EXTENSIONS),
            };
        }

        public void Draw(uint fbo, uint shader, uint vao, DrawParams drawParams) {
            SetRenderTarget(fbo);
            SetShader(shader);
            SetBlending(drawParams.SourceBlend, drawParams.DestBlend);
            SetFaceCulling(drawParams.FaceCulling);
            SetDepthTest(drawParams.ShouldWriteDepth, drawParams.DepthFunc);
            SetVertexArray(vao);
            glDrawElements((uint) drawParams.PrimitiveType, VAOMap[vao].NumElements, GL_UNSIGNED_INT, 0);
        }

        public void ReadPixels(int x, int y, int width, int height, PixelFormat format, byte[] dest) {
            glReadPixels(x, y, width, height, (uint) format, GL_UNSIGNED_BYTE, dest);
        }

        public void Clear(uint fbo, bool shouldClearColor, bool shouldClearDepth, float r, float b, float g, float a) {
            uint flags = 0;
            if(shouldClearColor) {
                flags |= GL_COLOR_BUFFER_BIT;
                SetClearColor(r, g, b, a);
            }

            if(shouldClearDepth) {
                flags |= GL_DEPTH_BUFFER_BIT;
            }

            SetRenderTarget(fbo);
            glClear(flags);
        }

        public void SetClearColor(float r, float g, float b, float a) {
            if(CurrentClearColorR != r || CurrentClearColorG != g || CurrentClearColorB != b || CurrentClearColorA != a) {
                glClearColor(r, g, b, a);
                CurrentClearColorR = r;
                CurrentClearColorG = g;
                CurrentClearColorB = b;
                CurrentClearColorA = a;
            }
        }

        public void SetBlending(BlendFunc sourceBlend, BlendFunc destBlend) {
            if(sourceBlend == CurrentSourceBlend && destBlend == CurrentDestBlend) {
                return;
            } else if(sourceBlend == BlendFunc.None || destBlend == BlendFunc.None) {
                glDisable(GL_BLEND);
            } else {
                if(CurrentSourceBlend == BlendFunc.None || CurrentDestBlend == BlendFunc.None) {
                    glEnable(GL_BLEND);
                }
                glBlendFunc((uint) sourceBlend, (uint) destBlend);
            }

            CurrentSourceBlend = sourceBlend;
            CurrentDestBlend = destBlend;
        }

        public void SetFaceCulling(FaceCulling cullingMode) {
            if(cullingMode == CurrentFaceCulling) {
                return;
            } else if(cullingMode == FaceCulling.None) {
                glDisable(GL_CULL_FACE);
            } else {
                if(CurrentFaceCulling == FaceCulling.None) {
                    glEnable(GL_CULL_FACE);
                }
                glCullFace((uint) cullingMode);
            }

            CurrentFaceCulling = cullingMode;
        }

        public void SetDepthTest(bool shouldWrite, DrawFunc depthFunc) {
            if(shouldWrite != ShouldWriteDepth) {
                glDepthMask(shouldWrite ? GL_TRUE : GL_FALSE);
                ShouldWriteDepth = shouldWrite;
            }

            if(depthFunc == CurrentDepthFunc) {
                return;
            }

            glDepthFunc((uint) depthFunc);
            CurrentDepthFunc = depthFunc;
        }

        public uint CreateVertexArray(float[][] vertexData, int[] vertexElementSizes, uint[] indices, BufferUsage usage) {
            int numBuffers = vertexData.Length + 1;
            int numVerticies = vertexData[0].Length / vertexElementSizes[0];

            uint[] buffers = new uint[numBuffers];
            int[] bufferSizes = new int[numBuffers];

            glGenVertexArrays(out uint vao);
            SetVertexArray(vao);

            glGenBuffers(buffers);
            for(uint i = 0; i < numBuffers - 1; i++) {
                int elementSize = vertexElementSizes[i];
                float[] bufferData = vertexData[i];
                int dataSize = elementSize * sizeof(float) * numVerticies;
                glBindBuffer(GL_ARRAY_BUFFER, buffers[i]);
                glBufferData(GL_ARRAY_BUFFER, dataSize, bufferData, (uint) usage);

                glEnableVertexAttribArray(i);
                glVertexAttribPointer(i, elementSize, GL_FLOAT, GL_FALSE, 0, 0);
            }

            int indicesSize = indices.Length * sizeof(uint);
            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, buffers[numBuffers - 1]);
            glBufferData(GL_ELEMENT_ARRAY_BUFFER, indicesSize, indices, (uint) usage);
            bufferSizes[numBuffers - 1] = indicesSize;

            VAOMap[vao] = new VertexArrayStruct() {
                Buffers = buffers,
                BufferSizes = bufferSizes,
                NumElements = indices.Length,
                Usage = usage,
            };

            return vao;
        }

        public uint ReleaseVertexArray(uint vao) {
            if(vao == 0 || !VAOMap.ContainsKey(vao)) return 0;

            VertexArrayStruct vaoData = VAOMap[vao];
            glDeleteVertexArrays(vao);
            glDeleteBuffers(vaoData.Buffers);
            VAOMap.Remove(vao);
            return 0;
        }

        public void SetVertexArray(uint vao) {
            if(BoundVAO == vao) {
                return;
            }

            glBindVertexArray(vao);
            BoundVAO = vao;
        }

        public VertexArrayStruct GetVertexArray(uint vao) {
            return VAOMap[vao];
        }

        public void UpdateVertexArrayBuffer(uint vao, uint bufferIndex, float[] data) {
            if(vao == 0 || !VAOMap.ContainsKey(vao)) return;

            VertexArrayStruct vaoData = GetVertexArray(vao);
            int dataSize = data.Length * sizeof(float);

            SetVertexArray(vao);
            glBindBuffer(GL_ARRAY_BUFFER, vaoData.Buffers[bufferIndex]);
            if(vaoData.BufferSizes[bufferIndex] >= dataSize) {
                glBufferSubData(GL_ARRAY_BUFFER, 0, dataSize, data);
            } else {
                glBufferData(GL_ARRAY_BUFFER, dataSize, data, (uint) vaoData.Usage);
                vaoData.BufferSizes[bufferIndex] = dataSize;
            }
        }

        public uint CreateTexture2D(int width, int height, byte[] data, PixelFormat dataFormat, PixelFormat internalFormat) {
            glGenTextures(out uint textureHandle);
            SetTexture2D(textureHandle);
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
            glTexParameterf(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
            glTexImage2D(GL_TEXTURE_2D, 0, (uint) internalFormat, width, height, 0, (uint) dataFormat, GL_UNSIGNED_BYTE, data);
            TextureMap[textureHandle] = new Texture2DStruct() {
                Width = width,
                Height = height,
            };

            return textureHandle;
        }

        public uint ReleaseTexture2D(uint textureHandle) {
            if(textureHandle == 0 || !TextureMap.ContainsKey(textureHandle)) return 0;

            glDeleteTextures(textureHandle);
            TextureMap.Remove(textureHandle);
            return 0;
        }

        public void SetTexture2D(uint textureHandle, uint slot = 0) {
            if(BoundTexture == textureHandle) {
                return;
            }

            glActiveTexture(GL_TEXTURE0 + slot);
            glBindTexture(GL_TEXTURE_2D, textureHandle);
            BoundTexture = textureHandle;
        }

        public void SetTexture2D(uint textureHandle, string uniformName) {
            SetTexture2D(textureHandle, (uint) ShaderMap[BoundShader].SamplerMap[uniformName]);
        }

        public Texture2DStruct GetTexture2D(uint textureHandle) {
            return TextureMap[textureHandle];
        }

        public void ReadTexture2DContents(uint textureHandle, byte[] dest, PixelFormat format) {
            SetTexture2D(textureHandle);
            SetPixelBuffer(0);
            glGetTexImage(GL_TEXTURE_2D, 0, (uint) format, GL_UNSIGNED_BYTE, dest);
        }

        public void UpdateTexture2DContents(uint textureHandle, int xoffset, int yoffset, int width, int height, byte[] pixels, PixelFormat format = PixelFormat.RGBA) {
            SetTexture2D(textureHandle);
            glTexSubImage2D(GL_TEXTURE_2D, 0, xoffset, yoffset, width, height, (uint) format, GL_UNSIGNED_BYTE, pixels);
        }

        public uint CreateRenderTarget(uint texture, int width, int height, FramebufferAttachment attachment, uint attachmentNumber = 0) {
            glGenFramebuffers(out uint fbo);
            SetRenderTarget(fbo, false);
            glFramebufferTexture2D(GL_FRAMEBUFFER, (uint) attachment + attachmentNumber, GL_TEXTURE_2D, texture, 0);

            if(attachment == FramebufferAttachment.Color) {
                glGenRenderbuffers(out uint depthbuffer);
                glBindRenderbuffer(GL_RENDERBUFFER, depthbuffer);
                glRenderbufferStorage(GL_RENDERBUFFER, GL_DEPTH_COMPONENT24, width, height);
                glBindRenderbuffer(GL_RENDERBUFFER, 0);
                glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, depthbuffer);
            }

            RenderTargetMap[fbo] = new RenderTargetStruct() {
                Width = width,
                Height = height,
            };

            return fbo;
        }

        public uint ReleaseRenderTarget(uint fbo) {
            if(fbo == 0 || !RenderTargetMap.ContainsKey(fbo)) return 0;

            glDeleteFramebuffers(fbo);
            RenderTargetMap.Remove(fbo);
            return 0;
        }

        public void SetRenderTarget(uint fbo, bool resizeViewport = true) {
            if(BoundRenderTarget == fbo) {
                return;
            }

            glBindFramebuffer(GL_FRAMEBUFFER, fbo);
            if(resizeViewport) {
                RenderTargetStruct fboData = RenderTargetMap[fbo];
                glViewport(0, 0, fboData.Width, fboData.Height);
            }

            BoundRenderTarget = fbo;
        }

        public RenderTargetStruct GetRenderTarget(uint fbo) {
            return RenderTargetMap[fbo];
        }

        public uint CreateShader(string source) {
            string[] segmentCodes = new string[3];
            string typeToken = "#type";
            int typeTokenLength = typeToken.Length;
            int pos = source.IndexOf(typeToken, 0);
            while(pos != -1) {
                int eol = source.IndexOf("\r\n", pos);
                int begin = pos + typeTokenLength + 1;
                string type = source.Substring(begin, eol - begin);

                int nextLinePos = -1;
                for(int i = eol; i < source.Length; i++) {
                    if(source[i] != '\r' || source[i] != '\n') {
                        nextLinePos = i;
                        break;
                    }
                }

                pos = source.IndexOf(typeToken, nextLinePos);
                segmentCodes[GetShaderTypeIndex(type)] = (pos == -1) ? source.Substring(nextLinePos) : source.Substring(nextLinePos, pos - nextLinePos);
            }

            return CreateShader(segmentCodes[0], segmentCodes[1], segmentCodes[2]);
        }

        private int GetShaderTypeIndex(string str) {
            if(str.ToLower() == "header") {
                return 0;
            } else if(str.ToLower() == "vertex") {
                return 1;
            } else if(str.ToLower() == "fragment" || str.ToLower() == "pixel") {
                return 2;
            }

            return -1;
        }

        public uint CreateShader(string headerCode, string vertexCode, string fragmentCode) {
            uint vertexShaderId = glCreateShader(GL_VERTEX_SHADER);
            string[] vertexShaderCode = { headerCode, vertexCode };
            glShaderSource(vertexShaderId, vertexShaderCode.Length, vertexShaderCode, new int[] { headerCode.Length, vertexCode.Length });
            glCompileShader(vertexShaderId);

            uint fragmentShaderId = glCreateShader(GL_FRAGMENT_SHADER);
            string[] fragmentShaderCode = { headerCode, fragmentCode };
            glShaderSource(fragmentShaderId, fragmentShaderCode.Length, fragmentShaderCode, new int[] { headerCode.Length, fragmentCode.Length });
            glCompileShader(fragmentShaderId);

            uint programId = glCreateProgram();
            glAttachShader(programId, vertexShaderId);
            glAttachShader(programId, fragmentShaderId);
            glLinkProgram(programId);

            glValidateProgram(programId);
            glGetProgramiv(programId, GL_LINK_STATUS, out int linked);
            glGetProgramiv(programId, GL_VALIDATE_STATUS, out int validated);
            if(linked == GL_FALSE || validated == GL_FALSE) {
                StringBuilder vertexErrors = new StringBuilder(4096);
                StringBuilder fragmentErrors = new StringBuilder(vertexErrors.Capacity);
                StringBuilder programErrors = new StringBuilder(vertexErrors.Capacity);
                glGetShaderInfoLog(vertexShaderId, vertexErrors.Capacity, out _, vertexErrors);
                glGetShaderInfoLog(fragmentShaderId, vertexErrors.Capacity, out _, fragmentErrors);
                glGetShaderInfoLog(programId, vertexErrors.Capacity, out _, programErrors);
                Debug.Assert(false, "");
            }

            ShaderStruct programData = new ShaderStruct() {
                Shaders = new uint[] { vertexShaderId, fragmentShaderId },
                UniformMap = new Dictionary<string, int>(),
                SamplerMap = new Dictionary<string, int>(),
            };
            ShaderMap[programId] = programData;

            SetShader(programId);
            glGetProgramiv(programId, GL_ACTIVE_UNIFORMS, out int numUniforms);
            glGetProgramiv(programId, GL_ACTIVE_UNIFORM_MAX_LENGTH, out int bufferSize);
            StringBuilder uniformNameBuffer = new StringBuilder(bufferSize);
            int samplerSlot = 0;
            for(uint i = 0; i < numUniforms; i++) {
                glGetActiveUniform(programId, i, bufferSize, out int nameLength, out int arraySize, out uint type, uniformNameBuffer);
                string uniformName = uniformNameBuffer.ToString();
                int uniformLocation = glGetUniformLocation(programId, uniformName);
                programData.UniformMap[uniformName] = uniformLocation;
                if(type == GL_SAMPLER_2D) {
                    programData.SamplerMap[uniformName] = samplerSlot;
                    SetUniform(programId, uniformName, samplerSlot);
                    samplerSlot++;
                }
            }

            return programId;
        }

        public uint ReleaseShader(uint programId) {
            if(programId == 0 || !ShaderMap.ContainsKey(programId)) return 0;

            ShaderStruct programData = ShaderMap[programId];

            for(int i = 0; i < programData.Shaders.Length; i++) {
                glDetachShader(programId, programData.Shaders[i]);
                glDeleteShader(programData.Shaders[i]);
            }

            glDeleteShader(programId);
            ShaderMap.Remove(programId);
            return 0;
        }

        public void SetShader(uint programId) {
            if(BoundShader == programId) {
                return;
            }

            glUseProgram(programId);
            BoundShader = programId;
        }

        public ShaderStruct GetShader(uint programId) {
            return ShaderMap[programId];
        }

        public void SetUniform(uint programId, string name, float v0) {
            SetShader(programId);
            glUniform1f(ShaderMap[programId].UniformMap[name], v0);
        }

        public void SetUniform(uint programId, string name, float v0, float v1) {
            SetShader(programId);
            glUniform2f(ShaderMap[programId].UniformMap[name], v0, v1);
        }

        public void SetUniform(uint programId, string name, float v0, float v1, float v2) {
            SetShader(programId);
            glUniform3f(ShaderMap[programId].UniformMap[name], v0, v1, v2);
        }

        public void SetUniform(uint programId, string name, float v0, float v1, float v2, float v3) {
            SetShader(programId);
            glUniform4f(ShaderMap[programId].UniformMap[name], v0, v1, v2, v3);
        }

        public void SetUniform(uint programId, string name, int v0) {
            SetShader(programId);
            glUniform1i(ShaderMap[programId].UniformMap[name], v0);
        }

        public void SetUniform(uint programId, string name, int v0, int v1) {
            SetShader(programId);
            glUniform2i(ShaderMap[programId].UniformMap[name], v0, v1);
        }

        public void SetUniform(uint programId, string name, int v0, int v1, int v2) {
            SetShader(programId);
            glUniform3i(ShaderMap[programId].UniformMap[name], v0, v1, v2);
        }

        public void SetUniform(uint programId, string name, int v0, int v1, int v2, int v3) {
            SetShader(programId);
            glUniform4i(ShaderMap[programId].UniformMap[name], v0, v1, v2, v3);
        }

        public void SetUniformMat2(uint programId, string name, bool transpose, float[] mat) {
            SetShader(programId);
            glUniformMat2(ShaderMap[programId].UniformMap[name], transpose ? GL_TRUE : GL_FALSE, mat);
        }

        public void SetUniformMat3(uint programId, string name, bool transpose, float[] mat) {
            SetShader(programId);
            glUniformMat3(ShaderMap[programId].UniformMap[name], transpose ? GL_TRUE : GL_FALSE, mat);
        }

        public void SetUniformMat4(uint programId, string name, bool transpose, float[] mat) {
            SetShader(programId);
            glUniformMat4(ShaderMap[BoundShader].UniformMap[name], transpose ? GL_TRUE : GL_FALSE, mat);
        }

        public uint CreatePixelBuffer(int size, BufferUsage usage) {
            glGenBuffers(out uint pbo);
            SetPixelBuffer(pbo);
            glBufferData(GL_PIXEL_PACK_BUFFER, size, null, (uint) usage);
            PixelBufferMap[pbo] = new PixelBufferStruct() {
                Size = size,
            };
            SetPixelBuffer(0);

            return pbo;
        }

        public uint ReleasePixelBuffer(uint pbo) {
            if(pbo == 0 || !PixelBufferMap.ContainsKey(pbo)) return 0;

            glDeleteBuffers(pbo);
            PixelBufferMap.Remove(pbo);
            return 0;
        }

        public void SetPixelBuffer(uint pbo) {
            if(BoundPixelBuffer == pbo) {
                return;
            }

            glBindBuffer(GL_PIXEL_PACK_BUFFER, pbo);
            BoundPixelBuffer = pbo;
        }

        public PixelBufferStruct GetPixelBuffer(uint pbo) {
            return PixelBufferMap[pbo];
        }

        public void ReadTexture2DContents(uint pbo, uint texture, byte[] dest, PixelFormat format) {
            SetTexture2D(texture);
            SetPixelBuffer(pbo);
            glGetTexImage(GL_TEXTURE_2D, 0, (uint) format, GL_UNSIGNED_BYTE, IntPtr.Zero);
            IntPtr pointer = glMapBuffer(GL_PIXEL_PACK_BUFFER, GL_READ_ONLY);
            Marshal.Copy(pointer, dest, 0, dest.Length);
            glUnmapBuffer(GL_PIXEL_PACK_BUFFER);
        }
    }
}
