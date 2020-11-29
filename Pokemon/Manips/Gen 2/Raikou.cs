using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Pokemon {

    public static class Raikou {

        public static SaveTile[] SaveTiles = {
            new SaveTile(12, 28, 0xC),
            new SaveTile(12, 28, 0x4),
            new SaveTile(12, 29, 0xC),
            new SaveTile(12, 29, 0x0),

            new SaveTile(13, 28, 0xC),
            new SaveTile(13, 28, 0x4),
            new SaveTile(13, 29, 0xC),
            new SaveTile(13, 29, 0x0),

            new SaveTile(14, 28, 0xC),
            new SaveTile(14, 28, 0x4),
            new SaveTile(14, 29, 0xC),
            new SaveTile(14, 29, 0x0),

            new SaveTile(15, 28, 0xC),
            new SaveTile(15, 29, 0xC),
        };

        public static void Execute(string[] args) {
        }

        public static int IGT(int frame, byte x, byte y, byte dir, string path, bool print = false, byte stepCount = 217) {
            if(path.Contains("-")) {
                path = path.Substring(0, path.IndexOf("-") - 1).Trim();
            }
            Crystal gb;
            lock(SaveLock) {
                MakeSave(x, y, dir, 0, stepCount);
                gb = new Crystal(true);
                gb.AdvanceToAddress(0x100);
                gb.SetTimeSec(3000);
            }
            gb.Press(Joypad.Start, Joypad.Start, Joypad.A);
            gb.AdvanceToAddress("GetJoypad");
            for(int i = 0; i <= frame; i++) {
                gb.AdvanceFrame();
            }

            byte[] igtState = gb.SaveState();

            int successes = 0;
            for(byte i = 0; i < 60; i++) {
                gb.LoadState(igtState);
                gb.CpuWrite("wGameTimeFrames", i);
                gb.AdvanceFrame(Joypad.A);
                gb.AdvanceToAddress("OWPlayerInput");

                if(print) Console.Write("[{0,2}] ", i);
                IGTResult result = ExecutePath(gb, path);

                if(print) Console.Write(result.ActionLog);
                if(print) Console.Write("- " + gb.Sym.GetAddressName(result.PC));
                if(print) Console.Write(" - ");
                if(result.PC == gb.Sym["RandomEncounter.ok"]) {
                    gb.AdvanceToAddress("CalcMonStats");
                    GscPokemon enemyMon = gb.GetEnemyMon();
                    if(print) Console.Write("DVs: {0:X4}, HP: {1} {2}", enemyMon.DVs, enemyMon.HPType, enemyMon.HPPower);
                    if(enemyMon.HPType == GscType.Ice && enemyMon.HPPower >= 68) {
                        successes++;
                    }
                } else {
                    if(print) Console.Write(" Raikou Location: 0x{0:X4}", gb.CpuReadWord(0xDFD1));
                }
                if(print) Console.Write(" ({0})", result.NumRandoms);
                if(print) Console.WriteLine();
            }

            if(print) Console.WriteLine(successes + "/60");

            return successes;
        }

        private static IGTResult ExecutePath(Crystal gb, string path) {
            IGTResult result = new IGTResult() {
                ActionLog = "",
                NumRandoms = 0,
                PC = default,
            };
            string[] actions = path.Split(" ");
            foreach(string action in actions) {
                result.ActionLog += action + " ";
                if((result.PC = Execute(gb, action.ToAction(), ref result.NumRandoms)) != gb.Sym["OWPlayerInput"]) break;
            }

            return result;
        }

        public static CpuAddress Execute(Crystal gb, Action action, ref int numRandoms) {
            CpuAddress ret = new CpuAddress(0);

            if(action.IsDpad()) {
                Joypad joypad;
                if(action == Action.LeftA || action == Action.RightA || action == Action.UpA || action == Action.DownA) {
                    joypad = (Joypad) (1 | ((uint) action >> 4));
                } else {
                    joypad = (Joypad) ((uint) action << 4);
                }
                gb.Inject(joypad);
                ret = AdvanceWithJoypadToAddressCountingRandoms(gb, joypad, ref numRandoms, "CountStep", "RandomEncounter.ok", "PrintLetterDelay.checkjoypad", "DoPlayerMovement.BumpSound");

                if(ret == gb.Sym["CountStep"]) {
                    ret = AdvanceWithJoypadToAddressCountingRandoms(gb, joypad, ref numRandoms, "OWPlayerInput", "RandomEncounter.ok", "PrintLetterDelay.checkjoypad", "DoPlayerMovement.BumpSound");
                }

                if(ret != gb.Sym["OWPlayerInput"]) return ret;
            } else if(action == Action.StartB) {
                gb.Inject(Joypad.Start);
                gb.AdvanceFrame(Joypad.Start);
                AdvanceWithJoypadToAddressCountingRandoms(gb, Joypad.None, ref numRandoms, "GetJoypad");
                gb.InjectMenu(Joypad.B);
                ret = AdvanceWithJoypadToAddressCountingRandoms(gb, Joypad.None, ref numRandoms, "OWPlayerInput", "PrintLetterDelay.checkjoypad");
            } else if(action == Action.Select) {
                gb.Inject(Joypad.Select);
                gb.AdvanceFrame(Joypad.Select);
                ret = AdvanceWithJoypadToAddressCountingRandoms(gb, Joypad.None, ref numRandoms, "OWPlayerInput", "PrintLetterDelay.checkjoypad");
            }

            return ret;
        }

        private static CpuAddress AdvanceWithJoypadToAddressCountingRandoms(Crystal gb, Joypad joypad, ref int numRandoms, params string[] addrs) {
            CpuAddress ret;
            do {
                ret = gb.AdvanceWithJoypadToAddress(joypad, addrs.Add("Random"));
                if(ret == gb.Sym["Random"]) {
                    numRandoms++;
                    gb.Step();
                } else {
                    break;
                }
            } while(true);

            return ret;
        }

        public static void StartRaikouSearch(byte x, byte y, byte facingDirection, byte igt, int frame, string path) {
            Crystal gb;
            lock(SaveLock) {
                MakeSave(x, y, facingDirection, igt);
                gb = new Crystal(true);

                gb.AdvanceToAddress(0x100);
            }

            gb.SetTimeSec(3000);
            gb.Press(Joypad.Start, Joypad.Start, Joypad.A);
            gb.AdvanceToAddress("GetJoypad");
            for(int i = 0; i <= frame; i++) {
                gb.AdvanceFrame();
            }
            gb.AdvanceFrame(Joypad.A);
            gb.AdvanceToAddress("OWPlayerInput");

            IGTResult result;
            if((result = ExecutePath(gb, path)).PC != gb.Sym["OWPlayerInput"]) {
                Console.WriteLine("Invalid path");
                return;
            }

            if(gb.CpuReadWord("wRoamMon1MapGroup") != 0x0A04) {
                Console.WriteLine("Raikou not on route");
                return;
            }

            GscTile curTile = gb.GetCurTile();
            RaikouSearch(gb, new HashSet<RaikouRNGState>(), new RaikouState() {
                Log = "[frame=" + frame + ", x=" + x + ", y=" + y + ", dir=" + facingDirection +", igt=" + igt + "] " + path,
                R37Log = "",
                OnBike = gb.CpuRead("wPlayerState") == 1,
                JustChangedDir = false,
                PrevChangedDir = false,
                FacingDir = ((uint) Action.Down) << 4,
                rDiv = gb.ReadRDIV(),
                hra = gb.ReadHRA(),
                hrs = gb.ReadHRS(),
                Tile = curTile,
                CanSelect = false,
                CanStart = false,
                CanAPress = false,
                TurnFrameStatus = false,
                WastedFrames = 0,
                RouteEnterTime = gb.GetCycleCount(),
                NumRandomCalls = result.NumRandoms,
                NumBikeSteps = 0,
            });
        }

        public const double RaikouMaxCost1 = 4.5;
        public const double RaikouMaxCost2 = 3.5;
        public const int MaxRandomCalls = 33;
        public const int MaxBikeSteps = 12;
        public static object WriteLock = new object();
        public static object SaveLock = new object();
        public static StreamWriter RaikouWriter = new StreamWriter("raikou_manips.txt");

        public static void RaikouSearch(Crystal gb, HashSet<RaikouRNGState> seenStates, RaikouState state) {
            if(!seenStates.Add(new RaikouRNGState() {
                rDiv = state.rDiv,
                hra = state.hra,
                hrs = state.hrs,
                X = state.Tile.X,
                Y = state.Tile.Y,
                OnBike = state.OnBike,
                CanSelect = state.CanSelect,
                CanStart = state.CanStart,
                CanAPress = state.CanAPress,
                TurnFrameStatus = state.TurnFrameStatus,
                FacingDir = state.FacingDir,
                PrevChangedDir = state.PrevChangedDir,
                JustChangedDir = state.JustChangedDir,
                NumBikeSteps = (byte) state.NumBikeSteps,
                NumRandomCalls = (byte) state.NumRandomCalls,
            })) {
                return;
            }

            double timeSpentOnRoute = (gb.GetCycleCount() - state.RouteEnterTime) / 2097152.0;
            if(timeSpentOnRoute > RaikouMaxCost1 || (timeSpentOnRoute > RaikouMaxCost2 && !state.R37Log.Contains("S_B"))) {
                return;
            }

            if(state.NumRandomCalls > MaxRandomCalls) {
                return;
            }

            byte[] savestate = gb.SaveState();

            foreach(GscEdge edge in state.Tile.Edges[0]) {
                int numRandomCalls = state.NumRandomCalls;
                bool justChangedDir = false;
                bool prevChangedDir = false;
                uint facingDir = state.FacingDir;
                bool canStart = false;
                bool canSelect = false;
                bool canAPress = false;
                bool turnFrameStatus = false;
                int numBikeSteps = state.NumBikeSteps;
                Action action = edge.Action;

                gb.LoadState(savestate);

                if(action.IsDpad()) {
                    uint joypad;
                    if(action == Action.LeftA || action == Action.RightA || action == Action.UpA || action == Action.DownA) {
                        if(state.OnBike || !state.CanAPress) {
                            continue;
                        }
                        joypad = (uint) action >> 4;
                    } else {
                        joypad = (uint) action << 4;
                        canAPress = true;
                    }

                    if(state.OnBike && state.JustChangedDir && state.PrevChangedDir && (joypad != state.FacingDir || state.TurnFrameStatus)) {
                        continue;
                    }

                    if(state.OnBike && state.NumBikeSteps >= MaxBikeSteps) {
                        continue;
                    }

                    CpuAddress ret = Execute(gb, action, ref numRandomCalls);

                    if(ret == gb.Sym["RandomEncounter.ok"]) {
                        gb.AdvanceToAddress("CalcMonStats");
                        GscPokemon enemyMon = gb.GetEnemyMon();
                        lock(WriteLock) {
                            Console.WriteLine("{0} - DVs: {1:X4}, HP: {2} {3}", state.Log + " " + action.LogString(), enemyMon.DVs.Value, enemyMon.HPType, enemyMon.HPPower);
                            if(enemyMon.HPType == GscType.Ice && enemyMon.HPPower >= 68 && enemyMon.DVs.Attack == 15 && enemyMon.DVs.Special >= 14) {
                                RaikouWriter.WriteLine("{0} - DVs: {1:X4}, HP: {2} {3}", state.Log + " " + action.LogString(), enemyMon.DVs.Value, enemyMon.HPType, enemyMon.HPPower);
                                RaikouWriter.Flush();
                            }
                        }
                    }

                    if(ret != gb.Sym["OWPlayerInput"]) {
                        continue;
                    }

                    justChangedDir = (state.FacingDir != joypad) || state.TurnFrameStatus;
                    prevChangedDir = justChangedDir;
                    facingDir = joypad;
                    canStart = true;
                    canSelect = true;
                    turnFrameStatus = false;
                    if(state.OnBike) numBikeSteps++;
                } else if(action == Action.StartB) {
                    if(!state.CanStart) {
                        continue;
                    }

                    if(Execute(gb, action, ref numRandomCalls) != gb.Sym["OWPlayerInput"]) {
                        continue;
                    }

                    justChangedDir = true;
                    prevChangedDir = false;
                    canStart = true;
                    canSelect = true;
                    turnFrameStatus = true;
                } else if(action == Action.Select) {
                    if(!state.CanSelect) {
                        continue;
                    }

                    if(Execute(gb, action, ref numRandomCalls) != gb.Sym["OWPlayerInput"]) {
                        continue;
                    }

                    justChangedDir = true;
                    prevChangedDir = false;
                    canStart = true;
                    canSelect = false;
                    turnFrameStatus = true;
                }

                RaikouSearch(gb, seenStates, new RaikouState() {
                    Log = state.Log + " " + edge.Action.LogString(),
                    R37Log = state.R37Log + " " + edge.Action.LogString(),
                    OnBike = gb.CpuRead("wPlayerState") != 0,
                    JustChangedDir = justChangedDir,
                    PrevChangedDir = prevChangedDir,
                    FacingDir = facingDir,
                    rDiv = gb.ReadRDIV(),
                    hra = gb.ReadHRA(),
                    hrs = gb.ReadHRS(),
                    Tile = edge.NextTile,
                    CanSelect = canSelect,
                    CanStart = canStart,
                    CanAPress = canAPress,
                    TurnFrameStatus = turnFrameStatus,
                    WastedFrames = 0,
                    RouteEnterTime = state.RouteEnterTime,
                    NumRandomCalls = numRandomCalls,
                    NumBikeSteps = numBikeSteps,
                });
            }
        }

        public static void InitTiles(Crystal gb) {
            GscMap map = gb.Maps["EcruteakCity"];

            map[0x0A, 0x1C].CanMoveUp = false;
            map[0x10, 0x1C].CanMoveUp = false;
            map[0x11, 0x1C].CanMoveUp = false;
            map[0x12, 0x1C].CanMoveUp = false;
            map[0x13, 0x1C].CanMoveUp = false;
            map[0x13, 0x1C].CanMoveUp = false;
            map[0x14, 0x1C].CanMoveUp = false;
            map[0x15, 0x1C].CanMoveUp = false;

            AStar.GenerateEdges(map, 0, map[0x11, 0x23], Action.Right | Action.RightA | Action.LeftA | Action.Left | Action.Down | Action.DownA | Action.UpA | Action.Up, 8);

            foreach(GscTile tile in map.Tiles) {
                if(tile.Edges.ContainsKey(0))
                    tile.Edges[0].Sort();
            }

            map = gb.Maps["Route37"];

            for(int y = 0; y <= 7; y++) {
                for(int x = 0; x <= 10; x++) {
                    GscTile tile = map[x, y];
                    if(tile.CanMoveDown && y != 7 && !(x == 6 && y == 5)) {
                        GscTile tileDown = map[x, y + 1];
                        tile.AddEdge(0, new GscEdge(Action.Down, tileDown, 0, 0));
                        tile.AddEdge(0, new GscEdge(Action.DownA, tileDown, 0, 0));
                    }

                    if(tile.CanMoveLeft && x != 0 && !(x == 7 && y == 6)) {
                        GscTile tileLeft = map[x - 1, y];
                        tile.AddEdge(0, new GscEdge(Action.Left, tileLeft, 0, 0));
                        tile.AddEdge(0, new GscEdge(Action.LeftA, tileLeft, 0, 0));
                    }

                    if(tile.CanMoveRight && x != 10 && !(x == 5 && y == 6)) {
                        GscTile tileRight = map[x + 1, y];
                        tile.AddEdge(0, new GscEdge(Action.Right, tileRight, 0, 0));
                        tile.AddEdge(0, new GscEdge(Action.RightA, tileRight, 0, 0));
                    }

                    if(tile.CanMoveUp && y != 0 && !(x == 6 && y == 7)) {
                        GscTile tileUp = map[x, y - 1];
                        tile.AddEdge(0, new GscEdge(Action.Up, tileUp, 0, 0));
                        tile.AddEdge(0, new GscEdge(Action.UpA, tileUp, 0, 0));
                    }
                    tile.AddEdge(0, new GscEdge(Action.StartB, tile, 0, 0));
                    tile.AddEdge(0, new GscEdge(Action.Select, tile, 0, 0));
                }
            }
        }

        public static void MakeSave(string save, byte facingDirection, byte frame, byte stepCount = 218) {
            byte[] baseSave = File.ReadAllBytes(save);

            baseSave[0x2000] = 0xE1; // options

            baseSave[0x2055] = 0x0;   // igt second
            baseSave[0x2056] = frame; // igt frame

            baseSave[0x206C] = facingDirection; // 0x00 = down, 0x04 = up, 0x08 = left, 0x0C = right

            baseSave[0x2045] = 0xA; // StartHour
            baseSave[0x2046] = 0x0; // StartMinute
            baseSave[0x2047] = 0x0; // StartSecond

            baseSave[0x2801] = stepCount;

            //baseSave[0x2687] = 0x40;    // Greg defeated event flag

            int csum1 = 0;
            for(int i = 0x2009; i <= 0x2B82; i++) {
                csum1 += baseSave[i] & 0xFF;
            }
            csum1 = (csum1 & 0xFFFF) ^ 0xFFFF;
            baseSave[0x2D0E] = (byte) ((csum1 / 256 & 0xFF) ^ 0xFF);
            baseSave[0x2D0D] = (byte) ((csum1 % 256 & 0xFF) ^ 0xFF);

            int csum2 = 0;
            for(int j = 0x1209; j <= 0x1D82; j++) {
                csum2 += baseSave[j] & 0xFF;
            }
            csum2 = (csum2 & 0xFFFF) ^ 0xFFFF;
            baseSave[0x1F0E] = (byte) ((csum2 / 256 & 0xFF) ^ 0xFF);
            baseSave[0x1F0D] = (byte) ((csum2 % 256 & 0xFF) ^ 0xFF);

            File.WriteAllBytes("roms/pokecrystal.sav", baseSave);
        }

        public static void MakeSave(int x, int y, byte facingDirection, byte frame, byte stepCountIncrease = 0) {
            MakeSave("raikou_base_saves/raikou_" + x + "_" + y + ".sav", facingDirection, frame, stepCountIncrease);
        }

        public struct SaveTile {

            public int X;
            public int Y;
            public byte FacingDirection;

            public SaveTile(int x, int y, byte direction) => (X, Y, FacingDirection) = (x, y, direction);
        }

        public struct IGTResult {

            public string ActionLog;
            public CpuAddress PC;
            public int NumRandoms;
        }

        public struct RaikouState {

            public string Log;
            public string R37Log;
            public bool OnBike;
            public bool JustChangedDir;
            public bool PrevChangedDir;
            public uint FacingDir;
            public byte rDiv;
            public byte hra;
            public byte hrs;
            public GscTile Tile;
            public bool CanSelect;
            public bool CanStart;
            public bool CanAPress;
            public bool TurnFrameStatus;
            public int WastedFrames;
            public ulong RouteEnterTime;
            public int NumRandomCalls;
            public int NumBikeSteps;

            public override bool Equals(object obj) {
                return obj is RaikouState state&&
                       OnBike==state.OnBike&&
                       JustChangedDir==state.JustChangedDir&&
                       PrevChangedDir==state.PrevChangedDir&&
                       FacingDir==state.FacingDir&&
                       rDiv==state.rDiv&&
                       hra==state.hra&&
                       hrs==state.hrs&&
                       Tile.X==state.Tile.X&&
                       Tile.Y==state.Tile.Y&&
                       CanSelect==state.CanSelect&&
                       CanStart==state.CanStart&&
                       TurnFrameStatus==state.TurnFrameStatus&&
                       WastedFrames==state.WastedFrames&&
                       NumRandomCalls==state.NumRandomCalls&&
                       NumBikeSteps==state.NumBikeSteps;
            }

            public override int GetHashCode() {
                var hash = new HashCode();
                hash.Add(OnBike);
                hash.Add(JustChangedDir);
                hash.Add(PrevChangedDir);
                hash.Add(FacingDir);
                hash.Add(rDiv);
                hash.Add(hra);
                hash.Add(hrs);
                hash.Add(Tile.X);
                hash.Add(Tile.Y);
                hash.Add(CanSelect);
                hash.Add(CanStart);
                hash.Add(TurnFrameStatus);
                hash.Add(WastedFrames);
                hash.Add(NumRandomCalls);
                hash.Add(NumBikeSteps);
                return hash.ToHashCode();
            }
        }

        public struct RaikouRNGState {

            public uint FacingDir;
            public byte rDiv;
            public byte hra;
            public byte hrs;
            public byte X;
            public byte Y;
            public byte NumBikeSteps;
            public byte NumRandomCalls;
            public bool OnBike;
            public bool CanSelect;
            public bool CanStart;
            public bool CanAPress;
            public bool TurnFrameStatus;
            public bool JustChangedDir;
            public bool PrevChangedDir;

            public override bool Equals(object obj) {
                return obj is RaikouRNGState state&&
                       FacingDir==state.FacingDir&&
                       rDiv==state.rDiv&&
                       hra==state.hra&&
                       hrs==state.hrs&&
                       X==state.X&&
                       Y==state.Y&&
                       NumBikeSteps==state.NumBikeSteps&&
                       NumRandomCalls==state.NumRandomCalls&&
                       OnBike==state.OnBike&&
                       CanSelect==state.CanSelect&&
                       CanStart==state.CanStart&&
                       CanAPress==state.CanAPress&&
                       TurnFrameStatus==state.TurnFrameStatus&&
                       JustChangedDir==state.JustChangedDir&&
                       PrevChangedDir==state.PrevChangedDir;
            }

            public override int GetHashCode() {
                var hash = new HashCode();
                hash.Add(FacingDir);
                hash.Add(rDiv);
                hash.Add(hra);
                hash.Add(hrs);
                hash.Add(X);
                hash.Add(Y);
                hash.Add(NumBikeSteps);
                hash.Add(NumRandomCalls);
                hash.Add(OnBike);
                hash.Add(CanSelect);
                hash.Add(CanStart);
                hash.Add(CanAPress);
                hash.Add(TurnFrameStatus);
                hash.Add(JustChangedDir);
                hash.Add(PrevChangedDir);
                return hash.ToHashCode();
            }
        }
    }
}
