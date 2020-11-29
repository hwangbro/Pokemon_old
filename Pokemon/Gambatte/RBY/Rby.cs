using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Pokemon {

    public class RbyCharmap : Charmap {

        public RbyCharmap() : base(0x80, "A B C D E F G H I J K L M N O P " +
                                         "Q R S T U V W X Y Z ( ) : ; [ ] " +
                                         "a b c d e f g h i j k l m n o p " +
                                         "q r s t u v w x y z E 'd 'l 's 't 'v " +
                                         "_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ " +
                                         "_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ " +
                                         "' _ _ - _ _ ? ! . _ _ _ _ _ _ M " +
                                         "_ * . / , F 0 1 2 3 4 5 6 7 8 9 ") { }
    }

    public enum RbyType : byte {

        Normal,
        Fighting,
        Flying,
        Poison,
        Ground,
        Rock,
        Bird,
        Bug,
        Ghost,
        Fire = 20,
        Water,
        Grass,
        Electric,
        Psyshic,
        Ice,
        Dragon
    }

    public class RbySpecies {

        public Rby Game;
        public string Name;
        public byte IndexNumber;
        public byte PokedexNumber;
        public byte BaseHP;
        public byte BaseAttack;
        public byte BaseDefense;
        public byte BaseSpeed;
        public byte BaseSpecial;
        public RbyType Type1;
        public RbyType Type2;
        public byte CatchRate;
        public byte BaseExpYield;
        public RomAddress FrontSpritePointer;
        public RomAddress BackSpritePointer;

        public RbySpecies(Rby game, string name, byte indexNumber, byte[] data, int offset = 0) {
            Game = game;
            Name = name;
            IndexNumber = indexNumber;
            PokedexNumber = data[offset + 0];
            BaseHP = data[offset + 1];
            BaseAttack = data[offset + 2];
            BaseDefense = data[offset + 3];
            BaseSpeed = data[offset + 4];
            BaseSpecial = data[offset + 5];
            Type1 = (RbyType) data[offset + 6];
            Type2 = (RbyType) data[offset + 7];
            CatchRate = data[offset + 8];
            BaseExpYield = data[offset + 9];
            ushort frontSpriteAddress = data.ReadWord(offset + 11);
            ushort backSpriteAddress = data.ReadWord(offset + 13);

            byte spriteBank = 0xD;
            if(indexNumber == 0x15 && game.GetType() != typeof(Yellow)) {
                spriteBank = 1;
            } else if(indexNumber == 0xB6) {
                spriteBank = 0xB;
            } else if(indexNumber < 0x1F) {
                spriteBank = 0xB;
            } else if(indexNumber < 0x4A) {
                spriteBank = 0xB;
            } else if(indexNumber < 0x74 || indexNumber == 0x74 && frontSpriteAddress > 0x7000) {
                spriteBank = 0xB;
            } else if(indexNumber < 0x99 || indexNumber == 0x99 && frontSpriteAddress > 0x7000) {
                spriteBank = 0xC;
            }

            FrontSpritePointer = new RomAddress(spriteBank, frontSpriteAddress);
            BackSpritePointer = new RomAddress(spriteBank, backSpriteAddress);
        }

        public static implicit operator byte(RbySpecies species) { return species.IndexNumber; }
    }

    public enum RbyMoveEffect : byte {

        NoAdditional,
        Unused01,
        PoisonSide1,
        DrainHp,
        BurnSide1,
        FreezeSide,
        ParalyzeSide1,
        Explode,
        DreamEater,
        MirrorMove,
        AttackUp1,
        DefenseUp1,
        SpeedUp1,
        SpecialUp1,
        AccuracyUp1,
        EvasionUp1,
        PayDay,
        Swift,
        AttackDown1,
        DefenseDown1,
        SpeedDown1,
        SpecialDown1,
        AccuracyDown1,
        EvasionDown1,
        Conversion,
        Haze,
        Bide,
        ThrashPetalDance,
        SwitchAndTeleport,
        TwoToFiveAttacks,
        Unused1e,
        FlinchSide1,
        Sleep,
        PoisonSide2,
        BurnSide2,
        Unused23,
        ParalyzeSide2,
        FlinchSide2,
        Ohko,
        Charge,
        SuperFang,
        SpecialDamage,
        Trapping,
        Fly,
        AttackTwice,
        JumpKick,
        Mist,
        FocusEnergy,
        Recoil,
        Confusion,
        AttackUp2,
        DefenseUp2,
        SpeedUp2,
        SpecialUp2,
        AccuracyUp2,
        EvasionUp2,
        Heal,
        Transform,
        AttackDown2,
        DefenseDown2,
        SpeedDown2,
        SpecialDown2,
        AccuracyDown2,
        EvasionDown2,
        LightScreen,
        Reflect,
        Poison,
        Paralyze,
        AttackDownSide,
        DefenseDownSide,
        SpeedDownSide,
        SpecialDownSide,
        Unused48,
        Unused49,
        Unused4a,
        Unused4b,
        ConfusionSide,
        Twineedle,
        Unused4e,
        Substitute,
        HyperBeam,
        Rage,
        Mimic,
        Metronome,
        LeechSeed,
        Splash,
        Disable,
    }

    public class RbyMove {

        public Rby Game;
        public string Name;
        public byte Id;
        public byte Animation;
        public RbyMoveEffect Effect;
        public byte Power;
        public RbyType Type;
        public byte Accuracy;
        public byte PP;

        public RbyMove(Rby game, string name, byte[] data, int offset = 0) {
            Game = game;
            Name = name;
            Id = data[offset + 0];
            Animation = data[offset + 0];
            Effect = (RbyMoveEffect) data[offset + 1];
            Power = data[offset + 2];
            Type = (RbyType) data[offset + 3];
            Accuracy = data[offset + 4];
            PP = data[offset + 5];
        }

        public static implicit operator byte(RbyMove move) { return move.Id; }
    }

    public enum RbyPermission : byte {

        Indoor,
        Cave,
        Outdoor
    }

    public class RbyTileset {

        public Rby Game;
        public byte Id;
        public byte Bank;
        public ushort BlockPointer;
        public ushort GfxPointer;
        public ushort CollisionPointer;
        public byte[] CounterTiles;
        public byte GrassTile;
        public RbyPermission Permission;
        public byte[] CollisionData;
        public List<(byte, byte)> TilePairCollisions;

        public RbyTileset(Rby game, byte id, byte bank, ushort blockPointer, ushort gfxPointer, ushort collisionPointer, byte[] counterTiles, byte grassTile, RbyPermission permission, byte[] collisionData, List<(byte, byte)> tilePairCollisions) => (Game, Id, Bank, BlockPointer, GfxPointer, CollisionPointer, CounterTiles, GrassTile, Permission, CollisionData, TilePairCollisions) = (game, id, bank, blockPointer, gfxPointer, collisionPointer, counterTiles, grassTile, permission, collisionData, tilePairCollisions);

        public static implicit operator byte(RbyTileset tileset) { return tileset.Id; }
    }

    public class RbyConnection {

        public RbyMap Map;
        public byte MapId;
        public ushort Source;
        public ushort Destination;
        public byte Length;
        public byte MapWidth;
        public byte YAlignment;
        public byte XAlignment;
        public ushort Window;

        public RbyMap DestinationMap {
            get { return Map.Game.Maps[MapId]; }
        }

        public RbyConnection(RbyMap map, byte[] data, int offset = 0) {
            Map = map;
            MapId = data[offset];
            Source = data.ReadWord(offset + 1);
            Destination = data.ReadWord(offset + 3);
            Length = data[offset + 5];
            MapWidth = data[offset + 6];
            YAlignment = data[offset + 7];
            XAlignment = data[offset + 8];
            Window = data.ReadWord(offset + 9);
        }
    }

    public class RbyWarp {

        public RbyMap Origin;
        public byte X;
        public byte Y;
        public byte Index;
        public RbyMap Destination;
        public byte DestinationIndex;
        public bool Allowed;

        public RbyTile SourceTile {
            get { return Origin.Tiles[X, Y]; }
        }

        public RbyWarp DestinationWarp {
            get { return Destination.Warps[DestinationIndex]; }
        }

        public RbyTile DestinationTile {
            get { return Destination.Tiles[DestinationWarp.X, DestinationWarp.Y]; }
        }

        public RbyWarp(RbyMap origin, byte x, byte y, byte index, RbyMap destination, byte destinationIndex) => (Origin, X, Y, Index, Destination, DestinationIndex, Allowed) = (origin, x, y, index, destination, destinationIndex, false);

        public override string ToString() {
            return string.Format("{0} at index {1} => {2} at index {3}", SourceTile, Index, DestinationTile, DestinationIndex);
        }

        public static implicit operator byte(RbyWarp warp) { return warp.Index; }
    }

    public class RbySign {

        public RbyMap Map;
        public byte X;
        public byte Y;
        public byte ScriptId;
        public string Text;

        public RbyTile Tile {
            get { return Map.Tiles[X, Y]; }
        }

        public string OneLineText {
            get { return Text.Replace('\n', ' '); }
        }

        public RbySign(RbyMap map, byte x, byte y, byte scriptId, string text) => (Map, X, Y, ScriptId, Text) = (map, x, y, scriptId, text);

        public override string ToString() {
            return string.Format("{0}: {1}", Tile, OneLineText);
        }
    }

    public enum RbySpriteMovement : byte {

        Turn = 0xFD,
        Walk = 0xFE,
        Stay = 0xFF,
    }

    public class RbySprite {

        public RbyMap Map;
        public byte Id;
        public byte PictureId;
        public byte X;
        public byte Y;
        public RbySpriteMovement Movement;
        public Direction Direction;
        public byte Range;

        public RbyTile Tile {
            get { return Map.Tiles[X, Y]; }
        }

        public RbySprite(RbyMap map, byte id, byte pictureId, byte x, byte y, RbySpriteMovement movement, Direction direction, byte range) => (Map, Id, PictureId, X, Y, Movement, Direction, Range) = (map, id, pictureId, x, y, movement, direction, range);

        public override string ToString() {
            return string.Format("{0}", Tile);
        }

        public static implicit operator byte(RbySprite sprite) { return sprite.Id; }
    }

    public class RbyTrainerClass {

        public byte Id;
        public string Name;
        public List<List<RbyPokemon>> Teams;

        public RbyTrainerClass(byte id, string name, List<List<RbyPokemon>> teams) => (Id, Name, Teams) = (id, name, teams);

        public override string ToString() {
            return Name;
        }

        public static implicit operator byte(RbyTrainerClass trainerClass) { return trainerClass.Id; }
    }

    public class RbyTrainer : RbySprite {

        public RbyTrainerClass TrainerClass;
        public byte TrainerId;
        public byte EventFlagBit;
        public byte ViewRange;
        public ushort EventFlagAddress;

        public List<RbyPokemon> Team {
            get { return TrainerClass.Teams[TrainerId - 1]; }
        }

        public RbyTrainer(RbySprite baseSprite, RbyTrainerClass trainerclass, byte trainerId, byte eventFlagBit, byte viewRange, ushort eventFlagAddress) : base(baseSprite.Map, baseSprite.Id, baseSprite.PictureId, baseSprite.X, baseSprite.Y, baseSprite.Movement, baseSprite.Direction, baseSprite.Range) {
            (TrainerClass, TrainerId, EventFlagBit, ViewRange, EventFlagAddress) = (trainerclass, trainerId, eventFlagBit, viewRange, eventFlagAddress);
        }

        public override string ToString() {
            return base.ToString() + string.Format(": {0} [{1}]", TrainerClass, string.Join(", ", Team));
        }
    }

    public class RbyItemball : RbySprite {

        public Item Item;

        public RbyItemball(RbySprite baseSprite, Item item) : base(baseSprite.Map, baseSprite.Id, baseSprite.PictureId, baseSprite.X, baseSprite.Y, baseSprite.Movement, baseSprite.Direction, baseSprite.Range) {
            Item = item;
        }

        public override string ToString() {
            return base.ToString() + string.Format(": {0}", Item);
        }
    }

    public class RbyPokemon {

        public RbySpecies Species;
        public ushort HP;
        public byte Status;
        public byte Status2;
        public RbyMove[] Moves;
        public DVs DVs;
        public byte[] PP;
        public byte Level;
        public ushort MaxHP;
        public ushort Attack;
        public ushort Defense;
        public ushort Speed;
        public ushort Special;

        public bool Asleep {
            get { return SleepCounter != 0; }
        }

        public byte SleepCounter {
            get { return (byte) (Status & 0b111); }
        }

        public bool Poisoned {
            get { return (Status & (1 << 3)) != 0; }
        }

        public bool Burned {
            get { return (Status & (1 << 4)) != 0; }
        }

        public bool Frozen {
            get { return (Status & (1 << 5)) != 0; }
        }

        public bool Paralyzed {
            get { return (Status & (1 << 6)) != 0; }
        }

        public bool XAccSetup {
            get { return (Status2 & (1)) != 0; }
        }

        public RbyPokemon(RbySpecies species, byte level) : this(species, level, 0x9888) { }
        public RbyPokemon(RbySpecies species, byte level, DVs dvs) => (Species, Level, DVs) = (species, level, dvs);
        public RbyPokemon(RbySpecies species, ushort hp, byte status, RbyMove[] moves, DVs dvs, byte[] pp, byte level, ushort maxHp, ushort attack, ushort defense, ushort speed, ushort special) => (Species, HP, Status, Moves, DVs, PP, Level, MaxHP, Attack, Defense, Speed, Special) = (species, hp, status, moves, dvs, pp, level, maxHp, attack, defense, speed, special);
        public RbyPokemon(RbySpecies species, ushort hp, byte status, byte status2, RbyMove[] moves, DVs dvs, byte[] pp, byte level, ushort maxHp, ushort attack, ushort defense, ushort speed, ushort special) => (Species, HP, Status, Status2, Moves, DVs, PP, Level, MaxHP, Attack, Defense, Speed, Special) = (species, hp, status, status2, moves, dvs, pp, level, maxHp, attack, defense, speed, special);

        public override string ToString() {
            return string.Format("L{0} {1} DVs {2:X4}", Level, Species.Name, DVs.Value);
        }

        public static implicit operator RbySpecies(RbyPokemon pokemon) { return pokemon.Species; }
    }

    public class RbyEncounterSlot {

        public RbyMap Map;
        public RbyPokemon Pokemon;
        public byte EncounterChance;

        public RbyEncounterSlot(RbyMap map, RbySpecies species, byte level, byte encounterChance) => (Map, Pokemon, EncounterChance) = (map, new RbyPokemon(species, level), encounterChance);

        public override string ToString() {
            return string.Format("{0}/256 {1}", EncounterChance, Pokemon);
        }
    }

    public class RbyHiddenItem : ItemStack {

        public RbyMap Map;
        public byte X;
        public byte Y;
        public ushort Routine;

        public RbyTile Tile {
            get { return Map.Tiles[X, Y]; }
        }

        public RbyHiddenItem(RbyMap map, byte x, byte y, ushort routine, ItemStack item) : base(item.Item, item.Quantity) {
            (Map, X, Y, Routine) = (map, x, y, routine);
        }

        public override string ToString() {
            return string.Format("{0}: {1}", Tile, base.ToString());
        }
    }

    public class RbyEdge : IComparable<RbyEdge> {

        public Action Action;
        public RbyTile NextTile;
        public int NextEdgeset;
        public int Cost;

        public RbyEdge(Action action, RbyTile nextTile, int nextEdgeset, int cost) => (Action, NextTile, NextEdgeset, Cost) = (action, nextTile, nextEdgeset, cost);

        public int CompareTo(RbyEdge other) {
            return Cost - other.Cost;
        }

        public override string ToString() {
            return Enum.GetName(typeof(Action), Action) + " cost: " + Cost;
        }
    }

    public class RbyTile {

        public RbyMap Map;
        public byte X;
        public byte Y;
        public byte TopLeft;
        public byte TopRight;
        public byte BottomLeft;
        public byte BottomRight;
        public bool Solid;
        public bool DownHop;
        public bool LeftHop;
        public bool RightHop;
        public bool CanMoveDown;
        public bool CanMoveUp;
        public bool CanMoveLeft;
        public bool CanMoveRight;
        public Dictionary<int, List<RbyEdge>> Edges;

        public bool InVisionOfTrainer {
            get {
                foreach(RbyTrainer trainer in Map.Trainers) {
                    (int, int) direction = trainer.Direction.ToCardinal();
                    for(int i = 1; i <= trainer.ViewRange; i++) {
                        if(X == trainer.X + i * direction.Item1 && Y == trainer.Y + i * direction.Item2) {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public bool IsUnallowedWarp {
            get {
                RbyWarp warp = Map.Warps[X, Y];
                return warp == null ? false : !warp.Allowed;
            }
        }

        public RbyTile Up {
            get { return Y == 0 ? null : Map.Tiles[X, Y - 1]; }
        }

        public RbyTile Down {
            get { return Y == Map.Height * 2 - 1 ? null : Map.Tiles[X, Y + 1]; }
        }

        public RbyTile Left {
            get { return X == 0 ? null : Map.Tiles[X - 1, Y]; }
        }

        public RbyTile Right {
            get { return X == Map.Width * 2 - 1 ? null : Map.Tiles[X + 1, Y]; }
        }

        public string PokeworldLink {
            get { return "https://gunnermaniac.com/pokeworld?map=" + (Map.Tileset == 0 || Map.Tileset == 21 ? 1 : Map.Id) + "#" + (X + Map.PokeworldX) + "/" + (Y + Map.PokeworldY); }
        }

        internal RbyTile() : this(null, 0, 0, 0, 0, 0, 0, true, false, false, false) { }
        public RbyTile(RbyMap map, byte x, byte y, byte topLeft, byte topRight, byte bottomLeft, byte bottomRight) : this(map, x, y, topLeft, topRight, bottomLeft, bottomRight, false, false, false, false) { }
        public RbyTile(RbyMap map, byte x, byte y, byte topLeft, byte topRight, byte bottomLeft, byte bottomRight, bool solid, bool downHop, bool leftHop, bool rightHop) : this(map, x, y, topLeft, topRight, bottomLeft, bottomRight, solid, downHop, leftHop, rightHop, false, false, false, false) { }
        public RbyTile(RbyMap map, byte x, byte y, byte topLeft, byte topRight, byte bottomLeft, byte bottomRight, bool solid, bool downHop, bool leftHop, bool rightHop, bool canMoveDown, bool canMoveUp, bool canMoveLeft, bool canMoveRight) => (Map, X, Y, TopLeft, TopRight, BottomLeft, BottomRight, Solid, DownHop, LeftHop, RightHop, CanMoveDown, CanMoveUp, CanMoveLeft, CanMoveRight, Edges) = (map, x, y, topLeft, topRight, bottomLeft, bottomRight, solid, downHop, leftHop, rightHop, canMoveDown, canMoveUp, canMoveLeft, canMoveRight, new Dictionary<int, List<RbyEdge>>());

        public void AddEdge(int edgeSet, RbyEdge edge) {
            if(!Edges.ContainsKey(edgeSet)) {
                Edges[edgeSet] = new List<RbyEdge>();
            }
            Edges[edgeSet].Add(edge);
            Edges[edgeSet].Sort();
        }

        public RbyEdge GetEdge(int edgeSet, Action action) {
            if(!Edges.ContainsKey(edgeSet)) {
                return null;
            }

            foreach(RbyEdge edge in Edges[edgeSet]) {
                if(edge.Action == action) {
                    return edge;
                }
            }

            return null;
        }

        public bool ContainsEdge(int edgeSet, Action action) {
            return GetEdge(edgeSet, action) != null;
        }

        public void RemoveEdge(int edgeSet, Action action) {
            RbyEdge edge = GetEdge(edgeSet, action);
            if(edge == null) {
                return;
            }

            Edges[edgeSet].Remove(edge);
            Edges[edgeSet].Sort();
        }

        public override string ToString() {
            return string.Format("{0}#{1},{2}", Map.Id, X, Y);
        }

        public string PrintEdges() {
            string output = "";
            foreach(KeyValuePair<int, List<RbyEdge>> pair in Edges) {
                output += String.Format("Edgeset {0}\n", pair.Key);
                foreach(RbyEdge edge in pair.Value) {
                    output += String.Format("    Edge: {0}\n", edge);
                }
                output += "\n";
            }
            return output;
        }
    }

    public class RbyMap {

        public Rby Game;
        public string Name;
        public byte Id;
        public byte Bank;
        public RbyTileset Tileset;
        public byte Height;
        public byte Width;
        public ushort DataPointer;
        public ushort TextPointer;
        public ushort ScriptPointer;
        public byte ConnectionFlags;
        public RbyConnection[] Connections;
        public ushort ObjectPointer;

        public DataArray<RbyTile> Tiles;

        public byte BorderBlock;
        public DataArray<RbyWarp> Warps;
        public DataArray<RbySign> Signs;
        public DataArray<RbySprite> Sprites;
        public DataArray<RbyTrainer> Trainers;
        public DataArray<RbyItemball> Itemballs;

        public byte EncounterRate;
        public List<RbyEncounterSlot> EncounterSlots;

        public DataArray<RbyHiddenItem> HiddenItems;

        public int PokeworldX;
        public int PokeworldY;

        public RbyConnection EastConnection {
            get { return Connections[0]; }
        }

        public RbyConnection WestConnection {
            get { return Connections[1]; }
        }

        public RbyConnection SouthConnection {
            get { return Connections[2]; }
        }

        public RbyConnection NorthConnection {
            get { return Connections[3]; }
        }

        public RbyMap(Rby game, string name, byte id, byte bank, byte[] data, byte[] wildData, int offset = 0) {
            Game = game;
            Name = name;
            Id = id;
            Bank = bank;
            Tileset = game.Tilesets[data[offset + 0]];
            Height = data[offset + 1];
            Width = data[offset + 2];
            DataPointer = data.ReadWord(offset + 3);
            TextPointer = data.ReadWord(offset + 5);
            ScriptPointer = data.ReadWord(offset + 7);
            ConnectionFlags = data[offset + 9];
            Connections = new RbyConnection[4];
            offset += 10;

            for(int i = 3; i >= 0; i--) {
                if(((ConnectionFlags >> i) & 1) == 1) {
                    Connections[i] = new RbyConnection(this, data, offset);
                    offset += 11;
                }
            }

            ObjectPointer = data.ReadWord(offset);

            EncounterRate = wildData[0];
            EncounterSlots = new List<RbyEncounterSlot>();

            if(EncounterRate > 0) {
                byte[] encounterChances = { 51, 51, 39, 25, 25, 25, 13, 13, 11, 3 };

                int pointer = 1;
                for(int i = 0; i < encounterChances.Length; i++) {
                    byte encounterChance = encounterChances[i];
                    byte level = wildData[pointer++];
                    byte species = wildData[pointer++];
                    EncounterSlots.Add(new RbyEncounterSlot(this, Game.Species[species], level, encounterChance));
                }
            }

            Tiles = new DataArray<RbyTile>(Width * 2, Height * 2);
            Warps = new DataArray<RbyWarp>(Width * 2, Height * 2, new Dictionary<Type, Func<RbyWarp, object, bool>>() {
                { typeof(int), (warp, indexNumber) => { return warp.Index == (int) indexNumber; } },
                { typeof(byte), (warp, indexNumber) => { return warp.Index == (byte) indexNumber; } },
            });
            Signs = new DataArray<RbySign>(Width * 2, Height * 2);
            Sprites = new DataArray<RbySprite>(Width * 2, Height * 2);
            Trainers = new DataArray<RbyTrainer>(Width * 2, Height * 2);
            Itemballs = new DataArray<RbyItemball>(Width * 2, Height * 2);
            HiddenItems = new DataArray<RbyHiddenItem>(Width * 2, Height * 2);
        }

        public RbyTile this[byte x, byte y] {
            get { return Tiles[x, y]; }
            set { Tiles[x, y] = value; }
        }

        public RbyTile this[int x, int y] {
            get { return Tiles[x, y]; }
            set { Tiles[x, y] = value; }
        }

        public static implicit operator byte(RbyMap map) { return map.Id; }
    }

    public class RbyBag : IEnumerable<ItemStack> {

        public Rby Game;
        public ItemStack[] Items;

        public int NumItems {
            get { return Items.Length; }
        }

        public RbyBag(Rby game, ItemStack[] items) => (Game, Items) = (game, items);

        public int IndexOf(string name) {
            return IndexOf(Game.Items[name]);
        }

        public int IndexOf(Item item) {
            for(int i = 0; i < NumItems; i++) {
                if(Items[i].Item == item) {
                    return i;
                }
            }

            return -1;
        }

        public bool Contains(string name) {
            return Contains(Game.Items[name]);
        }

        public bool Contains(Item item) {
            return IndexOf(item) != -1;
        }

        public ItemStack this[int index] {
            get { return Items[index]; }
            set { Items[index] = value; }
        }

        public ItemStack this[Item item] {
            get { return Items[IndexOf(item)]; }
            set { Items[IndexOf(item)] = value; }
        }

        public ItemStack this[string name] {
            get { return Items[IndexOf(Game.Items[name])]; }
            set { Items[IndexOf(Game.Items[name])] = value; }
        }

        public IEnumerator<ItemStack> GetEnumerator() {
            foreach(var item in Items) {
                if(item != null) yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            throw new NotImplementedException();
        }
    }

    internal class RbyData {

        public RbyCharmap Charmap;
        public DataMap<RbySpecies> Species;
        public DataMap<Item> Items;
        public DataMap<RbyMove> Moves;
        public DataMap<RbyTrainerClass> TrainerClasses;
        public DataMap<RbyTileset> Tilesets;
        public DataMap<RbyMap> Maps;

        public RbyData() {
            Charmap = new RbyCharmap();
            Species = new DataMap<RbySpecies>();
            Items = new DataMap<Item>();
            Moves = new DataMap<RbyMove>();
            TrainerClasses = new DataMap<RbyTrainerClass>();
            Tilesets = new DataMap<RbyTileset>();
            Maps = new DataMap<RbyMap>();

            Species.Callbacks = new Dictionary<Type, Func<RbySpecies, object, bool>>() {
                { typeof(string), (species, name) => { return species.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (species, indexNumber) => { return species.IndexNumber == (int) indexNumber; } },
                { typeof(uint), (species, indexNumber) => { return species.IndexNumber == (uint) indexNumber; } },
                { typeof(byte), (species, indexNumber) => { return species.IndexNumber == (byte) indexNumber; } },
            };

            Items.Callbacks = new Dictionary<Type, Func<Item, object, bool>>() {
                { typeof(string), (item, name) => { return item.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (item, id) => { return item.Id == (int) id; } },
                { typeof(uint), (item, id) => { return item.Id == (uint) id; } },
                { typeof(byte), (item, id) => { return item.Id == (byte) id; } },
            };

            Moves.Callbacks = new Dictionary<Type, Func<RbyMove, object, bool>>() {
                { typeof(string), (move, name) => { return move.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (move, id) => { return move.Id == (int) id; } },
                { typeof(uint), (move, id) => { return move.Id == (uint) id; } },
                { typeof(byte), (move, id) => { return move.Id == (byte) id; } },
            };

            TrainerClasses.Callbacks = new Dictionary<Type, Func<RbyTrainerClass, object, bool>>() {
                { typeof(string), (trainerclass, name) => { return trainerclass.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (trainerclass, id) => { return trainerclass.Id == (int) id; } },
                { typeof(uint), (trainerclass, id) => { return trainerclass.Id == (uint) id; } },
                { typeof(byte), (trainerclass, id) => { return trainerclass.Id == (byte) id; } },
            };

            Tilesets.Callbacks = new Dictionary<Type, Func<RbyTileset, object, bool>>() {
                { typeof(int), (tileset, id) => { return tileset.Id == (int) id; } },
                { typeof(uint), (tileset, id) => { return tileset.Id == (uint) id; } },
                { typeof(byte), (tileset, id) => { return tileset.Id == (byte) id; } },
            };

            Maps.Callbacks = new Dictionary<Type, Func<RbyMap, object, bool>>() {
                { typeof(string), (map, name) => { return map.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (map, id) => { return map.Id == (int) id; } },
                { typeof(uint), (map, id) => { return map.Id == (uint) id; } },
                { typeof(byte), (map, id) => { return map.Id == (byte) id; } },
            };
        }
    }

    public class Rby : GameBoy {

        private static RbyData Data;

        public RbyCharmap Charmap {
            get { return Data.Charmap; }
        }

        public DataMap<RbySpecies> Species {
            get { return Data.Species; }
        }

        public DataMap<Item> Items {
            get { return Data.Items; }
        }

        public DataMap<RbyMove> Moves {
            get { return Data.Moves; }
        }

        public DataMap<RbyTrainerClass> TrainerClasses {
            get { return Data.TrainerClasses; }
        }

        public DataMap<RbyTileset> Tilesets {
            get { return Data.Tilesets; }
        }

        public DataMap<RbyMap> Maps {
            get { return Data.Maps; }
        }

        public Rby(string rom, string saveName, SpeedupFlags speedupFlags = SpeedupFlags.None) : base(rom, saveName, "roms/gbc_bios.bin", speedupFlags) {
            if(Data == null) {
                Data = new RbyData();
                LoadSpecies();
                LoadItems();
                LoadMoves();
                LoadTrainerClasses();
                LoadTilesets();
                LoadMaps();
                LoadMapTiles();
                LoadMapObjects();
                FindPokeworldOffsets(Maps[0], 50, 234, new HashSet<RbyMap>());
                LoadHiddenItems();
            }
        }

        public override byte ReadHRA() {
            return CpuRead(0xFFD3);
        }

        public override byte ReadHRS() {
            return CpuRead(0xFFD4);
        }

        public byte CalcDSUM() {
            return (byte)((ReadHRA() + ReadHRS()) % 256);
        }

        public string getIGT() {
            return $"Second: {CpuRead("wPlayTimeSeconds")}, Frame: {CpuRead("wPlayTimeFrames")}";
        }

        public void setIGT(byte hours, byte minutes, byte seconds, byte frames) {
            CpuWrite("wPlayTimeHours", hours);
            CpuWrite("wPlayTimeMinutes", minutes);
            CpuWrite("wPlayTimeSeconds", seconds);
            CpuWrite("wPlayTimeFrames", frames);
        }

        public override void Press(params Joypad[] joypads) {
            foreach(Joypad joypad in joypads) {
                do {
                    Step();
                    AdvanceToAddress(Sym["_Joypad"]);
                } while((CpuRead(Sym["wd730"]) & (1 << 5)) != 0);
                Inject(joypad);
                AdvanceFrame();
            }
        }

        public CpuAddress WalkTo(int x, int y) {
            return Execute(AStar.FindPath(GetCurTile(), GetCurMap().Tiles[x, y], CpuRead(Sym["wWalkBikeSurfState"]) == 0x00 ? 17 : 8));
        }

        public override CpuAddress Execute(Action[] actions, params (byte, byte, byte)[] itemPickups) {
            CpuAddress ret = new CpuAddress(0);

            foreach(Action action in actions) {
                if(action.IsDpad()) {
                    Joypad joypad = (Joypad) ((byte) action << 4);
                    do {
                        AdvanceToAddress(Sym["JoypadOverworld"]);
                        Inject(joypad);
                        ret = AdvanceWithJoypadToAddress(joypad, Sym["CollisionCheckOnLand.collision"], Sym["CollisionCheckOnWater.collision"], Sym["TryDoWildEncounter.CanEncounter"].Cpu() + 6, Sym["OverworldLoopLessDelay.newBattle"].Cpu() + 3);
                        if(ret == Sym["TryDoWildEncounter.CanEncounter"].Cpu() + 6) {
                            return AdvanceToAddress(Sym["CalcStats"]);
                        } else if(ret == Sym["CollisionCheckOnWater.collision"]) {
                            return ret;
                        }
                        ret = Sym["JoypadOverworld"];
                        AdvanceToAddress(Sym["JoypadOverworld"], Sym["EnterMap"].Cpu() + 0x10u);
                    } while((CpuRead(Sym["wd736"]) & 64) != 0);
                    if(Array.Exists(itemPickups, element => element.Item1 == CpuRead(Sym["wCurMap"]) && element.Item2 == CpuRead(Sym["wXCoord"]) && element.Item3 == CpuRead(Sym["wYCoord"]))) {
                        PickupItem();
                    }
                } else if(action == Action.A) {
                    Inject(Joypad.A);
                    AdvanceFrame(Joypad.A);
                    ret = AdvanceWithJoypadToAddress(Joypad.A, Sym["JoypadOverworld"], Sym["PrintLetterDelay"]);
                    if(ret == Sym["PrintLetterDelay"]) {
                        return ret;
                    }
                } else if(action == Action.StartB) {
                    Press(Joypad.Start);
                    Press(Joypad.B);
                    ret = AdvanceToAddress(Sym["JoypadOverworld"]);
                } else if(action == Action.PokedexFlash) {
                    Press(Joypad.Start);
                    Press(Joypad.A);
                    Press(Joypad.B);
                    Press(Joypad.Start);
                    ret = AdvanceToAddress(Sym["JoypadOverworld"]);
                } else if(action == Action.Delay) {
                    Inject(Joypad.None);
                    AdvanceToAddress(Sym["OverworldLoop"]);
                    ret = AdvanceToAddress(Sym["JoypadOverworld"]);
                } else {
                    Debug.Assert(false, "Unknown Action: {0}", action);
                }
            }

            return ret;
        }

        public void PickupItem() {
            Inject(Joypad.A);
            AdvanceWithJoypadToAddress(Joypad.A, Sym["TextCommand0B"]);
        }

        public RbyMap GetCurMap() {
            return Maps[CpuRead(Sym["wCurMap"])];
        }

        public RbyTile GetCurTile() {
            RbyMap map = GetCurMap();
            return map == null ? null : map.Tiles[CpuRead(Sym["wXCoord"]), CpuRead(Sym["wYCoord"])];
        }

        private RbyPokemon CreatePartyStruct(CpuAddress address) {
            RbySpecies species = Species[CpuRead(address)];
            ushort hp = CpuReadWord(address + 1);
            byte status = CpuRead(address + 4);
            RbyMove[] moves = new RbyMove[4];
            DVs dvs = new DVs(CpuReadWord(address + 27));
            byte[] pp = new byte[4];
            byte level = CpuRead(address + 33);
            ushort maxHp = CpuReadWord(address + 34);
            ushort attack = CpuReadWord(address + 36);
            ushort defense = CpuReadWord(address + 38);
            ushort speed = CpuReadWord(address + 40);
            ushort special = CpuReadWord(address + 42);
            ReadMovesAndPP(address + 8, address + 29, ref moves, ref pp);
            return new RbyPokemon(species, hp, status, moves, dvs, pp, level, maxHp, attack, defense, speed, special);
        }

        private RbyPokemon CreateEnemyMon(CpuAddress address, bool enemyMon = false) {
            RbySpecies species = Species[CpuRead(address)];
            ushort hp = CpuReadWord(address + 1);
            byte status = CpuRead(address + 4);
            byte status2 = enemyMon ? CpuRead("wEnemyBattleStatus2") : CpuRead("wPlayerBattleStatus2");
            RbyMove[] moves = new RbyMove[4];
            DVs dvs = new DVs(CpuReadWord(address + 12));
            byte[] pp = new byte[4];
            byte level = CpuRead(address + 14);
            ushort maxHp = CpuReadWord(address + 15);
            ushort attack = CpuReadWord(address + 17);
            ushort defense = CpuReadWord(address + 19);
            ushort speed = CpuReadWord(address + 21);
            ushort special = CpuReadWord(address + 23);
            ReadMovesAndPP(address + 8, address + 25, ref moves, ref pp);
            return new RbyPokemon(species, hp, status, status2, moves, dvs, pp, level, maxHp, attack, defense, speed, special);
        }

        private void ReadMovesAndPP(CpuAddress moveAddress, CpuAddress ppAddress, ref RbyMove[] moves, ref byte[] pp) {
            for(byte i = 0; i < 4; i++) {
                moves[i] = Moves[CpuRead(moveAddress + i)];
                pp[i] = CpuRead(ppAddress + i);
            }
        }

        public RbyPokemon GetPartyMon(int index) {
            return CreatePartyStruct(Sym["wPartyMons"].Cpu() + (uint) (0x2C * index));
        }

        public RbyPokemon GetBattleMon() {
            return CreateEnemyMon(Sym["wBattleMon"]);
        }

        public RbyPokemon[] GetEnemyParty() {
            RbyPokemon[] party = new RbyPokemon[CpuRead("wEnemyPartyCount")];
            for(int i = 0; i < party.Length; i++) {
                party[i] = GetEnemyMon(i);
            }

            return party;
        }

        public RbyPokemon GetEnemyMon(int index) {
            return CreatePartyStruct(Sym["wEnemyMons"].Cpu() + (uint) (0x2C * index));
        }

        public RbyPokemon GetEnemyMon() {
            return CreateEnemyMon(Sym["wEnemyMon"], true);
        }

        public RbyBag GetBag() {
            byte numItems = CpuRead(Sym["wNumBagItems"]);
            ItemStack[] items = new ItemStack[numItems];
            for(byte i = 0; i < numItems; i++) {
                items[i] = new ItemStack(Items[CpuRead(Sym["wBagItems"].Cpu() + i * 2u + 0u)],
                                               CpuRead(Sym["wBagItems"].Cpu() + i * 2u + 1u));
            }

            return new RbyBag(this, items);
        }

        public void OpenItemBag() {
            if(CpuRead("wCurrentMenuItem") == 0) Press(Joypad.Down);
            Press(Joypad.A);
        }

        public void OpenFightMenu() {
            if(CpuRead("wCurrentMenuItem") == 1) Press(Joypad.Up);
            Press(Joypad.A);
            AdvanceToAddress("_Joypad");
        }

        public void BagScroll(string item) {
            BagScroll(GetBag().IndexOf(item));
        }

        public void BagScroll(Item item) {
            BagScroll(GetBag().IndexOf(item));
        }

        public void BagScroll(int targetSlot) {
            OpenItemBag();
            AdvanceToAddress("DisplayListMenuID");
            int currentSlot = CpuRead("wCurrentMenuItem") + CpuRead("wListScrollOffset");
            int difference = targetSlot - currentSlot;
            int numSlots = Math.Abs(difference);

            if(difference == 0) {
                Press(Joypad.None);
            } else if(difference == -2) {
                Press(Joypad.Up, Joypad.Up | Joypad.Left);
            } else if(difference == 2) {
                Press(Joypad.Down, Joypad.Down | Joypad.Left);
            } else {
                Joypad direction = difference < 0 ? Joypad.Up : Joypad.Down;
                for(int i = 0; i < numSlots; i++) {
                    Press(direction);
                    if(i != numSlots - 1) Press(Joypad.None);
                }
            }
        }

        public void UseItem(string item) {
            BagScroll(item);
            Press(Joypad.A);
        }

        public void UseXItem(string item) {
            UseItem(item);
            AdvanceToAddress(Sym["Char57"]);
            Inject(Joypad.B);
            AdvanceFrame(Joypad.B);
        }

        public void UseXAttack() {
            UseXItem("X ATTACK");
        }

        public void UseXDefense() {
            UseXItem("X DEFENSE");
        }

        public void UseXSpeed() {
            UseXItem("X SPEED");
        }

        public void UseXSpecial() {
            UseXItem("X SPECIAL");
        }

        public void UseXAccuracy() {
            UseXItem("X ACCURACY");
        }

        public void UseHealingItem(string item) {
            UseItem(item);
            Press(Joypad.None, Joypad.A, Joypad.B);
        }

        public void UseFullRestore() {
            UseHealingItem("FULL RESTORE");
        }

        public void UsePokeFlute() {
            BagScroll("POKE FLUTE");
            Press(Joypad.A);
        }

        public void UseMove(int slot, int numMoves = 4) {
            OpenFightMenu();
            int currentSlot = CpuRead("wCurrentMenuItem") - 1;
            int difference = currentSlot - slot;
            int numSlots = difference == 0 ? 0 : slot % 2 == currentSlot % 2 ? (int)(numMoves/2) : 1;
            Joypad joypad = (MathHelper.FloorMod(difference, numMoves) & 2) != 0 ? Joypad.Down : Joypad.Up;
            if(numSlots == 0) {
                Press(Joypad.None);
            } else if(numSlots == 1) {
                Press(joypad);
            } else if(numSlots == 2) {
                Press(joypad, Joypad.None, joypad);
            }
            Press(Joypad.A);
        }

        public void UseSlot1(int numMoves = 4) {
            UseMove(0, numMoves);
        }

        public void UseSlot2(int numMoves = 4) {
            UseMove(1, numMoves);
        }

        public void UseSlot3(int numMoves = 4) {
            UseMove(2, numMoves);
        }

        public void UseSlot4(int numMoves = 4) {
            UseMove(3, numMoves);
        }

        private void LoadSpecies() {
            int maxIndexNumber = 190;
            int nameLength = 10;

            RomAddress namesStart = Sym["MonsterNames"];
            RomAddress baseStatsStart = Sym["MonBaseStats"];
            int baseStatsSize = Sym["MonBaseStatsEnd"] - baseStatsStart;
            int numBaseStats = (Sym["CryData"] - baseStatsStart) / baseStatsSize;
            byte[] pokedex = Rom.Subarray(Sym["PokedexOrder"], maxIndexNumber);

            for(int i = 0; i < numBaseStats; i++) {
                byte[] header = Rom.Subarray(baseStatsStart + i * baseStatsSize, baseStatsSize);
                byte indexNumber = (byte) Array.IndexOf(pokedex, header[0]);
                string name = Charmap.Decode(Rom.Subarray(namesStart + indexNumber * nameLength, nameLength));
                Species.Add(new RbySpecies(this, name, ++indexNumber, header));
            }

            Species.Add(new RbySpecies(this, "'M", 0, new byte[baseStatsSize]));
            for(int i = 1; i <= maxIndexNumber; i++) {
                if(pokedex[i - 1] == 0) Species.Add(new RbySpecies(this, string.Format("MISSINGNO_{0:X2}", i), (byte) i, new byte[baseStatsSize]));
            }
        }

        private void LoadItems() {
            int numItems = 97;

            int namePointer = Sym["ItemNames"].Rom();
            for(int i = 0; i < numItems; i++) {
                Items.Add(new Item((byte) (i + 1), Charmap.Decode(Rom.ReadUntil(RbyCharmap.Terminator, ref namePointer))));
            }

            for(int i = 0; i < 256; i++) {
                if(i >= 0xC4 && i <= 0xC8) {
                    Items.Add(new Item((byte) i, string.Format("HM{0}", (i + 1 - 0xC4).ToString("D2"))));
                } else if(i >= 0xC9 && i <= 0xFF) {
                    Items.Add(new Item((byte) i, string.Format("TM{0}", (i + 1 - 0xC9).ToString("D2"))));
                } else if(Items[i] == null) {
                    Items.Add(new Item((byte) i, string.Format("hex{0:2X}", i)));
                }
            }
        }

        private void LoadMoves() {
            RomAddress movesStart = Sym["Moves"];
            int moveSize = Sym["MoveEnd"] - movesStart;
            int numMoves = (Sym["BaseStats"] - movesStart) / moveSize;

            int namePointer = Sym["MoveNames"].Rom();
            for(int i = 0; i < numMoves; i++) {
                string name = Charmap.Decode(Rom.ReadUntil(RbyCharmap.Terminator, ref namePointer));
                byte[] header = Rom.Subarray(movesStart + i * moveSize, moveSize);
                Moves.Add(new RbyMove(this, name, header));
            }
        }

        private void LoadTrainerClasses() {
            int numTrainerClases = 47;

            int namePointer = Sym["TrainerNames"].Rom();
            for(int i = 0; i < numTrainerClases; i++) {
                byte id = (byte) (i + 201);
                string name = Charmap.Decode(Rom.ReadUntil(RbyCharmap.Terminator, ref namePointer));

                RomAddress teamsPointer = GetTeamPointer(i);
                int length = i == TrainerClasses.Count - 1 ? 12 : GetTeamPointer(i + 1) - teamsPointer; // lance hardcoded to 12 bytes

                if(length == 0) continue;

                List<List<RbyPokemon>> teams = new List<List<RbyPokemon>>();
                int pointer = teamsPointer;
                while(pointer - teamsPointer < length) {
                    List<RbyPokemon> team = new List<RbyPokemon>();
                    byte format = Rom[pointer++];
                    byte level = format;
                    byte speciesIndex;

                    while((speciesIndex = Rom[pointer++]) != 0x00) {
                        if(format == 0xFF) {
                            level = speciesIndex;
                            speciesIndex = Rom[pointer++];
                        }
                        team.Add(new RbyPokemon(Species[speciesIndex], level));
                    }

                    teams.Add(team);
                }

                TrainerClasses.Add(new RbyTrainerClass(id, name, teams));
            }
        }

        private RomAddress GetTeamPointer(int index) {
            return new RomAddress(Sym["TrainerDataPointers"].Bank, Rom.ReadWord(Sym["TrainerDataPointers"].Rom() + index * 2));
        }

        private void LoadTilesets() {
            Dictionary<byte, List<(byte, byte)>> tilePairCollisionsLand = new Dictionary<byte, List<(byte, byte)>>();
            int pointer = Sym["TilePairCollisionsLand"].Rom();

            byte tileset;
            while((tileset = Rom[pointer++]) != 0xFF) {
                if(!tilePairCollisionsLand.ContainsKey(tileset)) tilePairCollisionsLand[tileset] = new List<(byte, byte)>();
                tilePairCollisionsLand[tileset].Add((Rom[pointer++], Rom[pointer++]));
            }

            int numTilesets = GetType() == typeof(Yellow) ? 25 : 24;
            pointer = Sym["Tilesets"].Rom();

            for(int i = 0; i < numTilesets; i++) {
                byte bank = Rom[pointer++];
                ushort blockPointer = Rom.ReadWord(ref pointer);
                ushort gfxPointer = Rom.ReadWord(ref pointer);
                ushort collPointer = Rom.ReadWord(ref pointer);
                byte[] counterTiles = Rom.Subarray(ref pointer, 3);
                byte grassTile = Rom[pointer++];
                RbyPermission permission = (RbyPermission) Rom[pointer++];

                byte[] collisionData = new byte[0];
                int collisionDataPointer = new RomAddress((byte) (GetType() == typeof(Yellow) ? 0x01 : 0x00), collPointer);
                byte tile;
                while((tile = Rom[collisionDataPointer++]) != 0xFF) {
                    collisionData = collisionData.Add(tile);
                }

                Tilesets.Add(new RbyTileset(this, (byte) i, bank, blockPointer, gfxPointer, collPointer, counterTiles, grassTile, permission, collisionData, tilePairCollisionsLand.GetValueOrDefault((byte) i, new List<(byte, byte)>())));
            }
        }

        private void LoadMaps() {
            int numMaps = GetType() == typeof(Yellow) ? 249 : 248;

            for(int i = 0; i < numMaps; i++) {
                byte headerBank = Rom[Sym["MapHeaderBanks"].Rom() + i];
                ushort headerPointer = Rom.ReadWord(Sym["MapHeaderPointers"].Rom() + i * 2);
                string name = Sym.GetAddressName(new CpuAddress((uint) (headerBank << 16) | headerPointer));
                if(name != null && name.Contains("_h")) {
                    name = name.Substring(0, name.IndexOf("_h"));

                    ushort wildDataPointer = Rom.ReadWord(Sym["WildDataPointers"].Rom() + i * 2);
                    byte[] wildData = Rom.Subarray(new RomAddress(Sym["WildDataPointers"].Bank, wildDataPointer), Sym["Route2Mons"].Rom() - Sym["Route1Mons"].Rom());
                    Maps.Add(new RbyMap(this, name, (byte) i, headerBank, Ram[headerBank], wildData, headerPointer));
                }
            }
        }

        private void LoadMapObjects() {
            foreach(RbyMap map in Maps) {
                int pointer = new RomAddress(map.Bank, map.ObjectPointer);

                map.BorderBlock = Rom[pointer++];

                byte numWarps = Rom[pointer++];
                for(byte index = 0; index < numWarps; index++) {
                    byte y = Rom[pointer++];
                    byte x = Rom[pointer++];
                    byte destinationIndex = Rom[pointer++];
                    byte destinationMap = Rom[pointer++];

                    if((destinationMap == 0xFF || destinationMap == 0xED) && GetType() != typeof(Yellow)) {
                        destinationMap = RedBlue.wLastMapDestinations[(map.Name, index)];
                        if(map.Name == "SilphCo11F") {
                            destinationIndex = 0;
                        }
                    }

                    map.Warps[x, y] = new RbyWarp(map, x, y, index, Maps[destinationMap], destinationIndex);
                }

                byte numSigns = Rom[pointer++];
                for(byte index = 0; index < numSigns; index++) {
                    byte y = Rom[pointer++];
                    byte x = Rom[pointer++];
                    byte scriptId = Rom[pointer++];

                    ushort scriptAddr = Rom.ReadWord(new RomAddress(map.Bank, (ushort) (map.TextPointer + (scriptId - 1) * 2)));
                    RomAddress addr = new RomAddress(map.Bank, scriptAddr);
                    if(Sym.GetAddressName(addr) == null) addr = new RomAddress(0, scriptAddr);

                    if(Rom[addr] != 0x17) continue;

                    uint textAddress = new RomAddress(Rom[addr + 3], Rom.ReadWord(addr + 1));

                    bool doneReading = false;
                    string text = "";
                    do {
                        byte character = Rom[textAddress++];

                        switch(character) {
                            case 0x51:
                            case 0x55:
                            case 0x4F:
                            case 0x4E:
                                text += "\n";
                                break;
                            case 0x57:
                            case 0x58:
                                doneReading = true;
                                break;
                            default:
                                text += Charmap.Decode(character);
                                break;
                        }
                    } while(!doneReading);

                    map.Signs[x, y] = new RbySign(map, x, y, scriptId, text);
                }

                byte numSprites = Rom[pointer++];
                for(byte index = 0; index < numSprites; index++) {
                    byte spriteId = Rom[pointer++];
                    byte y = (byte) (Rom[pointer++] - 4);
                    byte x = (byte) (Rom[pointer++] - 4);
                    RbySpriteMovement movement = (RbySpriteMovement) Rom[pointer++];
                    byte rangeOrDirection = Rom[pointer++];
                    byte range = 0;
                    Direction direction = Direction.None;
                    byte textId = Rom[pointer++];
                    bool isTrainer = (textId & (1 << 6)) != 0;
                    bool isItemball = (textId & (1 << 7)) != 0;
                    textId &= 0xFF ^ (1 << 6 | 1 << 7);

                    if(movement == RbySpriteMovement.Walk) {
                        range = rangeOrDirection;
                    } else {
                        direction = DirectionFunctions.RbySpriteDirection(rangeOrDirection);
                        if(direction == Direction.None) {
                            movement = RbySpriteMovement.Turn;
                        }
                    }

                    RbySprite baseSprite = new RbySprite(map, (byte) (index + 1), spriteId, x, y, movement, direction, range);
                    map.Sprites[x, y] = baseSprite;

                    if(isTrainer) {
                        byte trainerClassId = Rom[pointer++];
                        byte trainerId = Rom[pointer++];
                        if(trainerClassId < 200) continue; // Stationary encounters

                        RbyTrainerClass trainerClass = TrainerClasses[trainerClassId];

                        int textPointer = new RomAddress(map.Bank, map.TextPointer) + (textId - 1) * 2;
                        int scriptPointer = new RomAddress(map.Bank, Rom.ReadWord(textPointer));
                        int headerPointer = new RomAddress(map.Bank, Rom.ReadWord(scriptPointer + 2));

                        byte eventFlagBit = Rom[headerPointer++];
                        byte viewRange = (byte) (Rom[headerPointer++] >> 4);
                        ushort eventFlagAddress = Rom.ReadWord(ref headerPointer);

                        map.Trainers[x, y] = new RbyTrainer(baseSprite, trainerClass, trainerId, eventFlagBit, viewRange, eventFlagAddress);
                    } else if(isItemball) {
                        map.Itemballs[x, y] = new RbyItemball(baseSprite, Items[Rom[pointer++]]);
                    }
                }
            }
        }

        private void LoadMapTiles() {
            foreach(RbyMap map in Maps) {
                RbyTileset tileset = map.Tileset;
                byte width = map.Width;
                byte height = map.Height;
                byte[] blocks = Rom.Subarray(new RomAddress(map.Bank, map.DataPointer), width * height);

                int length = blocks.Length - blocks.Length % width;
                byte[] tiles = new byte[length * 4 * 4];
                for(int i = 0; i < length; i++) {
                    byte[] tiles2 = Rom.Subarray(new RomAddress(tileset.Bank, tileset.BlockPointer) + blocks[i] * 16, 16);
                    for(int j = 0; j < 4; j++) {
                        Array.Copy(tiles2, j * 4, tiles, (i / width) * (width * 4) * 4 + j * (width * 4) + (i % width) * 4, 4);
                    }
                }

                byte mapWidth = (byte) (width * 2);
                byte mapHeight = (byte) (height * 2);
                for(int y = 0; y < height * 4; y += 2) {
                    for(int x = 0; x < width * 4; x += 2) {
                        byte tl = tiles[(x) + (y) * width * 4];
                        byte tr = tiles[(x + 1) + (y) * width * 4];
                        byte bl = tiles[(x) + (y + 1) * width * 4];
                        byte br = tiles[(x + 1) + (y + 1) * width * 4];

                        byte xTile = (byte) (x / 2);
                        byte yTile = (byte) (y / 2);

                        RbyTile tile = new RbyTile(map, xTile, yTile, tl, tr, bl, br);

                        if(Array.IndexOf(tileset.CollisionData, bl) == -1) {
                            tile.Solid = true;
                        }

                        if(tileset.Id == 0 && tile.Solid) {
                            if(br == 0x37) tile.DownHop = true;
                            else if(tl == 0x27 && bl == 0x24) tile.LeftHop = true;
                            else if(tr == 0x24 && br == 0x24) tile.RightHop = true;
                        }

                        map.Tiles[xTile, yTile] = tile;
                    }
                }

                for(byte y = 0; y < mapHeight; y++) {
                    for(byte x = 0; x < mapWidth; x++) {
                        RbyTile down = y == mapHeight - 1 ? new RbyTile() : map.Tiles[x, y + 1];
                        RbyTile up = y == 0 ? new RbyTile() : map.Tiles[x, y - 1];
                        RbyTile left = x == 0 ? new RbyTile() : map.Tiles[x - 1, y];
                        RbyTile right = x == mapWidth - 1 ? new RbyTile() : map.Tiles[x + 1, y];
                        RbyTile tile = map.Tiles[x, y];

                        tile.CanMoveDown = !(tileset.TilePairCollisions.Contains((down.BottomLeft, tile.BottomLeft)) || tileset.TilePairCollisions.Contains((tile.BottomLeft, down.BottomLeft))) && (!down.Solid || down.DownHop);
                        tile.CanMoveUp = !(tileset.TilePairCollisions.Contains((up.BottomLeft, tile.BottomLeft)) || tileset.TilePairCollisions.Contains((tile.BottomLeft, up.BottomLeft))) && (!up.Solid);
                        tile.CanMoveLeft = !(tileset.TilePairCollisions.Contains((left.BottomLeft, tile.BottomLeft)) || tileset.TilePairCollisions.Contains((tile.BottomLeft, left.BottomLeft))) && (!left.Solid || left.LeftHop);
                        tile.CanMoveRight = !(tileset.TilePairCollisions.Contains((right.BottomLeft, tile.BottomLeft)) || tileset.TilePairCollisions.Contains((tile.BottomLeft, right.BottomLeft))) && (!right.Solid || right.RightHop);
                    }
                }
            }
        }

        private void FindPokeworldOffsets(RbyMap map, int x, int y, HashSet<RbyMap> seenMaps) {
            if(!seenMaps.Add(map)) {
                return;
            }
            map.PokeworldX = x;
            map.PokeworldY = y;
            for(int i = 0; i < 4; i++) {
                RbyConnection connection = map.Connections[i];
                if(connection != null) {
                    switch(i) {
                        case 0:
                            FindPokeworldOffsets(connection.DestinationMap, x + (map.Width * 2), y + (connection.YAlignment > 128 ? 256 - connection.YAlignment : connection.YAlignment * -1), seenMaps);
                            break;
                        case 1:
                            FindPokeworldOffsets(connection.DestinationMap, x - (connection.XAlignment + 1), y + (connection.YAlignment > 128 ? 256 - connection.YAlignment : connection.YAlignment * -1), seenMaps);
                            break;
                        case 2:
                            FindPokeworldOffsets(connection.DestinationMap, x + (connection.XAlignment > 128 ? 256 - connection.XAlignment : connection.XAlignment * -1), y + (map.Height * 2), seenMaps);
                            break;
                        case 3:
                            FindPokeworldOffsets(connection.DestinationMap, x + (connection.XAlignment > 128 ? 256 - connection.XAlignment : connection.XAlignment * -1), y - (connection.YAlignment + 1), seenMaps);
                            break;
                    }
                }
            }
        }

        private void LoadHiddenItems() {
            bool isYellow = GetType() == typeof(Yellow);

            for(int i = 0; i < 85; i++) {
                RbyMap map = Maps[Rom[Sym["HiddenObjectMaps"].Rom() + (isYellow ? i * 3 : i)]];
                int pointer = new RomAddress(Sym["HiddenObjectMaps"].Bank, Rom.ReadWord(isYellow ? Sym["HiddenObjectMaps"].Rom() + i * 3 + 1 : Sym["HiddenObjectPointers"].Rom() + i * 2));

                if(map == null) {
                    continue;
                }

                byte y;
                byte x;
                byte id;
                ushort routine;
                while((y = Rom[pointer++]) != 0xFF) {
                    x = Rom[pointer++];
                    id = Rom[pointer++];
                    pointer++;
                    routine = Rom.ReadWord(ref pointer);

                    ItemStack itemstack = null;
                    if(routine == Sym["HiddenItems"].Rom().Address) {
                        itemstack = new ItemStack(Items[id]);
                    } else if(routine == Sym["HiddenCoins"].Rom().Address) {
                        byte quantity = (byte) (id - Items["Coin"].Id);
                        if(quantity == 40) quantity = 20; // ?????
                        itemstack = new ItemStack(Items["Coin"], quantity);
                    }

                    if(itemstack != null && x > 0 && y > 0 && x < map.Width * 2 - 1 && y < map.Height * 2 - 1) {
                        map.HiddenItems[x, y] = new RbyHiddenItem(map, x, y, routine, itemstack);
                    }
                }
            }
        }
    }
}