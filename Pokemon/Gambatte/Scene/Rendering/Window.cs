using System;
using static Pokemon.Windows;

namespace Pokemon {

    public static class Window {

        public static WNDPROC WindowCallback = _WindowCallback;

        private static int _WindowCallback(IntPtr window, uint message, uint wParam, long lParam) {
            int result = 0;

            switch(message) {
                case WM_CLOSE:
                    // TODO: proper exit
                    Environment.Exit(1);
                    break;
                case WM_PAINT:
                    IntPtr deviceContext = BeginPaint(window, out PAINTSTRUCT paint);
                    PresentInternal(deviceContext);
                    EndPaint(window, ref paint);
                    break;
                default:
                    result = DefWindowProcA(window, message, wParam, lParam);
                    break;
            }

            return result;
        }

        public static IntPtr Create(string title, int x, int y, int width, int height) {
            IntPtr instance = GetModuleHandleA(null);

            WNDCLASS windowClass = new WNDCLASS() {};
            windowClass.style = CS_HREDRAW | CS_VREDRAW;
            windowClass.lpfnWndProc = WindowCallback;
            windowClass.hInstance = instance;
            windowClass.lpszClassName = "PokemonWindowClass";

            if(RegisterClassA(ref windowClass) != 0) {
                uint windowStyle = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX | WS_MAXIMIZEBOX | WS_VISIBLE;
                RECT windowSize = new RECT() {
                    left = 0,
                    top = 0,
                    right = width,
                    bottom = height,
                };
                AdjustWindowRect(ref windowSize, windowStyle, 0);

                IntPtr windowHandle = CreateWindowExA(0, windowClass.lpszClassName, title, windowStyle, x, y, windowSize.right - windowSize.left, windowSize.bottom - windowSize.top, IntPtr.Zero, IntPtr.Zero, instance, IntPtr.Zero);
                if(windowHandle != IntPtr.Zero) {
                    return windowHandle;
                }
            }

            return IntPtr.Zero;
        }

        public static void ProcessMessages() {
            MSG message;
            while(PeekMessageA(out message, IntPtr.Zero, 0, 0, PM_REMOVE) > 0) {
                if(message.message == WM_QUIT) {
                    // TODO: proper exit
                    Environment.Exit(1);
                }
                TranslateMessage(ref message);
                DispatchMessage(ref message);
            }
        }

        public static void Present(IntPtr windowHandle) {
            IntPtr deviceContext = GetDC(windowHandle);
            PresentInternal(deviceContext);
            ReleaseDC(windowHandle, deviceContext);
        }

        private static void PresentInternal(IntPtr deviceContext) {
            SwapBuffers(deviceContext);
        }

        public static int GetWidth(IntPtr windowHandle) {
            GetClientRect(windowHandle, out RECT rect);
            return rect.right - rect.left;
        }

        public static int GetHeight(IntPtr windowHandle) {
            GetClientRect(windowHandle, out RECT rect);
            return rect.bottom - rect.top;
        }
    }
}
