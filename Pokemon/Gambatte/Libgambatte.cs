using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public enum LoadFlags : uint {

        GcbMode =         1 << 0,
        GbaFlag =         1 << 1,
        MultiCartCompat = 1 << 2,
        SgbMode =         1 << 3,
        ReadOnlySav =     1 << 4,
    }

    public enum LoaderResult : int {

        Ok =                          0,
        BadFileOrUnknownMbc =        -0x7FFF,
        IoError =                    -0x1FE,
        UnsupportedMbcHuc3 =         -0x1FE,
        UnsupportedMbcTama5 =        -0x122,
        UnsupportedMbcPocketCamera = -0x122,
        UnsupportedMbcMbc7 =         -0x122,
        UnsupportedMbcMbc6 =         -0x120,
        UnsupportedMbcMbc4 =         -0x117,
        UnsupportedMbcMmm01 =        -0x10D,
    }

    public enum Joypad : byte {

        None   = 0,
        A      = 1 << 0,
        B      = 1 << 1,
        Select = 1 << 2,
        Start  = 1 << 3,
        Right  = 1 << 4,
        Left   = 1 << 5,
        Up     = 1 << 6,
        Down   = 1 << 7,
        All    = 0xFF,
    }

    public enum PalType : uint {

        BgPalette =  0,
        Sp1Palette = 1,
        Sp2Palette = 2
    }

    public enum RegisterIndex : uint {

        PC, SP, A, B, C, D, E, F, H, L
    }

    public enum SpeedupFlags : uint {

        None =      0,
        NoSound =   1 << 0,
        NoPPUCall = 1 << 1,
        NoVideo =   1 << 2,
        All =       0xFFFFFFFF,
    }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate Joypad InputGetter();

    public static unsafe class Libgambatte {

        public const string dll = "libgambatte.dll";

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr gambatte_create();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void gambatte_destory();

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern LoaderResult gambatte_load(IntPtr gb, string rom, LoadFlags flags);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern LoaderResult gambatte_loadbios(IntPtr gb, string bios, uint size, uint crc);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void gambatte_setinputgetter(IntPtr gb, InputGetter callback);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte gambatte_cpuread(IntPtr gb, ushort address);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte gambatte_cpuwrite(IntPtr gb, ushort address, byte data);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern byte gambatte_runfor(IntPtr gb, byte[] videoBuf, uint pitch, byte[] audioBuf, ref uint samples);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern CpuAddress gambatte_gethitinterruptaddress(IntPtr gb);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void gambatte_setinterruptaddresses(IntPtr gb, CpuAddress* addrs, int numAddrs);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint gambatte_savestate(IntPtr gb, byte[] videoBuf, int pitch, byte* stateBuf);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool gambatte_loadstate(IntPtr gb, byte[] state, int size);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong gambatte_reset(IntPtr gb, uint samplesToStall);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong gambatte_setspeedupflags(IntPtr gb, SpeedupFlags flags);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern void gambatte_getregs(IntPtr gb, int[] dest);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern ulong gambatte_timenow(IntPtr gb);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int gambatte_getdivstate(IntPtr gb);

        [DllImport(dll, CallingConvention = CallingConvention.Cdecl)]
        public static extern int gambatte_revision();
    }
}
