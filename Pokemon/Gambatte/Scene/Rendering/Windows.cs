using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    using HWND = IntPtr;
    using HINSTANCE = IntPtr;
    using HICON = IntPtr;
    using HCURSOR = IntPtr;
    using HBRUSH = IntPtr;
    using HDC = IntPtr;
    using HMENU = IntPtr;
    using LPVOID = IntPtr;
    using HMODULE = IntPtr;

    public delegate int WNDPROC(HWND hwnd, uint uMsg, uint wParam, long lParam);

    public struct WNDCLASS {

        public uint style;
        public WNDPROC lpfnWndProc;
        public int cbClsExtra;
        public int cbWndExtra;
        public HINSTANCE hInstance;
        public HICON hIcon;
        public HCURSOR hCursor;
        public HBRUSH hBrush;
        public string lpszMenuName;
        public string lpszClassName;
    }

    public struct POINT {

        public int x;
        public int y;
    }

    public struct MSG {

        public HWND hwnd;
        public uint message;
        public uint wParam;
        public long lParam;
        public uint time;
        public POINT pt;
        public uint lPrivate;
    }

    public struct RECT {

        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public unsafe struct PAINTSTRUCT {

        public HDC hdc;
        public int fErase;
        public RECT rcPaint;
        public int fRestore;
        public int fIncUpdate;
        public fixed byte rgbReserved[32];
    }

    public struct PIXELFORMATDESCRIPTOR {

        public ushort nSize;
        public ushort nVersion;
        public uint dwFlags;
        public byte iPixelType;
        public byte cColorBits;
        public byte cRedBits;
        public byte cRedShift;
        public byte cGreenBits;
        public byte cGreenShift;
        public byte cBlueBits;
        public byte cBlueShift;
        public byte cAlphaBits;
        public byte cAlphaShift;
        public byte cAccumBits;
        public byte cAccumRedBits;
        public byte cAccumGreenBits;
        public byte cAccumBlueBits;
        public byte cAccumAlphaBits;
        public byte cDepthBits;
        public byte cStencilBits;
        public byte cAuxBuffers;
        public byte iLayerType;
        public byte bReserved;
        public uint dwLayerMask;
        public uint dwVisibleMask;
        public uint dwDamageMask;
    }

    public static class Windows {

        public const string USER32 = "user32.dll";
        public const string KERNEL32 = "kernel32.dll";
        public const string GDI32 = "gdi32.dll";

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern ushort RegisterClassA(ref WNDCLASS lpWndClass);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DefWindowProcA(HWND hWnd, uint msg, uint wParam, long lParam);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern HWND CreateWindowExA(uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, HWND hWndParent, HMENU hMenu, HINSTANCE hInstance, LPVOID lpParam);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DestroyWindow(HWND hWnd);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetMessage(out MSG lpMsg, HWND hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int PeekMessageA(out MSG lpMsg, HWND hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int TranslateMessage(ref MSG lpMsg);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DispatchMessage(ref MSG lpMsg);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern HDC GetDC(HWND hWnd);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReleaseDC(HWND hWnd, HDC hDC);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetWindowRect(HWND hWnd, out RECT lpRect);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetClientRect(HWND hWnd, out RECT lpRect);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AdjustWindowRect(ref RECT lpRect, uint dwStyle, int bMenu);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern HDC BeginPaint(HWND hWnd, out PAINTSTRUCT lpPaint);

        [DllImport(USER32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int EndPaint(HWND hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport(GDI32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ChoosePixelFormat(HDC hDC, ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport(GDI32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int DescribePixelFormat(HDC hDC, int iPixelFormat, uint nBytes, out PIXELFORMATDESCRIPTOR ppfd);

        [DllImport(GDI32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetPixelFormat(HDC hDC, int format, ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport(GDI32, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SwapBuffers(HDC hDC);

        [DllImport(KERNEL32, CallingConvention = CallingConvention.Cdecl)]
        public static extern HMODULE GetModuleHandleA(string lpModuleName);

        public const uint CS_VREDRAW         = 0x00001;
        public const uint CS_HREDRAW         = 0x00002;
        public const uint CS_DBLCLKS         = 0x00008;
        public const uint CS_OWNDC           = 0x00020;
        public const uint CS_PARENTDC        = 0x00080;
        public const uint CS_NOCLOSE         = 0x00100;
        public const uint CS_SAVEBITS        = 0x00800;
        public const uint CS_BYTEALIGNCLIENT = 0x01000;
        public const uint CS_BYTEALIGNWINDOW = 0x02000;
        public const uint CS_GLOBALCLASS     = 0x04000;
        public const uint CS_DROPSHADOW      = 0x20000;

        public const uint WM_NULL            = 0x0000;
        public const uint WM_CREATE          = 0x0001;
        public const uint WM_DESTROY         = 0x0002;
        public const uint WM_MOVE            = 0x0003;
        public const uint WM_SIZE            = 0x0005;
        public const uint WM_ACTIVATE        = 0x0006;
        public const uint WM_SETFOCUS        = 0x0007;
        public const uint WM_KILLFOCUS       = 0x0008;
        public const uint WM_ENABLE          = 0x000A;
        public const uint WM_SETREDRAW       = 0x000B;
        public const uint WM_SETTEXT         = 0x000C;
        public const uint WM_GETTEXT         = 0x000D;
        public const uint WM_GETTEXTLENGTH   = 0x000E;
        public const uint WM_PAINT           = 0x000F;
        public const uint WM_CLOSE           = 0x0010;
        public const uint WM_QUERYENDSESSION = 0x0011;
        public const uint WM_QUERYOPEN       = 0x0013;
        public const uint WM_ENDSESSION      = 0x0016;
        public const uint WM_QUIT            = 0x0012;
        public const uint WM_ERASEBKGND      = 0x0014;
        public const uint WM_SYSCOLORCHANGE  = 0x0015;
        public const uint WM_SHOWWINDOW      = 0x0018;
        public const uint WM_WININICHANGE    = 0x001A;
        public const uint WM_SETTINGCHANGE   = WM_WININICHANGE;
        public const uint WM_DEVMODECHANGE   = 0x001B;
        public const uint WM_ACTIVATEAPP     = 0x001C;

        public const uint WS_BORDER           = 0x00800000;
        public const uint WS_CAPTION          = 0x00C00000;
        public const uint WS_CHILD            = 0x40000000;
        public const uint WS_CHILDWINDOW      = 0x40000000;
        public const uint WS_CLIPCHILDREN     = 0x02000000;
        public const uint WS_CLIPSIBLINGS     = 0x04000000;
        public const uint WS_DISABLED         = 0x08000000;
        public const uint WS_DLGFRAME         = 0x00400000;
        public const uint WS_GROUP            = 0x00020000;
        public const uint WS_HSCROLL          = 0x00100000;
        public const uint WS_ICONIC           = 0x20000000;
        public const uint WS_MAXIMIZE         = 0x01000000;
        public const uint WS_MAXIMIZEBOX      = 0x00010000;
        public const uint WS_MINIMIZE         = 0x20000000;
        public const uint WS_MINIMIZEBOX      = 0x00020000;
        public const uint WS_OVERLAPPED       = 0x00000000;
        public const uint WS_OVERLAPPEDWINDOW = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        public const uint WS_POPUP            = 0x80000000;
        public const uint WS_POPUPWINDOW      = (WS_POPUP | WS_BORDER | WS_SYSMENU);
        public const uint WS_SIZEBOX          = 0x00040000;
        public const uint WS_SYSMENU          = 0x00080000;
        public const uint WS_TABSTOP          = 0x00010000;
        public const uint WS_THICKFRAME       = 0x00040000;
        public const uint WS_TILED            = 0x00000000;
        public const uint WS_TILEDWINDOW      = (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX);
        public const uint WS_VISIBLE          = 0x10000000;
        public const uint WS_VSCROLL          = 0x00200000;

        public const uint PFD_TYPE_RGBA             = 0;
        public const uint PFD_TYPE_COLORINDEX       = 1;
        public const uint PFD_MAIN_PLANE            = 0;
        public const uint PFD_OVERLAY_PLANE         = 1;
        public const uint PFD_DOUBLEBUFFER          = 0x00000001;
        public const uint PFD_STEREO                = 0x00000002;
        public const uint PFD_DRAW_TO_WINDOW        = 0x00000004;
        public const uint PFD_DRAW_TO_BITMAP        = 0x00000008;
        public const uint PFD_SUPPORT_GDI           = 0x00000010;
        public const uint PFD_SUPPORT_OPENGL        = 0x00000020;
        public const uint PFD_GENERIC_FORMAT        = 0x00000040;
        public const uint PFD_NEED_PALETTE          = 0x00000080;
        public const uint PFD_NEED_SYSTEM_PALETTE   = 0x00000100;
        public const uint PFD_SWAP_EXCHANGE         = 0x00000200;
        public const uint PFD_SWAP_COPY             = 0x00000400;
        public const uint PFD_SWAP_LAYER_BUFFERS    = 0x00000800;
        public const uint PFD_GENERIC_ACCELERATED   = 0x00001000;
        public const uint PFD_SUPPORT_DIRECTDRAW    = 0x00002000;
        public const uint PFD_DEPTH_DONTCARE        = 0x20000000;
        public const uint PFD_DOUBLEBUFFER_DONTCARE = 0x40000000;
        public const uint PFD_STEREO_DONTCARE       = 0x80000000;

        public const uint PM_NOREMOVE = 0x0000;
        public const uint PM_REMOVE   = 0x0001;
        public const uint PM_NOYIELD  = 0x0002;
    }
}
