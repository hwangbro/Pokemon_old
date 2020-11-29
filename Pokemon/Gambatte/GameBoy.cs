using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Pokemon {

    public struct OAMSprite {

        public byte Y;
        public byte X;
        public byte TileNumber;
        public byte Attribute;

        public byte Priority {
            get { return (byte) (Attribute & (1 << 7)); }
        }

        public bool YFlip {
            get { return (Attribute & (1 << 6)) != 0; }
        }

        public bool XFlip {
            get { return (Attribute & (1 << 5)) != 0; }
        }

        public byte PaletteNumber {
            get { return (byte) (Attribute & (1 << 4)); }
        }

        public byte VramBank {
            get { return (byte) (Attribute & (1 << 3) >> 3); }
        }

        public byte PaletteNumberCBG {
            get { return (byte) (Attribute & 3); }
        }
    }

    public unsafe class GameBoy : IDisposable {

        public const uint VideoBufferWidth = 160;
        public const uint VideoBufferHeight = 144;
        public const uint VideoBufferSize = VideoBufferWidth * VideoBufferHeight;
        public const uint AudioBufferSize = (35112 + 2064) * 2;
        public const int NumSamplesPerFrame = 35112;
        public const int BankSize = 0x4000;

        public delegate void ComparisonFn(GameBoy gb);

        public IntPtr Handle;
        public Sym Sym;
        public byte[] Rom;
        public byte[][] Ram;

        public uint Samples;
        public byte[] VideoBuffer;
        public byte[] AudioBuffer;
        public ulong TotalCyclesRan;
        public double SceneNow;
        public uint FrameOverflow;
        public Joypad CurrentJoypad;
        public InputGetter InputCallback;

        public uint SaveStateSize;
        public Dictionary<string, uint> SaveStateLabels;

        public Scene Scene;

        public GameBoy(string rom, string saveName, string bios, SpeedupFlags speedupFlags = SpeedupFlags.None, LoadFlags loadFlags = LoadFlags.GbaFlag | LoadFlags.GcbMode | LoadFlags.ReadOnlySav) {
            Handle = Libgambatte.gambatte_create();

            // copy base save to <romname>.sav
            string saveDest = rom.Replace(".gbc", ".sav");
            if (saveName != saveDest) {
                System.IO.File.Copy(saveName, saveDest, true);
            }

            LoaderResult romResult = Libgambatte.gambatte_load(Handle, rom, loadFlags);
            //Debug.Info("ROM loading: {0}", romResult);
            Debug.Assert(romResult == LoaderResult.Ok, "ROM loading failed!");
            Rom = File.ReadAllBytes(rom);
            int count = Rom.Length / BankSize;
            Ram = new byte[count][];
            byte[] home = Rom.Subarray(0, BankSize);
            for(int bank = 0; bank < count; bank++) {
                Ram[bank] = new byte[BankSize * 3];
                home.CopyTo(Ram[bank], 0);
                Rom.Subarray(bank * BankSize, BankSize).CopyTo(Ram[bank], BankSize);
            }

            LoaderResult biosResult = Libgambatte.gambatte_loadbios(Handle, bios, 0, 0);
            //Debug.Info("BIOS loading: {0}", biosResult);
            Debug.Assert(biosResult == LoaderResult.Ok, "BIOS loading failed!");

            string name = Path.GetFileNameWithoutExtension(rom);
            string symPath = "sym/" + name + ".sym";

            if(File.Exists(symPath)) {
                Sym = new Sym(symPath);
            }

            VideoBuffer = new byte[VideoBufferSize * 4];
            AudioBuffer = new byte[AudioBufferSize * 4];

            TotalCyclesRan = 0;
            FrameOverflow = 0;

            CurrentJoypad = 0;
            InputCallback = () => CurrentJoypad;
            Libgambatte.gambatte_setinputgetter(Handle, InputCallback);

            SetSpeedupFlags(speedupFlags);

            SaveStateSize = Libgambatte.gambatte_savestate(Handle, null, (int) VideoBufferWidth, null);
            SaveStateLabels = SaveStateUtils.GetLabels(SaveState());

            //Debug.Info("Gambatte Revision: {0}", Libgambatte.gambatte_revision());
        }

        public void Dispose() {
            if(Scene != null) {
                Scene.Dispose();
            }
        }

        private SymEntry EvaluateAddressName(string address) {
            if(!address.Contains("+") && !address.Contains("-")) {
                return Sym[address];
            } else {
                int offsetBegin = Math.Max(address.IndexOf("+"), address.IndexOf("-"));
                int offset = Convert.ToInt32(address.Substring(offsetBegin + 1, 4), 16);
                if(address[offsetBegin] == '-') offset *= -1;

                SymEntry baseEntry = Sym[address.Substring(0, offsetBegin)];
                return new SymEntry(new Address() {
                    Bank = baseEntry.Bank,
                    Addr = (ushort) (baseEntry.Addr + offset),
                });
            }
        }

        public void CpuWrite(CpuAddress address, byte data) {
            Libgambatte.gambatte_cpuwrite(Handle, address, data);
        }

        public void CpuWrite(CpuAddress address, byte[] data) {
            for(uint offset = 0; offset < data.Length; offset++) {
                CpuWrite(address + offset, data[offset]);
                Libgambatte.gambatte_cpuwrite(Handle, address + offset, data[offset]);
            }
        }

        public void CpuWriteWord(CpuAddress address, ushort data) {
            Libgambatte.gambatte_cpuwrite(Handle, address, (byte) ((data >> 8) & 0xFF));
            Libgambatte.gambatte_cpuwrite(Handle, address + 1u, (byte) ((data) & 0xFF));
        }

        public void CpuWrite(string address, byte data) {
            CpuWrite(EvaluateAddressName(address), data);
        }

        public void CpuWrite(string address, byte[] data) {
            CpuWrite(EvaluateAddressName(address), data);
        }

        public void CpuWriteWord(string address, ushort data) {
            CpuWriteWord(EvaluateAddressName(address), data);
        }

        public byte CpuRead(CpuAddress address) {
            return Libgambatte.gambatte_cpuread(Handle, address);
        }

        public byte[] CpuRead(CpuAddress address, uint length) {
            byte[] result = new byte[length];
            for(uint i = 0; i < length; i++) {
                result[i] = CpuRead(address + i);
            }
            return result;
        }

        public ushort CpuReadWord(CpuAddress address) {
            return (ushort) ((CpuRead(address) << 8) | CpuRead(address + 1));
        }

        public byte CpuRead(string address) {
            return CpuRead(EvaluateAddressName(address));
        }

        public byte[] CpuRead(string address, uint length) {
            return CpuRead(EvaluateAddressName(address), length);
        }

        public ushort CpuReadWord(string address) {
            return CpuReadWord(EvaluateAddressName(address));
        }

        public int Runfor(uint samplesToRun = NumSamplesPerFrame) {
            Samples = samplesToRun;
            int ret = Libgambatte.gambatte_runfor(Handle, VideoBuffer, VideoBufferWidth, AudioBuffer, ref Samples);
            TotalCyclesRan += Samples;
            FrameOverflow += Samples;

            if(Scene != null) {
                Scene.Render();
            }

            while(TotalCyclesRan > SceneNow) {
                SceneNow += Math.Pow(2, 21) / 60.0;
            }

            return ret;
        }

        public int Step(Joypad joypad = Joypad.None) {
            CurrentJoypad = joypad;
            int ret = Runfor(1);
            CurrentJoypad = Joypad.None;
            return ret;
        }

        public void AdvanceFrames(int frames) {
            for (int i = 0; i < frames; i++) {
                AdvanceFrame();
            }
        }

        public CpuAddress AdvanceFrame(Joypad joypad = Joypad.None) {
            CurrentJoypad = joypad;
            int ret = Runfor(NumSamplesPerFrame - FrameOverflow);
            CpuAddress hitAddress = Libgambatte.gambatte_gethitinterruptaddress(Handle);
            if(ret >= 0 || FrameOverflow >= NumSamplesPerFrame) {
                FrameOverflow = 0;
            }
            CurrentJoypad = Joypad.None;
            return hitAddress;
        }

        public CpuAddress AdvanceToAddress(params CpuAddress[] addresses) {
            return AdvanceWithJoypadToAddress(Joypad.None, addresses);
        }

        public CpuAddress AdvanceWithJoypadToAddress(Joypad joypad, params CpuAddress[] addresses) {
            fixed(CpuAddress* addr = addresses) {
                Libgambatte.gambatte_setinterruptaddresses(Handle, addr, addresses.Length);
                CpuAddress hitAddress;
                do {
                    hitAddress = AdvanceFrame(joypad);
                } while(Array.IndexOf(addresses, hitAddress) == -1);
                Libgambatte.gambatte_setinterruptaddresses(Handle, null, 0);
                return hitAddress;
            }
        }

        public CpuAddress AdvanceToAddress(params string[] addresses) {
            return AdvanceWithJoypadToAddress(Joypad.None, addresses);
        }

        public CpuAddress AdvanceWithJoypadToAddress(Joypad joypad, params string[] addresses) {
            return AdvanceWithJoypadToAddress(joypad, Array.ConvertAll(addresses, addr => EvaluateAddressName(addr).Cpu()));
        }

        public virtual void Inject(Joypad joypad) {
            throw new NotImplementedException();
        }

        public virtual void InjectMenu(Joypad joypad) {
            throw new NotImplementedException();
        }

        public virtual byte ReadHRA() {
            throw new NotImplementedException();
        }

        public virtual byte ReadHRS() {
            throw new NotImplementedException();
        }

        public virtual byte ReadRDIV() {
            return CpuRead(0xFF04);
        }

        public void Press(string actions) {
            string[] arr = actions.Split(" ");
            foreach(string action in arr) {
                if(action.ToUpper() == "U") {
                    Press(Joypad.Up);
                } else if(action.ToUpper() == "D") {
                    Press(Joypad.Down);
                } else if(action.ToUpper() == "R") {
                    Press(Joypad.Right);
                } else if(action.ToUpper() == "L") {
                    Press(Joypad.Left);
                } else if(action.ToUpper() == "A") {
                    Press(Joypad.A);
                }
            }
        }

        public virtual void Press(params Joypad[] joypads) {
            throw new NotImplementedException();
        }

        public void Press(int count, params Joypad[] joypads) {
            for(int i = 0; i < count; i++) {
                Press(joypads);
            }
        }

        public CpuAddress Execute(List<Action> actions, params (byte, byte, byte)[] itemPickups) {
            return Execute(actions.ToArray(), itemPickups);
        }

        public CpuAddress Execute(string actions, params (byte, byte, byte)[] itemPickups) {
            actions = actions.Trim();
            if(actions == "") return new CpuAddress(0);
            return Execute(Array.ConvertAll(actions.Split(" "), action => ActionFunctions.ToAction(action)), itemPickups);
        }

        public virtual CpuAddress Execute(Action action, params (byte, byte, byte)[] itemPickups) {
            return Execute(new Action[] { action }, itemPickups);
        }

        public virtual CpuAddress Execute(Action[] actions, params (byte, byte, byte)[] itemPickups) {
            throw new NotImplementedException();
        }

        public void Reset(uint samplesToStall = 0) {
            Libgambatte.gambatte_reset(Handle, samplesToStall);
        }

        public void SetTimeSec(uint rtc) {
            byte[] savestate = SaveState();
            savestate[SaveStateLabels["timesec"] + 0] = (byte) (rtc >> 24);
            savestate[SaveStateLabels["timesec"] + 1] = (byte) (rtc >> 16);
            savestate[SaveStateLabels["timesec"] + 2] = (byte) (rtc >>  8);
            savestate[SaveStateLabels["timesec"] + 3] = (byte) (rtc & 0xFF);
            LoadState(savestate);
        }

        public void RandomizeRNG(Random random) {
            byte[] randomValues = new byte[3];
            random.NextBytes(randomValues);

            byte[] savestate = SaveState();
            savestate[SaveStateLabels["hram"] + 0x104] = randomValues[0]; // rdiv
            savestate[SaveStateLabels["hram"] + 0x1D3] = randomValues[1]; // hRandomAdd
            savestate[SaveStateLabels["hram"] + 0x1D4] = randomValues[2]; // hRandomSub
            LoadState(savestate);
        }

        public void PlayGambatteMovie(string fileName) {
            PlayGambatteMovie(File.ReadAllBytes(fileName));
        }

        public void PlayGambatteMovie(byte[] movie) {
            Debug.Assert(movie[0] == 0xFE, "Invalid movie file!");
            Debug.Assert(movie[1] == 0x01, "Invalid movie version!");

            int stateSize = (movie[2] << 16) | (movie[3] << 8) | (movie[4]);
            byte[] state = movie.Subarray(5, stateSize);
            LoadState(state);

            for(int i = 5 + stateSize; i < movie.Length; i += 5) {
                uint samplesToRun = (uint) ((movie[i] << 24) | (movie[i + 1] << 16) | (movie[i + 2] << 8) | movie[i + 3]);
                byte input = movie[i + 4];
                if(input == 0xFF) {
                    Reset(101 * (2 << 14)); // = 3309568 / 2^21 ~= 1.578125s
                } else {
                    CurrentJoypad = (Joypad) input;
                    while(samplesToRun > 0 && samplesToRun <= 0x80000000) {
                        Runfor(Math.Min(samplesToRun, NumSamplesPerFrame));
                        samplesToRun -= Samples;
                    }
                }
            }
        }

        public void PlayBizhawkMovie(string fileName) {
            using(FileStream file = File.OpenRead(fileName))
            using(ZipArchive zip = new ZipArchive(file, ZipArchiveMode.Read))
            using(StreamReader reader = new StreamReader(zip.GetEntry("Input Log.txt").Open())) {
                PlayBizhawkInputLog(reader.ReadToEnd().Split('\n'));
            }
        }

        public void PlayBizhawkInputLog(string fileName) {
            PlayBizhawkInputLog(File.ReadAllLines(fileName));
        }

        public void PlayBizhawkInputLog(string[] lines) {
            Joypad[] joypadFlags = { Joypad.Up, Joypad.Down, Joypad.Left, Joypad.Right, Joypad.Start, Joypad.Select, Joypad.B, Joypad.A };
            lines = lines.Subarray(2, lines.Length - 3);

            for(int i = 0; i < lines.Length; i++) {
                Joypad joypad = Joypad.None;
                for(int j = 0; j < joypadFlags.Length; j++) {
                    if(lines[i][j + 1] != '.') {
                        joypad |= joypadFlags[j];
                    }
                }
                AdvanceFrame(joypad);
            }
        }

        public void Show() {
            Scene scene = new Scene(320, 288, this);
            scene.Add(new Entity(0, 0, 320, 288).AddComponent(new VideoBufferComponent()));
            SetSpeedupFlags(SpeedupFlags.NoSound);
        }

        public void Record(string movieName) {
            Show();
            RecordingComponent recorder = new RecordingComponent(movieName);
            Scene.Add(new Entity().AddComponent(recorder));
            recorder.RecordingNow = TotalCyclesRan;
            SetSpeedupFlags(SpeedupFlags.None);
        }

        public void RecordComparison(string savestate, ComparisonFn fn, string movieName) {
            Scene scene = new Scene(160, 160, this);
            LoadState(File.ReadAllBytes(savestate));

            TimerComponent timer = new TimerComponent("ss\\.fff");
            RecordingComponent recorder = new RecordingComponent(movieName);
            scene.Add(new Entity(0, 0, 160, 144).AddComponent(new VideoBufferComponent()).AddComponent(recorder));
            scene.Add(new Entity(0, 144, 16, 16).AddComponent(timer));

            recorder.Start();
            timer.Start();
            fn(this);
            timer.Stop();
            for(int i = 0; i < 3 * 60; i++) AdvanceFrame();
            recorder.Stop();

            scene.Dispose();
        }

        public byte[] SaveState() {
            byte[] savestate = new byte[SaveStateSize];
            fixed(byte* ptr = savestate) {
                Libgambatte.gambatte_savestate(Handle, null, (int) VideoBufferWidth, ptr);
            }

            return savestate;
        }

        public void SaveState(string filename) {
            byte[] savestate = SaveState();
            File.WriteAllBytes(filename, savestate);
        }

        public void LoadState(string fileName) {
            LoadState(File.ReadAllBytes(fileName));
        }

        public void LoadState(byte[] buffer) {
            Debug.Assert(Libgambatte.gambatte_loadstate(Handle, buffer, buffer.Length), "Unable to load save state!");
        }

        public ulong GetCycleCount() {
            return Libgambatte.gambatte_timenow(Handle);
        }

        public int GetDivState() {
            return Libgambatte.gambatte_getdivstate(Handle);
        }

        public int[] GetRegisters() {
            int[] registers = new int[10];
            Libgambatte.gambatte_getregs(Handle, registers);
            return registers;
        }

        public int GetRegister(RegisterIndex index) {
            return GetRegisters()[(int) index];
        }

        public void SetSpeedupFlags(SpeedupFlags flags) {
            Libgambatte.gambatte_setspeedupflags(Handle, flags);
        }

        public void SaveScreenshot(string file) {
            Bitmap bitmap;

            if(Scene == null) {
                bitmap = new Bitmap(160, 144, VideoBuffer, false, false, ComponentMapping.BGRA);
            } else {
                RenderContext.CopyToOffscreenBuffer(PixelFormat.RGBA);
                bitmap = new Bitmap(RenderContext.ScreenBufferWidth, RenderContext.ScreenBufferHeight, RenderContext.OffscreenBuffer, false, true);
            }

            bitmap.Save(file);
        }
    }
}