using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading;

namespace Pokemon {

    public static class RaikouLocation {

        public struct SaveTile {

            public int X;
            public int Y;
            public byte FacingDirection;

            public SaveTile(int x, int y, byte direction) => (X, Y, FacingDirection) = (x, y, direction);
        }

        public struct Result {

            public int Successes;
            public string NPCLog;
        }

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
            /*foreach(SaveTile savetile in SaveTiles) {
                string path = "R R ";

                for(int x = savetile.X; x < 15; x++) {
                    path += "R ";
                }

                if(savetile.Y == 0x1C) {
                    path += "D ";
                }

                path += "D D D D D D D ";
                for(int frame = 11; frame <= 13; frame++) {
                    Result res = IGT(frame, (byte) savetile.X, (byte) savetile.Y, savetile.FacingDirection, path);
                    Console.WriteLine("[frame=" + frame + ", x=" + savetile.X + ", y=" + savetile.Y + ", dir=" + savetile.FacingDirection + "] " + path + " - " + res.Successes + "/60 " + res.NPCLog);
                }
            }*/

            //Result res = IGT(13, 14, 28, 12, "R R L L L R R R R D D D D D D D U U U U U S_B R D D D D D D");
            //Console.WriteLine(res.Successes + " " + res.NPCLog);

#if true_
            InitTiles(new Crystal());
            List<Thread> threads = new List<Thread>();
            foreach(SaveTile saveTile in SaveTiles) {
                for(int frame = 11; frame <= 13; frame++) {
                    Thread t = new Thread(() => {
                        StartLocationSearch(frame, (byte) saveTile.X, (byte) saveTile.Y, saveTile.FacingDirection);
                    });
                    t.Start();
                    threads.Add(t);
                    Thread.Sleep(200);
                }
            }
#endif

            //FindClusters("locationmanips.txt");
            IGT(11, 13, 29, 12, "R R R SEL R D D D D D D U A+D R L R D D D");
        }


        const int ActionBuffer = 3;
        const int NumExtraActions = 40;

        public static void FindClusters(string path) {
            string[] lines = File.ReadAllLines(path);

            HashSet<(string, string, string)> seenClusters = new HashSet<(string, string, string)>();
            List<Cluster> clusters = new List<Cluster>();

            HashSet<string> seenLines = new HashSet<string>();

            foreach(string line1 in lines) {
                if(line1.StartsWith("[frame=11")) {
                    Frame frame1 = GetFrame(line1);
                    foreach(string line2 in lines) {
                        if(line2.StartsWith("[frame=12")) {
                            Frame frame2 = GetFrame(line2);

                            if(!(frame1.X == frame2.X && frame1.Y == frame2.Y && frame1.Dir == frame2.Dir && frame1.Path == frame2.Path)) {
                                continue;
                            }

                            foreach(string line3 in lines) {
                                if(line3.StartsWith("[frame=13")) {
                                    Frame frame3 = GetFrame(line3);

                                    if(!(frame1.X == frame3.X && frame1.Y == frame3.Y && frame1.Dir == frame3.Dir && frame1.Path == frame3.Path)) {
                                        continue;
                                    }

                                    /*if(frame1.Path.Contains("D U U U D") || frame2.Path.Contains("D U U U D") || frame3.Path.Contains("D U U U D")) {
                                        continue;
                                    }*/

                                    if(!seenClusters.Add((line1, line2, line3))) continue;

                                    string frame1NPC = string.Join(" ", frame1.NPC);
                                    string frame2NPC = string.Join(" ", frame2.NPC);
                                    string frame3NPC = string.Join(" ", frame3.NPC);

                                    if(frame1NPC != frame2NPC && frame2NPC != frame3NPC && frame1NPC != frame3NPC) {
                                        if(seenLines.Add(line1)) Console.WriteLine(line1);
                                        if(seenLines.Add(line2)) Console.WriteLine(line2);
                                        if(seenLines.Add(line3)) Console.WriteLine(line3);
                                    }

                                    /*List<Frame> frames = new List<Frame>() {
                                        frame1, frame2, frame3
                                    };

                                    int firstDifferent = 10000;
                                    Frame firstDifferentFrame = default;
                                    int minNPCMoves = Math.Min(frame1.NPC.Count, Math.Min(frame2.NPC.Count, frame3.NPC.Count));

                                    for(int i = 0; i < frames.Count; i++) {
                                        Frame frame = frames[i];
                                        List<Frame> others = frames.Except(new List<Frame>() { frame}).ToList();
                                        for(int j = 0; j < minNPCMoves; j++) {
                                            if(others.TrueForAll(f => f.NPC[j].Item1 != frame.NPC[j].Item1 || f.NPC[j].Item2 != frame.NPC[j].Item2)) {
                                                int differenceIndex = frame.NPC[j].Item1;
                                                if(differenceIndex < firstDifferent) {
                                                    firstDifferent = differenceIndex;
                                                    firstDifferentFrame = frame;
                                                }
                                            }
                                        }
                                    }

                                    if(firstDifferent == 10000) {
                                        break;
                                    }

                                    Dictionary<Frame, int> differenceIndices = new Dictionary<Frame, int>();
                                    differenceIndices[firstDifferentFrame] = firstDifferent + ActionBuffer;

                                    List<Frame> o = frames.Except(new List<Frame>() { firstDifferentFrame}).ToList();
                                    minNPCMoves = Math.Min(o[0].NPC.Count, o[1].NPC.Count);

                                    for(int j = 0; j < minNPCMoves; j++) {
                                        if(o[0].NPC[j].Item1 != o[1].NPC[j].Item1 || o[0].NPC[j].Item2 != o[1].NPC[j].Item2) {
                                            int min = Math.Min(o[0].NPC[j].Item1, o[1].NPC[j].Item1) + ActionBuffer;
                                            differenceIndices[o[0]] = min;
                                            differenceIndices[o[1]] = min;
                                        }
                                    }

                                    if(differenceIndices.Count == 3) {

                                        if(firstDifferentFrame.Actions.Length < differenceIndices[firstDifferentFrame] || o[0].Actions.Length < differenceIndices[firstDifferentFrame] || o[1].Actions.Length < differenceIndices[firstDifferentFrame]) {
                                            continue;
                                        }

                                        string[] cut1 = firstDifferentFrame.Actions.Subarray(0, differenceIndices[firstDifferentFrame]);
                                        string[] cut2 = o[0].Actions.Subarray(0, differenceIndices[firstDifferentFrame]);
                                        string[] cut3 = o[1].Actions.Subarray(0, differenceIndices[firstDifferentFrame]);

                                        if(Enumerable.SequenceEqual(cut1, cut2) && Enumerable.SequenceEqual(cut2, cut3)) {

                                            if(o[0].Actions.Length < differenceIndices[o[0]] || o[1].Actions.Length < differenceIndices[o[1]]) {
                                                continue;
                                            }

                                            string[] cut4 = o[0].Actions.Subarray(0, Math.Min(differenceIndices[o[0]], o[0].Actions.Length));
                                            string[] cut5 = o[1].Actions.Subarray(0, Math.Min(differenceIndices[o[1]], o[1].Actions.Length));
                                            if(Enumerable.SequenceEqual(cut4, cut5)) {
                                                if(seenLines.Add(line1)) Console.WriteLine(line1);
                                                if(seenLines.Add(line2)) Console.WriteLine(line2);
                                                if(seenLines.Add(line3)) Console.WriteLine(line3);
                                            }
                                        }
                                    }*/
                                }
                            }
                        }
                    }
                }
            }
        }

        public static Frame GetFrame(string line) {
            Frame result = new Frame() {};

            result.X = Convert.ToByte(line.Substring(line.IndexOf("x=")+2, 2));
            result.Y = Convert.ToByte(line.Substring(line.IndexOf("y=")+2, 2));
            result.Dir= Convert.ToByte(line[(line.IndexOf("dir=")+4)..line.IndexOf("]")]);
            result.Path = line[(line.IndexOf("]")+2)..line.IndexOf("-")].Trim();
            result.Actions = result.Path.Split(" ");
            result.NPC = new List<(int, string)>();

            string[] splitarray = line[(line.IndexOf("(")+1)..(line.IndexOf(")"))].Split(";");

            foreach(string entry in splitarray) {
                string val = entry.Trim();
                int actionIndex = Convert.ToInt32(val.Substring(0, val.IndexOf(" ")));
                string action = val.Substring(val.IndexOf(" ") + 1);
                result.NPC.Add((actionIndex, action));
            }

            return result;
        }

        public struct Frame {

            public byte X;
            public byte Y;
            public byte Dir;
            public string Path;
            public string[] Actions;
            public List<(int, string)> NPC;

            public int ExtraActions() {
                return Actions.Length - ((17 + 29) - (X + Y));
            }
        }

        public class Cluster : IComparable<Cluster> {

            public byte X;
            public byte Y;
            public (string, string, string) Paths;

            public int CompareTo(Cluster other) {
                int accum1 = 0;
                accum1 += Paths.Item1.Length - ((X + Y) - (14 + 29));
                accum1 += Paths.Item2.Length - ((X + Y) - (14 + 29));
                accum1 += Paths.Item3.Length - ((X + Y) - (14 + 29));

                int accum2 = 0;
                accum2 += other.Paths.Item1.Length - ((other.X + other.Y) - (14 + 29));
                accum2 += other.Paths.Item2.Length - ((other.X + other.Y) - (14 + 29));
                accum2 += other.Paths.Item3.Length - ((other.X + other.Y) - (14 + 29));

                return accum1 - accum2;
            }
        }

        public static StreamWriter writer = new StreamWriter("locationmanips.txt");
        public static object savelock = new object();
        public static object writelock = new object();

        public static void StartLocationSearch(int frame, byte x, byte y, byte dir) {
            Crystal gb;
            lock(savelock) {
                MakeSave(x, y, dir, 0);
                gb = new Crystal(true);
                gb.AdvanceToAddress(0x100);
                gb.SetTimeSec(3000);
                gb.Press(Joypad.Start, Joypad.Start, Joypad.A);
                gb.AdvanceToAddress("GetJoypad");
                for(int i = 0; i <= frame; i++) {
                    gb.AdvanceFrame();
                }
            }

            byte[] initialState = gb.SaveState();
            gb.AdvanceFrame(Joypad.A);
            gb.AdvanceToAddress("OWPlayerInput");

            GscTile tile = gb.GetCurTile();

            LocationSearch(gb, new LocationState() {
                Tile = tile,
                Log = "[frame=" + frame + ", x=" + x + ", y=" + y + ", dir=" + dir + "] ",
                Path = "",
                HasSelected = false,
                JustChangedDirection = false,
                FacingDirection = (uint) Action.Right << 4,
                Cost = 0,
                CanAPress = false,
                hra = gb.ReadHRA(),
                hrs = gb.ReadHRS(),
                rdiv = gb.ReadRDIV(),
            }, frame, x, y, dir, initialState, new HashSet<LocationState>());
        }


        public static void Test(string file) {
            string[] lines = File.ReadAllLines(file);

            foreach(string line in lines) {
                Frame frame = GetFrame(line);
                Crystal gb;
                MakeSave(frame.X, frame.Y, frame.Dir, 0);
                gb = new Crystal(true);
                gb.AdvanceToAddress(0x100);
                gb.SetTimeSec(3000);
                gb.Press(Joypad.Start, Joypad.Start, Joypad.A);
                gb.AdvanceToAddress("GetJoypad");
                for(int i = 0; i <= 11; i++) {
                    gb.AdvanceFrame();
                }

                const int ss = 58;
                byte[] initialState = gb.SaveState();
                Result f12 = Test2(gb, initialState, frame.Path, 1);
                if(f12.Successes >= ss) {
                    Result f13 = Test2(gb, initialState, frame.Path, 2);
                    if(f13.Successes >= ss) {
                        Console.WriteLine("[frame={0}, x={1}, y={2}, dir={3}] {4}", 12, frame.X, frame.Y, frame.Dir, frame.Path);
                        Console.WriteLine("[frame={0}, x={1}, y={2}, dir={3}] {4}", 13, frame.X, frame.Y, frame.Dir, frame.Path);
                    }
                }
            }
        }

        private static Result Test2(Crystal gb, byte[] state, string path, int frame) {
            Result result = new Result() { };

            for(byte i = 0; i < 60; i++) {
                gb.LoadState(state);
                for(int j = 0; j < frame; j++) gb.AdvanceFrame();
                gb.CpuWrite("wGameTimeFrames", i);
                gb.AdvanceFrame(Joypad.A);
                gb.AdvanceToAddress("OWPlayerInput");

                string[] actions = path.Trim().Split(" ");

                byte npcX = 0x19;
                byte npcDir = 0;
                string npcLog = "";

                foreach(string action in actions) {
                    if(gb.Execute(action.ToAction()) != gb.Sym["OWPlayerInput"]) {
                        Console.WriteLine(gb.Sym.GetAddressName(new CpuAddress((uint) gb.GetRegister(RegisterIndex.PC))));
                        break;
                    }

                    byte playerX = gb.CpuRead("wXCoord");
                    byte playerY = gb.CpuRead("wYCoord");

                    if(gb.CpuRead("wObject1Sprite") != 0 && gb.CpuReadWord("wMapGroup") == 0x0409) {
                        byte newX = gb.CpuRead("wObject1StandingMapX");
                        byte newDir = gb.CpuRead("wObject1Facing");

                        if(newDir != 0xFF) {
                            if(newDir > 0xC) newDir = 0xC;
                            else if(newDir < 0xC) newDir = 0x8;
                        }

                        if(newX > npcX) {
                            npcLog += "R->";
                            newDir = 0xC;
                        } else if(newX < npcX) {
                            npcLog += "L->";
                            newDir = 0x8;
                        } else if(playerY <= 0x21 && npcDir != newDir && newDir != 0xFF) {

                            if(newDir == 0xC) npcLog += "r->";
                            else if(newDir == 0x8) npcLog += "l->";
                        }

                        npcX = newX;
                        npcDir = newDir;
                    }

                    if(playerY == 0x23 || playerX == 13) {
                        npcX = 0x19;
                    }
                }

                if(gb.CpuReadWord("wRoamMon1MapGroup") == 0x0A04) {
                    result.Successes++;
                }

                if(result.NPCLog == null) result.NPCLog = npcLog.Substring(0, npcLog.Length - 2);
            }

            return result;
        }

        public const int MaxCost = 5 * 16;

        public static void LocationSearch(Crystal gb, LocationState state, int frame, byte x, byte y, byte dir, byte[] initialState, HashSet<LocationState> seenStates) {
            if(!seenStates.Add(state)) {
                return;
            }

            if(state.Tile.Y == 0x23) {
                byte[] savestate = gb.SaveState();
                gb.Execute(Action.Down);

                if(gb.CpuReadWord("wRoamMon1MapGroup") == 0x0A04) {

                    Result result = new Result() {
                        Successes = 60,
                    };

                    for(byte i = 0; i < 60 && result.Successes >= 57; i++) {
                        gb.LoadState(initialState);
                        gb.CpuWrite("wGameTimeFrames", i);
                        gb.AdvanceFrame(Joypad.A);
                        gb.AdvanceToAddress("OWPlayerInput");

                        string[] actions = (state.Path + " D").Trim().Split(" ");

                        byte npcX = 0x19;
                        byte npcDir = 0;
                        string npcLog = "";

                        for(int j = 0; j < actions.Length; j++) {
                            string action = actions[j];
                            if(gb.Execute(action.ToAction()) != gb.Sym["OWPlayerInput"]) {
                                break;
                            }

                            byte playerX = gb.CpuRead("wXCoord");
                            byte playerY = gb.CpuRead("wYCoord");

                            if(gb.CpuRead("wObject1Sprite") != 0 && gb.CpuReadWord("wMapGroup") == 0x0409) {
                                byte newX = gb.CpuRead("wObject1StandingMapX");
                                byte newDir = gb.CpuRead("wObject1Facing");

                                if(newDir != 0xFF) {
                                    if(newDir > 0xC) newDir = 0xC;
                                    else if(newDir < 0xC) newDir = 0x8;
                                }

                                if(newX > npcX) {
                                    npcLog += j + " R; ";
                                    newDir = 0xC;
                                } else if(newX < npcX) {
                                    npcLog += j + " L; ";
                                    newDir = 0x8;
                                } else if(playerY <= 0x21 && npcDir != newDir && newDir != 0xFF) {

                                    if(newDir == 0xC) npcLog += j + " r; ";
                                    else if(newDir == 0x8) npcLog += j + " l; ";
                                }

                                npcX = newX;
                                npcDir = newDir;
                            }

                            if(playerY == 0x23 || playerX == 13) {
                                npcX = 0x19;
                                npcDir = 0;
                            }
                        }

                        if(gb.CpuReadWord("wRoamMon1MapGroup") != 0x0A04) {
                            result.Successes--;
                        }

                        if(result.NPCLog == null) result.NPCLog = npcLog.Substring(0, npcLog.Length - 2);
                    }

                    if(result.Successes >= 57) {
                        lock(writelock) {
                            writer.WriteLine(state.Log + " D - " + result.Successes + " (" + result.NPCLog + ")");
                            writer.Flush();
                        }
                    }
                }

                gb.LoadState(savestate);
            }

            byte[] save = gb.SaveState();

            foreach(GscEdge edge in state.Tile.Edges[0]) {
                gb.LoadState(save);
                Action action = edge.Action;
                bool justChangedDir = false;
                bool prevChangedDir = false;
                bool has2TileTurn = state.Has2TileTurn;
                bool hasSelected = state.HasSelected;
                bool canAPress = false;
                uint facingDir = state.FacingDirection;

                if(state.Cost + edge.Cost > MaxCost) {
                    continue;
                }

                if(action.IsDpad()) {
                    if(!state.HasSelected && state.Tile.X <= 0x10 && (action == Action.Down || action == Action.Up)) {
                        continue;
                    }

                    uint joypad;
                    if(action == Action.LeftA || action == Action.RightA || action == Action.UpA || action == Action.DownA) {
                        if(!state.HasSelected || !state.CanAPress) {
                            continue;
                        }
                        joypad = (uint) action >> 4;
                    } else {
                        joypad = (uint) action << 4;
                        canAPress = true;
                    }

                    if(!state.HasSelected && (state.FacingDirection != joypad && (state.JustChangedDirection || state.PrevChangedDirection))) {
                        continue;
                    }

                    if(gb.Execute(action) != gb.Sym["OWPlayerInput"]) {
                        continue;
                    }

                    justChangedDir = state.FacingDirection != joypad;
                    prevChangedDir = state.JustChangedDirection;
                    if(!has2TileTurn && justChangedDir && !prevChangedDir) has2TileTurn = true;
                    facingDir = joypad;
                } else if(action == Action.Select) {
                    if(state.HasSelected || state.JustChangedDirection) {
                        continue;
                    }

                    if(gb.Execute(action) != gb.Sym["OWPlayerInput"]) {
                        continue;
                    }

                    justChangedDir = false;
                    prevChangedDir = false;
                    hasSelected = true;
                    canAPress = false;
                }

                LocationSearch(gb, new LocationState() {
                    Log = state.Log + " " + action.LogString(),
                    Path = state.Path + " " + action.LogString(),
                    Tile = edge.NextTile,
                    FacingDirection = facingDir,
                    JustChangedDirection = justChangedDir,
                    PrevChangedDirection = prevChangedDir,
                    HasSelected = hasSelected,
                    Cost = state.Cost + edge.Cost * (hasSelected ? 2 : 1),
                    Has2TileTurn = has2TileTurn,
                    CanAPress = canAPress,
                    hra = gb.ReadHRA(),
                    hrs = gb.ReadHRS(),
                    rdiv = gb.ReadRDIV(),
                }, frame, x, y, dir, initialState, seenStates);
            }
        }

        public struct LocationState {

            public string Log;
            public string Path;
            public GscTile Tile;
            public byte hra;
            public byte hrs;
            public byte rdiv;
            public uint FacingDirection;
            public bool JustChangedDirection;
            public bool PrevChangedDirection;
            public bool Has2TileTurn;
            public bool HasSelected;
            public bool CanAPress;
            public int Cost;

            public override bool Equals(object obj) {
                return obj is LocationState state&&
                       Tile.X==state.Tile.X&&
                       Tile.Y==state.Tile.Y&&
                       hra==state.hra&&
                       hrs==state.hrs&&
                       rdiv==state.rdiv;
            }

            public override int GetHashCode() {
                var hash = new HashCode();
                hash.Add(Tile.X);
                hash.Add(Tile.Y);
                hash.Add(hra);
                hash.Add(hrs);
                hash.Add(rdiv);
                return hash.ToHashCode();
            }
        }

        public static void InitTiles(Crystal gb) {
            GscMap map = gb.Maps["EcruteakCity"];
            AStar.GenerateEdges(map, 0, map[0x11, 0x23], Action.Left | Action.Right | Action.Down| Action.Up | Action.StartB | Action.Select | Action.LeftA | Action.RightA | Action.DownA | Action.UpA, 8);

            foreach(GscTile tile in map.Tiles) {
                if(tile.Edges.ContainsKey(0)) {
                    tile.Edges[0].Sort();
                    tile.GetEdge(0, Action.Select).Cost = 0;
                }
            }
        }

        public static void Record(int frame, byte x, byte y, byte dir, string path) {
            MakeSave(x, y, dir, 0);
            Crystal gb = new Crystal(true);
            gb.AdvanceToAddress(0x100);
            gb.SetTimeSec(3000);
            gb.Press(Joypad.Start, Joypad.Start, Joypad.A);
            gb.AdvanceToAddress("GetJoypad");
            for(int i = 0; i <= frame; i++) {
                gb.AdvanceFrame();
            }

            gb.CpuWrite("wGameTimeFrames", 0);
            gb.AdvanceFrame(Joypad.A);
            gb.Record("test");
            gb.AdvanceToAddress("OWPlayerInput");
            gb.Execute(path);

            gb.Dispose();
        }

        public static void RecordCluster(byte x, byte y, byte dir, params string[] paths) {
            MakeSave(x, y, dir, 0);
            Crystal gb = new Crystal(true);
            gb.AdvanceToAddress(0x100);
            gb.SetTimeSec(3000);
            gb.Press(Joypad.Start, Joypad.Start, Joypad.A);
            gb.AdvanceToAddress("GetJoypad");
            for(int i = 0; i <= 11-2; i++) {
                gb.AdvanceFrame();
            }

            byte[] savestate = gb.SaveState();
            gb.Record("test");

            for(int i = 0; i < 3+4; i++) {
                gb.LoadState(savestate);
                for(int j = 0; j < i; j++) {
                    gb.AdvanceFrame();
                }
                gb.CpuWrite("wGameTimeFrames", 0);
                gb.AdvanceFrame(Joypad.A);
                gb.AdvanceToAddress("OWPlayerInput");
                gb.Execute(i >= paths.Length ? paths[paths.Length - 1] : paths[i]);
            }
            gb.Dispose();
        }

        public static Result IGT(int frame, byte x, byte y, byte dir, string path) {
            MakeSave(x, y, dir, 0);
            Crystal gb = new Crystal(true);
            gb.AdvanceToAddress(0x100);
            gb.SetTimeSec(3000);
            gb.Press(Joypad.Start, Joypad.Start, Joypad.A);
            gb.AdvanceToAddress("GetJoypad");
            for(int i = 0; i <= frame; i++) {
                gb.AdvanceFrame();
            }

            byte[] igtState = gb.SaveState();

            Result result = new Result() { };

            for(byte i = 0; i < 60; i++) {
                gb.LoadState(igtState);
                gb.CpuWrite("wGameTimeFrames", i);
                gb.AdvanceFrame(Joypad.A);
                gb.AdvanceToAddress("OWPlayerInput");

                string[] actions = path.Trim().Split(" ");

                byte npcX = 0x19;
                byte npcDir = 0;
                string npcLog = "";

                foreach(string action in actions) {
                    if(gb.Execute(action.ToAction()) != gb.Sym["OWPlayerInput"]) {
                        Console.WriteLine(gb.Sym.GetAddressName(new CpuAddress((uint) gb.GetRegister(RegisterIndex.PC))));
                        break;
                    }

                    byte playerX = gb.CpuRead("wXCoord");
                    byte playerY = gb.CpuRead("wYCoord");

                    if(gb.CpuRead("wObject1Sprite") != 0 && gb.CpuReadWord("wMapGroup") == 0x0409) {
                        byte newX = gb.CpuRead("wObject1StandingMapX");
                        byte newDir = gb.CpuRead("wObject1Facing");

                        if(newDir != 0xFF) {
                            if(newDir > 0xC) newDir = 0xC;
                            else if(newDir < 0xC) newDir = 0x8;
                        }

                        if(newX > npcX) {
                            npcLog += "R->";
                            newDir = 0xC;
                        } else if(newX < npcX) {
                            npcLog += "L->";
                            newDir = 0x8;
                        } else if(playerY <= 0x21 && npcDir != newDir && newDir != 0xFF) {

                            if(newDir == 0xC) npcLog += "r->";
                            else if(newDir == 0x8) npcLog += "l->";
                        }

                        npcX = newX;
                        npcDir = newDir;
                    }

                    if(playerY == 0x23 || playerX == 13) {
                        npcX = 0x19;
                    }
                }

                if(gb.CpuReadWord("wRoamMon1MapGroup") == 0x0A04) {
                    result.Successes++;
                }

                if(result.NPCLog == null) result.NPCLog = npcLog.Substring(0, npcLog.Length - 2);
            }

            return result;
        }

        public static void MakeSave(string save, byte facingDirection, byte frame, byte stepCount = 221) {
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

        public static void MakeSave(int x, int y, byte facingDirection, byte frame, byte stepCount = 0) {
            MakeSave("raikou_base_saves/raikou_" + x + "_" + y + ".sav", facingDirection, frame, stepCount);
        }
    }
}
