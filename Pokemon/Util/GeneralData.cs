using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public abstract class Charmap {

        public static readonly byte Terminator = 0x50;
        public static readonly string TerminatorChar = "@";

        protected Dictionary<byte, string> Map;
        public byte Offset;

        public Charmap(byte offset, string charmap) {
            Offset = offset;
            Map = new Dictionary<byte, string>();
            Map[Terminator] = TerminatorChar;
            Map[0x7F] = " ";
            Map[0x4A] = "PkMn";
            Map[0x54] = "POKE";
            Map[0x52] = "<PLAYER>";
            Map[0x53] = "<RIVAL>";

            string[] chars = charmap.Split(" ");
            for(int i = 0; i < chars.Length; i++) {
                Map[(byte) (offset + i)] = chars[i];
            }
        }

        public string Decode(byte b) {
            return Map[b];
        }

        public string Decode(byte[] bytes, bool hideTerminators = true) {
            string decodedName = "";

            for(int i = 0; i < bytes.Length; i++) {
                byte b = bytes[i];
                if(b == Terminator && hideTerminators) {
                    continue;
                }
                decodedName += Map[b];
            }

            return decodedName;
        }
    }

    public enum Direction {

        Down  = 0x01,
        Up    = 0x02,
        Left  = 0x04,
        Right = 0x08,
        None,
        BoulderMovementByte2,
    }

    public static class DirectionFunctions {

        public static Direction RbySpriteDirection(byte direction) {
            switch(direction) {
                case 0x00:
                case 0xFF: return Direction.None;
                case 0x10: return Direction.BoulderMovementByte2;
                default:   return (Direction) (1 << (direction - 0xD0));
            }
        }

        public static (int, int) ToCardinal(this Direction direction) {
            (int, int) ret = (0, 0);

                 if(direction == Direction.Down) ret.Item2 = 1;
            else if(direction == Direction.Up) ret.Item2 = -1;
            else if(direction == Direction.Right) ret.Item1 = 1;
            else if(direction == Direction.Left) ret.Item1 = -1;

            return ret;
        }

        public static Direction FromString(string str) {
            str = str.ToLower();
            if(str == "left") return Direction.Left;
            else if(str == "right") return Direction.Right;
            else if(str == "down") return Direction.Down;
            else if(str == "up") return Direction.Up;
            return Direction.None;
        }

        public static Direction Opposite(this Direction direction) {
            if (direction == Direction.Right || direction == Direction.Up) {
                return (Direction) ((int) direction >> 1);
            } else {
                return (Direction) ((int) direction << 1);
            }
        }
    }

    public enum Action : uint {

        None =         0x00,
        Right =        0x01,
        Left =         0x02,
        Up =           0x04,
        Down =         0x08,
        A =            0x10,
        StartB =       0x20,
        PokedexFlash = 0x40,
        Delay =        0x80,
        RightA =       0x100,
        LeftA =        0x200,
        UpA =          0x400,
        DownA =        0x800,
        Select =       0x1000,
    }

    public static class ActionFunctions {

        private static Dictionary<string, Action> Actions = new Dictionary<string, Action>() {
            { "R",       Action.Right        },
            { "L",       Action.Left         },
            { "U",       Action.Up           },
            { "D",       Action.Down         },
            { "A",       Action.A            },
            { "S_B",     Action.StartB       },
            { "S_A_B_S", Action.PokedexFlash },
            { "Del",     Action.Delay },
            { "A+R",     Action.RightA },
            { "A+L",     Action.LeftA },
            { "A+U",     Action.UpA },
            { "A+D",     Action.DownA },
            { "SEL",     Action.Select },
        };

        public static bool IsDpad(this Action action) {
            return action == Action.Right || action == Action.Left || action == Action.Up || action == Action.Down || action == Action.RightA || action == Action.LeftA || action == Action.UpA || action == Action.DownA;
        }

        public static string LogString(this Action action) {
            return Actions.GetKeyByValue(action);
        }

        public static Action ToAction(this string str) {
            return Actions[str];
        }
    }

    public enum YoloballTypes
    {
        Normal,
        NormalSlot2,
        Select,
        SelectSlot2,
    }

    public class DVs {

        public ushort Value;

        public byte Upper {
            get { return (byte) (Value >> 4); }
        }

        public byte Lower {
            get { return (byte) (Value & 0xFF); }
        }

        public byte HP {
            get { return (byte) (((Attack & 1) << 3) | ((Defense & 1) << 2) | ((Speed & 1) << 1) | (Special & 1)); }
        }

        public byte Attack {
            get { return (byte) ((Value >> 12) & 0xF); }
        }

        public byte Defense {
            get { return (byte) ((Value >> 8) & 0xF); }
        }

        public byte Speed {
            get { return (byte) ((Value >> 4) & 0xF); }
        }

        public byte Special {
            get { return (byte) ((Value) & 0xF); }
        }

        public DVs(byte attack, byte defense, byte speed, byte special) : this((ushort) ((attack << 12) | (defense << 8) | (speed << 4) | special)) { }
        public DVs(ushort dvs) => Value = dvs;

        public override string ToString() {
            return string.Format("0x{0:X4}", Value);
        }

        public static implicit operator DVs(ushort value) { return new DVs(value); }
        public static implicit operator DVs(int value) { return new DVs((ushort) value); }
        public static implicit operator DVs(uint value) { return new DVs((ushort) value); }

        public static implicit operator ushort(DVs dvs) { return dvs.Value; }
        public static implicit operator int(DVs dvs) { return dvs.Value; }
        public static implicit operator uint(DVs dvs) { return dvs.Value; }
    }

    public class Item {

        public byte Id;
        public string Name;

        public Item(byte id, string name) => (Id, Name) = (id, name);

        public override string ToString() {
            return Name;
        }
    }

    public class ItemStack {

        public Item Item;
        public byte Quantity;

        public ItemStack(Item item, byte quantity = 1) => (Item, Quantity) = (item, quantity);

        public override string ToString() {
            return Item + " x" + Quantity;
        }
    }

    public struct EncounterState {
            public string Log;
            public RbyTile Tile;
            public int APressCounter;
            public int WastedFrames;
            public int EdgeSet;
            public int NextEdgeSet;
            public EncounterRNGState[] IGTs;
            public int AllowedAPress;

            public override bool Equals(object obj) {
                return obj is EncounterState state&&
                    Tile.X==state.Tile.X&&
                    Tile.Y==state.Tile.Y&&
                    APressCounter==state.APressCounter&&
                    WastedFrames==state.WastedFrames&&
                    EdgeSet==state.EdgeSet&&
                    NextEdgeSet==state.NextEdgeSet&&
                    IGTs==state.IGTs&&
                    AllowedAPress==state.AllowedAPress;
            }

            public override int GetHashCode() {
                var hash = new HashCode();
                hash.Add(Tile.X);
                hash.Add(Tile.Y);
                hash.Add(APressCounter);
                hash.Add(WastedFrames);
                hash.Add(EdgeSet);
                hash.Add(NextEdgeSet);
                hash.Add(IGTs);
                hash.Add(AllowedAPress);
                return hash.ToHashCode();
            }
        }

    public class EncounterRNGState {
        public byte rDiv;
        public byte hra;
        public byte hrs;
        public byte X;
        public byte Y;
        public bool CanStart;
        public bool CanAPress;
        public byte[] SaveState;

        public EncounterRNGState(Rby gb) {
                rDiv = gb.ReadRDIV();
                hra = gb.ReadHRA();
                hrs = gb.ReadHRS();
                X = gb.GetCurTile().X;
                Y = gb.GetCurTile().Y;
                CanAPress = true;
                CanStart = true;
                SaveState = gb.SaveState();
            }

        public override bool Equals(object obj) {
            return obj is EncounterRNGState state&&
                rDiv==state.rDiv&&
                hra==state.hra&&
                hrs==state.hrs&&
                X==state.X&&
                Y==state.Y&&
                CanStart==state.CanStart&&
                CanAPress==state.CanAPress;
        }

        public override int GetHashCode() {
            var hash = new HashCode();
            hash.Add(rDiv);
            hash.Add(hra);
            hash.Add(hrs);
            hash.Add(X);
            hash.Add(Y);
            hash.Add(CanStart);
            hash.Add(CanAPress);
            return hash.ToHashCode();
        }
    }
}