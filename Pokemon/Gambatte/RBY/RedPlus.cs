using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;

// cff1 = enemymondvs, 2 bytes
namespace Pokemon {

    public class RedPlusCharmap : Charmap {

        public RedPlusCharmap() : base(0x80, "A B C D E F G H I J K L M N O P " +
                                        "Q R S T U V W X Y Z ( ) : ; [ ] " +
                                        "a b c d e f g h i j k l m n o p " +
                                        "q r s t u v w x y z E 'd 'l 's 't 'v " +
                                        "_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ " +
                                        "_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ " +
                                        "' _ _ - _ _ ? ! . _ _ _ _ _ _ M " +
                                        "_ * . / , F 0 1 2 3 4 5 6 7 8 9 ") { }
    }

    public enum RedPlusType : byte {
        Normal,
        Fighting,
        Flying,
        Poison,
        Ground,
        Rock,
        Bird,
        Bug,
        Ghost,
        Steel,
        Fire = 20,
        Water,
        Grass,
        Electric,
        Psyshic,
        Ice,
        Dragon,
        Dark,
        Fairy
    }

    public class RedPlusSpecies {
        public RedPlus Game;
        public string Name;
        public byte IndexNumber;
        public byte PokedexNumber;
        public byte BaseHP;
        public byte BaseAttack;
        public byte BaseDefense;
        public byte BaseSpeed;
        public byte BaseSpecial;
        public RedPlusType Type1;
        public RedPlusType Type2;
        public byte CatchRate;
        public byte BaseExpYield;
        public RomAddress FrontSpritePointer;
        public RomAddress BackSpritePointer;

        public RedPlusSpecies(RedPlus game, string name, byte indexNumber, byte[] data, int offset = 0) {
            Game = game;
            Name = name;
            IndexNumber = indexNumber;
            PokedexNumber = data[offset + 0];
            BaseHP = data[offset + 1];
            BaseAttack = data[offset + 2];
            BaseDefense = data[offset + 3];
            BaseSpeed = data[offset + 4];
	        BaseSpecial = data[offset + 5];
            Type1 = (RedPlusType) data[offset + 6];
            Type2 = (RedPlusType) data[offset + 7];
            CatchRate = data[offset + 8];
            BaseExpYield = data[offset + 9];
            ushort frontSpriteAddress = data.ReadWord(offset + 11);
            ushort backSpriteAddress = data.ReadWord(offset + 13);

            byte spriteBank = 0xD;
//             if(indexNumber == 0x15 && game.GetType() != typeof(Yellow)) {
//                 spriteBank = 1;
//             } else if(indexNumber == 0xB6) {
//                 spriteBank = 0xB;
//             } else if(indexNumber < 0x1F) {
//                 spriteBank = 0xB;
//             } else if(indexNumber < 0x4A) {
//                 spriteBank = 0xB;
//             } else if(indexNumber < 0x74 || indexNumber == 0x74 && frontSpriteAddress > 0x7000) {
//                 spriteBank = 0xB;
//             } else if(indexNumber < 0x99 || indexNumber == 0x99 && frontSpriteAddress > 0x7000) {
//                 spriteBank = 0xC;
//             }

            FrontSpritePointer = new RomAddress(spriteBank, frontSpriteAddress);
            BackSpritePointer = new RomAddress(spriteBank, backSpriteAddress);
        }

        public static implicit operator byte(RedPlusSpecies species) { return species.IndexNumber; }
    }


    public enum RedPlusMoveEffect : byte {

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
        DefenseUP1,
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
        UnusedEffect1e,
        FlinchSide,
        Slee,
        PoisonSide2,
        BurnSide2,
        FreezeSide2,
        ParalyzeSide2,
        FlinchSide2,
        OHKO,
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
        AccuracyDownSide,
        EvasionDownSide,
        UnusedEffect4a,
        UnusedEffect4b,
        ConfusionSide,
        Twinneedle,
        Nuzzle,
        Substitute,
        HyperBeam,
        Rage,
        Mimic,
        Metronome,
        LeechSeed,
        Splash,
        Disable,
        FireFang,
        IceFang,
        ThunderFang,
        VoltTackle,
        PoisonFang,
        Growth,
        HoneClaws,
        DynamicPunch,
        SilverWind,
        AttackUp1Side,
        AttackUp1Side2,
        DefenseUp1Side,
        TriAttack,
    }

    public class RedPlusMove {

        public RedPlus Game;
        public string Name;
        public byte Id;
        public byte Animation;
        public byte Power;
        public RedPlusType Type;
        public byte Accuracy;
        public byte PP;

        public RedPlusMove(RedPlus game, string name, byte[] data, int offset = 0) {
            Game = game;
            Name = name;
            Id = data[offset + 0];
            Animation = data[offset + 0];
            Power = data[offset + 2];
            Type = (RedPlusType) data[offset + 3];
            Accuracy = data[offset + 4];
            PP = data[offset + 5];
        }

        public static implicit operator byte(RedPlusMove move) { return move.Id; }
    }

    public enum RedPlusPermission : byte {
        Indoor,
        Cave,
        Outdoor
    }

    public class RedPlusTileset {

        public RedPlus Game;
        public byte Id;
        public byte Bank;
        public ushort BlockPointer;
        public ushort GfxPointer;
        public ushort CollisionPointer;
        public byte[] CounterTiles;
        public byte GrassTile;
        public RedPlusPermission Permission;
        public byte[] CollisionData;
        public List<(byte, byte)> TilePairCollisions;
        public RedPlusTileset(RedPlus game, byte id, byte bank, ushort blockPointer, ushort gfxPointer, ushort collisionPointer, byte[] counterTiles, byte grassTile, RedPlusPermission permission, byte[] collisionData, List<(byte, byte)> tilePairCollisions) => (Game, Id, Bank, BlockPointer, GfxPointer, CollisionPointer, CounterTiles, GrassTile, Permission,  CollisionData, TilePairCollisions) = (game, id, bank, blockPointer, gfxPointer, collisionPointer, counterTiles, grassTile, permission, collisionData, tilePairCollisions);
        public static implicit operator byte(RedPlusTileset tileset) { return tileset.Id; }
    }

    public class RedPlusConnection {

        public RedPlusMap Map;
        public byte MapId;
        public ushort Source;
        public ushort Destination;
        public byte Length;
        public byte MapWidth;
        public byte YAlignment;
        public byte XAlignment;
        public ushort Window;

        public RedPlusMap DestinationMap {
            get { return Map.Game.Maps[MapId]; }
        }

        public RedPlusConnection(RedPlusMap map, byte[] data, int offset = 0) {
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

    public class RedPlusWarp {

        public RedPlusMap Origin;
        public byte X;
        public byte Y;
        public byte Index;
        public RedPlusMap Destination;
        public byte DestinationIndex;
        public bool Allowed;

        public RedPlusTile SourceTile {
            get { return Origin.Tiles[X, Y]; }
        }

        public RedPlusWarp DestinationWarp {
            get { return Destination.Warps[DestinationIndex]; }
        }

        public RedPlusTile DestinationTile {
            get { return Destination.Tiles[DestinationWarp.X, DestinationWarp.Y]; }
        }

        public RedPlusWarp(RedPlusMap origin, byte x, byte y, byte index, RedPlusMap destination, byte destinationIndex) => (Origin, X, Y, Index, Destination, DestinationIndex, Allowed) = (origin, x, y, index, destination, destinationIndex, false);

        public override string ToString() {
            return string.Format("{0} at index {1} => {2} at index {3}", SourceTile, Index, DestinationTile, DestinationIndex);
        }

        public static implicit operator byte(RedPlusWarp warp) { return warp.Index; }
    }

    public class RedPlusSign {

        public RedPlusMap Map;
        public byte X;
        public byte Y;
        public byte ScriptId;
        public string Text;

        public RedPlusTile Tile{
            get { return Map.Tiles[X, Y]; }
        }

        public string OneLineText {
            get { return Text.Replace('\n', ' '); }
        }

        public RedPlusSign(RedPlusMap map, byte x, byte y, byte scriptId, string text) => (Map, X, Y, ScriptId, Text) = (map, x, y, scriptId, text);

        public override string ToString() {
            return string.Format("{0}: {1}", Tile, OneLineText);
        }
    }

    public enum RedPlusSpriteMovement : byte {
        Turn = 0xFD,
        Walk = 0xFE,
        Stay = 0xFF,

    }

    public class RedPlusSprite {

        public RedPlusMap Map;
        public byte Id;
        public byte PictureId;
        public byte X;
        public byte Y;
        public RedPlusSpriteMovement Movement;
        public Direction Direction;
        public byte Range;

        public RedPlusTile Tile {
            get { return Map.Tiles[X, Y]; }
        }

        public RedPlusSprite(RedPlusMap map, byte id, byte pictureId, byte x, byte y, RedPlusSpriteMovement movement, Direction direction, byte range) => (Map, Id, PictureId, X, Y, Movement, Direction, Range) = (map, id, pictureId, x, y, movement, direction, range);

        public override string ToString() {
            return string.Format("{0}", Tile);
        }

        public static implicit operator byte(RedPlusSprite sprite) { return sprite.Id; }
    }

    public class RedPlusTrainerClass {

        public byte Id;
        public string Name;
        public List<List<RedPlusPokemon>> Teams;

        public RedPlusTrainerClass(byte id, string name, List<List<RedPlusPokemon>> teams) => (Id, Name, Teams) = (id, name, teams);

        public override string ToString() {
            return Name;
        }

        public static implicit operator byte(RedPlusTrainerClass trainerClass) { return trainerClass.Id; }
    }

    public class RedPlusTrainer : RedPlusSprite {

        public RedPlusTrainerClass TrainerClass;
        public byte TrainerId;
        public byte EventFlagBit;
        public byte ViewRange;
        public ushort EventFlagAddress;

        public List<RedPlusPokemon> Team {
            get { return TrainerClass.Teams[TrainerId - 1]; }
        }

        public RedPlusTrainer(RedPlusSprite baseSprite, RedPlusTrainerClass trainerclass, byte trainerId, byte eventFlagBit, byte viewRange, ushort eventFlagAddress) : base(baseSprite.Map, baseSprite.Id, baseSprite.PictureId, baseSprite.X, baseSprite.Y, baseSprite.Movement, baseSprite.Direction, baseSprite.Range) {
            (TrainerClass, TrainerId, EventFlagBit, ViewRange, EventFlagAddress) = (trainerclass, trainerId, eventFlagBit, viewRange, eventFlagAddress);
        }

        public override string ToString() {
            return base.ToString() + string.Format(": {0} [{1}]", TrainerClass, string.Join(", ", Team));
        }
    }

    public class RedPlusItemball : RedPlusSprite {

        public Item Item;
        public RedPlusItemball(RedPlusSprite baseSprite, Item item) : base(baseSprite.Map, baseSprite.Id, baseSprite.PictureId, baseSprite.X, baseSprite.Y, baseSprite.Movement, baseSprite.Direction, baseSprite.Range) {
            Item = item;
        }

        public override string ToString() {
            return base.ToString() + string.Format(": {0}", Item);
        }
    }

    public class RedPlusPokemon {

        public RedPlusSpecies Species;
        public ushort HP;
        public byte Status;
        public RedPlusMove[] Moves;
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

        public RedPlusPokemon(RedPlusSpecies species, byte level) : this(species, level, 0x9888) { }
        public RedPlusPokemon(RedPlusSpecies species, byte level, DVs dvs) => (Species, Level, DVs) = (species, level, dvs);
        public RedPlusPokemon(RedPlusSpecies species, ushort hp, byte status, RedPlusMove[] moves, DVs dvs, byte[] pp, byte level, ushort maxHp, ushort attack, ushort defense, ushort speed, ushort special) => (Species, HP, Status, Moves, DVs, PP, Level, MaxHP, Attack, Defense, Speed, Special) = (species, hp, status, moves, dvs, pp, level, maxHp, attack, defense, speed, special);

        public override string ToString() {
            return string.Format("L{0} {1} DVs {2:X4}", Level, Species.Name, DVs.Value);
        }

        public static implicit operator RedPlusSpecies(RedPlusPokemon pokemon) { return pokemon.Species; }
    }

    public class RedPlusEncounterSlot {

        public RedPlusMap Map;
        public RedPlusPokemon Pokemon;
        public byte EncounterChance;

        public RedPlusEncounterSlot(RedPlusMap map, RedPlusSpecies species, byte level, byte encounterChance) => (Map, Pokemon, EncounterChance) = (map, new RedPlusPokemon(species, level), encounterChance);

        public override string ToString() {
            return string.Format("{0}/256 {1}", EncounterChance, Pokemon);
        }
    }

    public class RedPlusHiddenItem: ItemStack {

        public RedPlusMap Map;
        public byte X;
        public byte Y;
        public ushort Routine;

        public RedPlusTile Tile {
            get { return Map.Tiles[X, Y]; }
        }

        public RedPlusHiddenItem(RedPlusMap map, byte x, byte y, ushort routine, ItemStack item) : base(item.Item, item.Quantity) {
            (Map, X, Y, Routine) = (map, x, y, routine);
        }

        public override string ToString() {
            return string.Format("{0}: {1}", Tile, base.ToString());
        }
    }

    public class RedPlusEdge : IComparable<RedPlusEdge> {

        public Action Action;
        public RedPlusTile NextTile;
        public int NextEdgeset;
        public int Cost;

        public RedPlusEdge(Action action, RedPlusTile nextTile, int nextEdgeset, int cost) => (Action, NextTile, NextEdgeset, Cost) = (action, nextTile, nextEdgeset, cost);

        public int CompareTo(RedPlusEdge other) {
            return Cost - other.Cost;
        }

        public override string ToString() {
            return Enum.GetName(typeof(Action), Action) + " cost: " + Cost;
        }
    }

    public class RedPlusTile {

        public RedPlusMap Map;
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
        public Dictionary<int, List<RedPlusEdge>> Edges;

        public bool InVisionOfTrainer {
            get {
                foreach(RedPlusTrainer trainer in Map.Trainers) {
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
                RedPlusWarp warp = Map.Warps[X, Y];
                return warp == null ? false : !warp.Allowed;
            }
        }

        public RedPlusTile Up {
            get { return Y == 0 ? null : Map.Tiles[X, Y - 1]; }
        }

        public RedPlusTile Down {
            get { return Y == Map.Height * 2 - 1 ? null : Map.Tiles[X, Y + 1]; }
        }

        public RedPlusTile Left {
            get { return X == 0 ? null : Map.Tiles[X - 1, Y]; }
        }

        public RedPlusTile Right {
            get { return X == Map.Width * 2 - 1 ? null : Map.Tiles[X + 1, Y]; }
        }

        public string PokeworldLink {
            get { return "https://gunnermaniac.com/pokeworld?map=" + (Map.Tileset == 0 || Map.Tileset == 21 ? 1 : Map.Id) + "#" + (X + Map.PokeworldX) + "/" + (Y + Map.PokeworldY); }
        }

        internal RedPlusTile() : this(null, 0, 0, 0, 0, 0, 0, true, false, false, false) { }

        public RedPlusTile(RedPlusMap map, byte x, byte y, byte topLeft, byte topRight, byte bottomLeft, byte bottomRight) : this(map, x, y, topLeft, topRight, bottomLeft, bottomRight, false, false, false, false) { }
        public RedPlusTile(RedPlusMap map, byte x, byte y, byte topLeft, byte topRight, byte bottomLeft, byte bottomRight, bool solid, bool downHop, bool leftHop, bool rightHop) : this(map, x, y, topLeft, topRight, bottomLeft, bottomRight, solid, downHop, leftHop, rightHop, false, false, false, false) { }
        public RedPlusTile(RedPlusMap map, byte x, byte y, byte topLeft, byte topRight, byte bottomLeft, byte bottomRight, bool solid, bool downHop, bool leftHop, bool rightHop, bool canMoveDown, bool canMoveUp, bool canMoveLeft, bool canMoveRight) => (Map, X, Y, TopLeft, TopRight, BottomLeft, BottomRight, Solid, DownHop, LeftHop, RightHop, CanMoveDown, CanMoveUp, CanMoveLeft, CanMoveRight, Edges) = (map, x, y, topLeft, topRight, bottomLeft, bottomRight, solid, downHop, leftHop, rightHop, canMoveDown, canMoveUp, canMoveLeft, canMoveRight, new Dictionary<int, List<RedPlusEdge>>());

        public void AddEdge(int edgeSet, RedPlusEdge edge) {
            if(!Edges.ContainsKey(edgeSet)) {
                Edges[edgeSet] = new List<RedPlusEdge>();
            }
            Edges[edgeSet].Add(edge);
            Edges[edgeSet].Sort();
        }

        public RedPlusEdge GetEdge(int edgeSet, Action action) {
            if(!Edges.ContainsKey(edgeSet)) {
                return null;
            }

            foreach(RedPlusEdge edge in Edges[edgeSet]) {
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
            RedPlusEdge edge = GetEdge(edgeSet, action);
            if(edge == null) {
                return;
            }

            Edges[edgeSet].Remove(edge);
            Edges[edgeSet].Sort();
        }

        public override string ToString() {
            return string.Format("{0}#{1},{2}", Map.Id, X, Y);
        }
    }

    public class RedPlusMap {

        public RedPlus Game;
        public string Name;
        public byte Id;
        public byte Bank;
        public RedPlusTileset Tileset;
        public byte Height;
        public byte Width;
        public ushort DataPointer;
        public ushort TextPointer;
        public ushort ScriptPointer;
        public byte ConnectionFlags;
        public RedPlusConnection[] Connections;
        public ushort ObjectPointer;

        public DataArray<RedPlusTile> Tiles;

        public byte BorderBlock;
        public DataArray<RedPlusWarp> Warps;
        public DataArray<RedPlusSign> Signs;
        public DataArray<RedPlusSprite> Sprites;
        public DataArray<RedPlusTrainer> Trainers;
        public DataArray<RedPlusItemball> Itemballs;

        public byte EncounterRate;
        public List<RedPlusEncounterSlot> EncounterSlots;

        public DataArray<RedPlusHiddenItem> HiddenItems;

        public int PokeworldX;
        public int PokeworldY;

        public RedPlusConnection EastConnection {
            get { return Connections[0]; }
        }

        public RedPlusConnection WestConnection {
            get { return Connections[1]; }
        }

        public RedPlusConnection SouthConnection {
            get { return Connections[2]; }
        }

        public RedPlusConnection NorthConnection {
            get { return Connections[3]; }
        }

        public RedPlusMap(RedPlus game, string name, byte id, byte bank, byte[] data, byte[] wildData, int offset = 0) {
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
            Connections = new RedPlusConnection[4];
            offset += 10;

            for(int i = 3; i >= 0; i--) {
                if(((ConnectionFlags >> i) & 1) == 1) {
                    Connections[i] = new RedPlusConnection(this, data, offset);
                    offset += 11;
                }
            }

            ObjectPointer = data.ReadWord(offset);

            EncounterRate = wildData[0];
            EncounterSlots = new List<RedPlusEncounterSlot>();

            if(EncounterRate > 0) {
                byte[] encounterChances = { 51, 51, 39, 25, 25, 25, 13, 13, 11, 3 };

                int pointer = 1;
                for(int i = 0; i < encounterChances.Length; i++) {
                    byte encounterChance = encounterChances[i];
                    byte level = wildData[pointer++];
                    byte species = wildData[pointer++];
                    EncounterSlots.Add(new RedPlusEncounterSlot(this, Game.Species[species], level, encounterChance));
                }
            }

            Tiles = new DataArray<RedPlusTile>(Width * 2, Height * 2);
            Warps = new DataArray<RedPlusWarp>(Width * 2, Height * 2, new Dictionary<Type, Func<RedPlusWarp, object, bool>>() {
                    { typeof(int), (warp, indexNumber) => { return warp.Index == (int) indexNumber; } },
                    { typeof(byte), (warp, indexNumber) => { return warp.Index == (byte) indexNumber; } },
                    });
            Signs = new DataArray<RedPlusSign>(Width * 2, Height * 2);
            Sprites = new DataArray<RedPlusSprite>(Width * 2, Height * 2);
            Trainers = new DataArray<RedPlusTrainer>(Width * 2, Height * 2);
            Itemballs = new DataArray<RedPlusItemball>(Width * 2, Height * 2);
            HiddenItems = new DataArray<RedPlusHiddenItem>(Width * 2, Height * 2);
        }

        public RedPlusTile this[byte x, byte y] {
            get { return Tiles[x, y]; }
            set { Tiles[x, y] = value; }
        }

        public RedPlusTile this[int x, int y] {
            get { return Tiles[x, y]; }
            set { Tiles[x, y] = value; }
        }

        public static implicit operator byte(RedPlusMap map) { return map.Id; }
    }

    public class RedPlusBag : IEnumerable<ItemStack> {
        public RedPlus Game;
        public ItemStack[] Items;

        public int NumItems {
            get { return Items.Length; }
        }

        public RedPlusBag(RedPlus game, ItemStack[] items) => (Game, Items) = (game, items);

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

    internal class RedPlusData {

        public RedPlusCharmap Charmap;
        public DataMap<RedPlusSpecies> Species;
        public DataMap<Item> Items;
        public DataMap<RedPlusMove> Moves;
        public DataMap<RedPlusTrainerClass> TrainerClasses;
        public DataMap<RedPlusTileset> Tilesets;
        public DataMap<RedPlusMap> Maps;

        public RedPlusData() {
            Charmap = new RedPlusCharmap();
            Species = new DataMap<RedPlusSpecies>();
            Items = new DataMap<Item>();
            Moves = new DataMap<RedPlusMove>();
            TrainerClasses = new DataMap<RedPlusTrainerClass>();
            Tilesets = new DataMap<RedPlusTileset>();
            Maps = new DataMap<RedPlusMap>();

            Species.Callbacks = new Dictionary<Type, Func<RedPlusSpecies, object, bool>>() {
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

            Moves.Callbacks = new Dictionary<Type, Func<RedPlusMove, object, bool>>() {
                { typeof(string), (move, name) => { return move.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (move, id) => { return move.Id == (int) id; } },
                { typeof(uint), (move, id) => { return move.Id == (uint) id; } },
                { typeof(byte), (move, id) => { return move.Id == (byte) id; } },
            };

            TrainerClasses.Callbacks = new Dictionary<Type, Func<RedPlusTrainerClass, object, bool>>() {
                { typeof(string), (trainerclass, name) => { return trainerclass.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (trainerclass, id) => { return trainerclass.Id == (int) id; } },
                { typeof(uint), (trainerclass, id) => { return trainerclass.Id == (uint) id; } },
                { typeof(byte), (trainerclass, id) => { return trainerclass.Id == (byte) id; } },
            };

            Tilesets.Callbacks = new Dictionary<Type, Func<RedPlusTileset, object, bool>>() {
                { typeof(int), (tileset, id) => { return tileset.Id == (int) id; } },
                { typeof(uint), (tileset, id) => { return tileset.Id == (uint) id; } },
                { typeof(byte), (tileset, id) => { return tileset.Id == (byte) id; } },
            };

            Maps.Callbacks = new Dictionary<Type, Func<RedPlusMap, object, bool>>() {
                { typeof(string), (map, name) => { return map.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (map, id) => { return map.Id == (int) id; } },
                { typeof(uint), (map, id) => { return map.Id == (uint) id; } },
                { typeof(byte), (map, id) => { return map.Id == (byte) id; } },
            };
        }
    }


    public class RedPlus : GameBoy {

        public static readonly Dictionary<(string, byte), byte> wLastMapDestinations = new Dictionary<(string, byte), byte>() {
            { ("RedsHouse1F", 0), 0 },
            { ("RedsHouse1F", 1), 0 },
            { ("BluesHouse", 0), 0 },
            { ("BluesHouse", 1), 0 },
            { ("OaksLab", 0), 0 },
            { ("OaksLab", 1), 0 },
            { ("ViridianPokecenter", 0), 1 },
            { ("ViridianPokecenter", 1), 1 },
            { ("ViridianMart", 0), 1 },
            { ("ViridianMart", 1), 1 },
            { ("School", 0), 1 },
            { ("School", 1), 1 },
            { ("ViridianHouse", 0), 1 },
            { ("ViridianHouse", 1), 1 },
            { ("ViridianGym", 0), 1 },
            { ("ViridianGym", 1), 1 },
            { ("DiglettsCaveRoute2", 0), 13 },
            { ("DiglettsCaveRoute2", 1), 13 },
            { ("ViridianForestExit", 0), 13 },
            { ("ViridianForestExit", 1), 13 },
            { ("Route2House", 0), 13 },
            { ("Route2House", 1), 13 },
            { ("Route2Gate", 0), 13 },
            { ("Route2Gate", 1), 13 },
            { ("Route2Gate", 2), 13 },
            { ("Route2Gate", 3), 13 },
            { ("ViridianForestEntrance", 2), 13 },
            { ("ViridianForestEntrance", 3), 13 },
            { ("Museum1F", 0), 2 },
            { ("Museum1F", 1), 2 },
            { ("Museum1F", 2), 2 },
            { ("Museum1F", 3), 2 },
            { ("PewterGym", 0), 2 },
            { ("PewterGym", 1), 2 },
            { ("PewterHouse1", 0), 2 },
            { ("PewterHouse1", 1), 2 },
            { ("PewterMart", 0), 2 },
            { ("PewterMart", 1), 2 },
            { ("PewterHouse2", 0), 2 },
            { ("PewterHouse2", 1), 2 },
            { ("PewterPokecenter", 0), 2 },
            { ("PewterPokecenter", 1), 2 },
            { ("MtMoon1", 0), 15 },
            { ("MtMoon1", 1), 15 },
            { ("MtMoon2", 7), 15 },
            { ("CeruleanHouseTrashed", 0), 3 },
            { ("CeruleanHouseTrashed", 1), 3 },
            { ("CeruleanHouseTrashed", 2), 3 },
            { ("CeruleanHouse1", 0), 3 },
            { ("CeruleanHouse1", 1), 3 },
            { ("CeruleanPokecenter", 0), 3 },
            { ("CeruleanPokecenter", 1), 3 },
            { ("CeruleanGym", 0), 3 },
            { ("CeruleanGym", 1), 3 },
            { ("BikeShop", 0), 3 },
            { ("BikeShop", 1), 3 },
            { ("CeruleanMart", 0), 3 },
            { ("CeruleanMart", 1), 3 },
            { ("MtMoonPokecenter", 0), 15 },
            { ("MtMoonPokecenter", 1), 15 },
            { ("Route5Gate", 0), 16 },
            { ("Route5Gate", 1), 16 },
            { ("Route5Gate", 2), 16 },
            { ("Route5Gate", 3), 16 },
            { ("UndergroundPathEntranceRoute5", 0), 16 },
            { ("UndergroundPathEntranceRoute5", 1), 16 },
            { ("UndergroundPathEntranceRoute6", 0), 17 },
            { ("UndergroundPathEntranceRoute6", 1), 17 },
            { ("UndergroundPathEntranceRoute7", 0), 18 },
            { ("UndergroundPathEntranceRoute7", 1), 18 },
            { ("UndergroundPathEntranceRoute7Copy", 0), 18 },
            { ("UndergroundPathEntranceRoute7Copy", 1), 18 },
            { ("UndergroundPathEntranceRoute8", 0), 19 },
            { ("UndergroundPathEntranceRoute8", 1), 19 },
            { ("DayCareM", 0), 16 },
            { ("DayCareM", 1), 16 },
            { ("Route6Gate", 0), 17 },
            { ("Route6Gate", 1), 17 },
            { ("Route6Gate", 2), 17 },
            { ("Route6Gate", 3), 17 },
            { ("Route7Gate", 0), 18 },
            { ("Route7Gate", 1), 18 },
            { ("Route7Gate", 2), 18 },
            { ("Route7Gate", 3), 18 },
            { ("Route8Gate", 0), 19 },
            { ("Route8Gate", 1), 19 },
            { ("Route8Gate", 2), 19 },
            { ("Route8Gate", 3), 19 },
            { ("RockTunnelPokecenter", 0), 21 },
            { ("RockTunnelPokecenter", 1), 21 },
            { ("RockTunnel1", 0), 21 },
            { ("RockTunnel1", 1), 21 },
            { ("RockTunnel1", 2), 21 },
            { ("RockTunnel1", 3), 21 },
            { ("PowerPlant", 0), 21 },
            { ("PowerPlant", 1), 21 },
            { ("PowerPlant", 2), 21 },
            { ("Route11Gate", 0), 22 },
            { ("Route11Gate", 1), 22 },
            { ("Route11Gate", 2), 22 },
            { ("Route11Gate", 3), 22 },
            { ("DiglettsCaveEntranceRoute11", 0), 22 },
            { ("DiglettsCaveEntranceRoute11", 1), 22 },
            { ("Route12Gate", 0), 23 },
            { ("Route12Gate", 1), 23 },
            { ("Route12Gate", 2), 23 },
            { ("Route12Gate", 3), 23 },
            { ("BillsHouse", 0), 36 },
            { ("BillsHouse", 1), 36 },
            { ("VermilionPokecenter", 0), 5 },
            { ("VermilionPokecenter", 1), 5 },
            { ("FanClub", 0), 1 },
            { ("FanClub", 1), 1 },
            { ("VermilionMart", 0), 5 },
            { ("VermilionMart", 1), 5 },
            { ("VermilionGym", 0), 5 },
            { ("VermilionGym", 1), 5 },
            { ("VermilionHouse1", 0), 5 },
            { ("VermilionHouse1", 1), 5 },
            { ("VermilionDock", 0), 5 },
            { ("VictoryRoad1", 0), 34 },
            { ("VictoryRoad1", 1), 34 },
            { ("CeladonMart1", 0), 6 },
            { ("CeladonMart1", 1), 6 },
            { ("CeladonMart1", 2), 6 },
            { ("CeladonMart1", 3), 6 },
            { ("CeladonMansion1", 0), 6 },
            { ("CeladonMansion1", 1), 6 },
            { ("CeladonMansion1", 2), 6 },
            { ("CeladonPokecenter", 0), 6 },
            { ("CeladonPokecenter", 1), 6 },
            { ("CeladonGym", 0), 6 },
            { ("CeladonGym", 1), 6 },
            { ("CeladonGameCorner", 0), 10 },
            { ("CeladonGameCorner", 1), 10 },
            { ("CeladonPrizeRoom", 0), 6 },
            { ("CeladonPrizeRoom", 1), 6 },
            { ("CeladonDiner", 0), 6 },
            { ("CeladonDiner", 1), 6 },
            { ("CeladonHouse", 0), 6 },
            { ("CeladonHouse", 1), 6 },
            { ("CeladonHotel", 0), 6 },
            { ("CeladonHotel", 1), 6 },
            { ("LavenderPokecenter", 0), 4 },
            { ("LavenderPokecenter", 1), 4 },
            { ("PokemonTower1", 0), 4 },
            { ("PokemonTower1", 1), 4 },
            { ("LavenderHouse1", 0), 4 },
            { ("LavenderHouse1", 1), 4 },
            { ("LavenderMart", 0), 4 },
            { ("LavenderMart", 1), 4 },
            { ("LavenderHouse2", 0), 4 },
            { ("LavenderHouse2", 1), 4 },
            { ("FuchsiaMart", 0), 7 },
            { ("FuchsiaMart", 1), 7 },
            { ("FuchsiaHouse1", 0), 7 },
            { ("FuchsiaHouse1", 1), 7 },
            { ("FuchsiaPokecenter", 0), 7 },
            { ("FuchsiaPokecenter", 1), 7 },
            { ("FuchsiaHouse2", 0), 7 },
            { ("FuchsiaHouse2", 1), 7 },
            { ("FuchsiaGym", 0), 7 },
            { ("FuchsiaGym", 1), 7 },
            { ("FuchsiaMeetingRoom", 0), 7 },
            { ("FuchsiaMeetingRoom", 1), 7 },
            { ("VermilionHouse2", 0), 5 },
            { ("VermilionHouse2", 1), 5 },
            { ("FuchsiaHouse3", 0), 7 },
            { ("FuchsiaHouse3", 1), 7 },
            { ("FuchsiaHouse3", 2), 7 },
            { ("Mansion1", 0), 8 },
            { ("Mansion1", 1), 8 },
            { ("Mansion1", 2), 8 },
            { ("Mansion1", 3), 8 },
            { ("Mansion1", 6), 8 },
            { ("Mansion1", 7), 8 },
            { ("CinnabarGym", 0), 8 },
            { ("CinnabarGym", 1), 8 },
            { ("Lab1", 0), 8 },
            { ("Lab1", 1), 8 },
            { ("CinnabarPokecenter", 0), 8 },
            { ("CinnabarPokecenter", 1), 8 },
            { ("CinnabarMart", 0), 8 },
            { ("CinnabarMart", 1), 8 },
            { ("IndigoPlateauLobby", 0), 9 },
            { ("IndigoPlateauLobby", 1), 9 },
            { ("CopycatsHouse1F", 0), 10 },
            { ("CopycatsHouse1F", 1), 10 },
            { ("FightingDojo", 0), 10 },
            { ("FightingDojo", 1), 10 },
            { ("SaffronGym", 0), 10 },
            { ("SaffronGym", 1), 10 },
            { ("SaffronHouse1", 0), 10 },
            { ("SaffronHouse1", 1), 10 },
            { ("SaffronMart", 0), 10 },
            { ("SaffronMart", 1), 10 },
            { ("SilphCo1", 0), 10 },
            { ("SilphCo1", 1), 10 },
            { ("SaffronPokecenter", 0), 10 },
            { ("SaffronPokecenter", 1), 10 },
            { ("SaffronHouse2", 0), 10 },
            { ("SaffronHouse2", 1), 10 },
            { ("Route15Gate", 0), 26 },
            { ("Route15Gate", 1), 26 },
            { ("Route15Gate", 2), 26 },
            { ("Route15Gate", 3), 26 },
            { ("Route16Gate", 0), 27 },
            { ("Route16Gate", 1), 27 },
            { ("Route16Gate", 2), 27 },
            { ("Route16Gate", 3), 27 },
            { ("Route16Gate", 4), 27 },
            { ("Route16Gate", 5), 27 },
            { ("Route16Gate", 6), 27 },
            { ("Route16Gate", 7), 27 },
            { ("Route16House", 0), 27 },
            { ("Route16House", 1), 27 },
            { ("Route12House", 0), 23 },
            { ("Route12House", 1), 23 },
            { ("Route18Gate", 0), 29 },
            { ("Route18Gate", 1), 29 },
            { ("Route18Gate", 2), 29 },
            { ("Route18Gate", 3), 29 },
            { ("SeafoamIslands1", 0), 31 },
            { ("SeafoamIslands1", 1), 31 },
            { ("SeafoamIslands1", 2), 31 },
            { ("SeafoamIslands1", 3), 31 },
            { ("Route22Gate", 0), 33 },
            { ("Route22Gate", 1), 33 },
            { ("Route22Gate", 2), 34 },
            { ("Route22Gate", 3), 34 },
            { ("VictoryRoad2", 1), 34 },
            { ("VictoryRoad2", 2), 34 },
            { ("VermilionHouse3", 0), 5 },
            { ("VermilionHouse3", 1), 5 },
            { ("Route19Gate", 0), 30 },
            { ("Route19Gate", 1), 30 },
            { ("Route19Gate", 2), 7 },
            { ("Route19Gate", 3), 7 },
            { ("BeachHouse", 0), 30 },
            { ("BeachHouse", 1), 30 },
            { ("UnknownDungeon1", 0), 3 },
            { ("UnknownDungeon1", 1), 3 },
            { ("NameRater", 0), 4 },
            { ("NameRater", 1), 4 },
            { ("CeruleanHouse2", 0), 3 },
            { ("CeruleanHouse2", 1), 3 },
            { ("CeruleanHouse2", 2), 3 },
            { ("SilphCo11", 2), 10 },
            { ("SilphCoElevator", 0), 181 },
            { ("SilphCoElevator", 1), 181 },
        };

        private static RedPlusData Data;

        public RedPlusCharmap Charmap {
            get { return Data.Charmap; }
        }

        public DataMap<RedPlusSpecies> Species {
            get { return Data.Species; }
        }

        public DataMap<Item> Items {
            get { return Data.Items; }
        }

        public DataMap<RedPlusMove> Moves {
            get { return Data.Moves; }
        }

        public DataMap<RedPlusTrainerClass> TrainerClasses {
            get { return Data.TrainerClasses; }
        }

        public DataMap<RedPlusTileset> Tilesets {
            get { return Data.Tilesets; }
        }

        public DataMap<RedPlusMap> Maps {
            get { return Data.Maps; }
        }

        public RedPlus(bool speedup = false, string saveName = "roms/red++.sav", string rom = "roms/red++.gbc") : this(rom, saveName, speedup ? SpeedupFlags.NoSound | SpeedupFlags.NoVideo : SpeedupFlags.None) { }


        public RedPlus(string rom, string saveName, SpeedupFlags speedupFlags = SpeedupFlags.All) : base(rom, saveName, "roms/gbc_bios.bin", speedupFlags) {
            if(Data == null) {
                Data = new RedPlusData();
                LoadSpecies();
                LoadItems();
                LoadMoves();
                LoadTrainerClasses();
                LoadTilesets();
                LoadMaps();
                LoadMapTiles();
                LoadMapObjects();
                // FindPokeworldOffests(Maps[0], 50, 234, new HashSet<RedPlusMap>());
                LoadHiddenItems();
            }
        }

        public override byte ReadHRA() {
            return CpuRead(0xFFD3);
        }

        public override byte ReadHRS() {
            return CpuRead(0xFFD4);
        }

        public string getIGT() {
            return $"Second: {CpuRead("wPlayTimeSeconds")}, Frame: {CpuRead("wPlayTimeFrames")}";
        }

        private void LoadSpecies() {
            int maxIndexNumber = 208;
            int nameLength = 10;

            RomAddress namesStart = Sym["MonsterNames"];
            RomAddress baseStatsStart = Sym["MonBaseStats"];
            int baseStatsSize = Sym["MonBaseStatsEnd"] - baseStatsStart;
            int numBaseStats = (Sym["Music_TCGHallOfHonor"] - baseStatsStart) / baseStatsSize;
            byte[] pokedex = Rom.Subarray(Sym["PokedexOrder"], maxIndexNumber);

            for(int i = 0; i < numBaseStats; i++) {
                byte[] header = Rom.Subarray(baseStatsStart + i * baseStatsSize, baseStatsSize);
                byte indexNumber = (byte) Array.IndexOf(pokedex, header[0]);
                string name = Charmap.Decode(Rom.Subarray(namesStart + indexNumber * nameLength, nameLength));
                Species.Add(new RedPlusSpecies(this, name, ++indexNumber, header));
            }
        }

        private void LoadItems() {
            int numItems = 108;

            int namePointer = Sym["ItemNames"].Rom();
            for(int i = 0; i < numItems; i++) {
                Items.Add(new Item((byte) (i+1), Charmap.Decode(Rom.ReadUntil(RedPlusCharmap.Terminator, ref namePointer))));
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
            int numMoves = (Sym["CryData"] - movesStart) / moveSize;

            int namePointer = Sym["MoveNames"].Rom();
            for(int i = 0; i < numMoves; i++) {
                string name = Charmap.Decode(Rom.ReadUntil(RedPlusCharmap.Terminator, ref namePointer));
                byte[] header = Rom.Subarray(movesStart + i * moveSize, moveSize);
                Moves.Add(new RedPlusMove(this, name, header));
            }
        }

        private void LoadTrainerClasses() {
            // trainernames and trainerdata does not line up
            // need some way to translate trainername and trainerdata
            int numTrainerClasses = 49;
            List<string> trainerClassNames = new List<string>()
            {
                "Youngster", "Bug Catcher", "Lass", "Sailor", "Camper", "Picnicker", "Poké Maniac", "Super Nerd", "Hiker",
                "Biker", "Burglar", "Engineer", "Couple", "Fisherman", "SwimmerM", "Roughneck", "Gambler", "Beauty", "Psychic",
                "Guitarist", "Juggler", "Tamer", "Bird Keeper", "Black Belt", "Rival", "SwimmerF", "RocketF",
                "Scientist", "Boss", "Rocket", "Ace TrainerM", "Ace TrainerF", "Bruno", "Brock", "Misty", "Surge",
                "Erika", "Koga", "Blaine", "Sabrina", "Gentleman", "Rival2", "Champion", "Lorelei", "Channeler",
                "Agatha", "Lance", "Hex Maniac", "Trainer",
            };


            RomAddress trainerDataPointer = Sym["TrainerDataPointers"];
            // Console.WriteLine("Base address: " + trainerDataPointer.Value);
            // Console.WriteLine("Len: " + trainerClassNames.Count);

            Dictionary<String, RomAddress> classDataStart = new Dictionary<string, RomAddress>();
            for (int i = 0; i < numTrainerClasses; i++) {
                string name = trainerClassNames[i];
                RomAddress dataStart = new RomAddress(0x3B, Rom.ReadWord(trainerDataPointer + i*2));
                classDataStart.Add(name, dataStart);
                List<List<RedPlusPokemon>> teams = new List<List<RedPlusPokemon>>(); // for now, did not add team/parties
                TrainerClasses.Add(new RedPlusTrainerClass((byte) (i+201), name, teams));
            }
        }

        private void LoadTilesets() {
            Dictionary<byte, List<(byte, byte)>> tilePairCollisionsLand = new Dictionary<byte, List<(byte, byte)>>();
            int pointer = Sym["TilePairCollisionsLand"].Rom();

            byte tileset;
            while((tileset = Rom[pointer++]) != 0xFF) {
                if(!tilePairCollisionsLand.ContainsKey(tileset)) tilePairCollisionsLand[tileset] = new List<(byte, byte)>();
                tilePairCollisionsLand[tileset].Add((Rom[pointer++], Rom[pointer++]));
            }

            int numTilesets = 28;
            pointer = Sym["Tilesets"].Rom();

            for(int i = 0; i < numTilesets; i++) {
                byte bank = Rom[pointer++];
                ushort blockPointer = Rom.ReadWord(ref pointer);
                ushort gfxPointer = Rom.ReadWord(ref pointer);
                ushort collPointer = Rom.ReadWord(ref pointer);
                byte[] counterTiles = Rom.Subarray(ref pointer, 3);
                byte grassTile = Rom[pointer++];
                RedPlusPermission permission = (RedPlusPermission) Rom[pointer++];

                byte[] collisionData = new byte[0];
                int collisionDataPointer = new RomAddress((byte) 0x01, collPointer);
                byte tile;
                while((tile = Rom[collisionDataPointer++]) != 0xFF) {
                    collisionData = collisionData.Add(tile);
                }

                Tilesets.Add(new RedPlusTileset(this, (byte) i, bank, blockPointer, gfxPointer, collPointer, counterTiles, grassTile, permission, collisionData, tilePairCollisionsLand.GetValueOrDefault((byte) i, new List<(byte, byte)>())));
            }
        }

        private void LoadMaps() {
            int numMaps = 248;

            for(int i = 0; i < numMaps; i++) {
                byte headerBank = Rom[Sym["MapHeaderBanks"].Rom() + i];
                ushort headerPointer = Rom.ReadWord(Sym["MapHeaderPointers"].Rom() + i*2);
                string name = Sym.GetAddressName(new CpuAddress((uint) (headerBank << 16) | headerPointer));
                if(name != null && name.Contains("_h")) {
                    name = name.Substring(0, name.IndexOf("_h"));

                    ushort wildDataPointer = Rom.ReadWord(Sym["WildDataPointers"].Rom() + i * 2);
                    byte[] wildData = Rom.Subarray(new RomAddress(Sym["WildDataPointers"].Bank, wildDataPointer), Sym["Route2Mons"].Rom() - Sym["Route1Mons"].Rom());
                    Maps.Add(new RedPlusMap(this, name, (byte) i, headerBank, Ram[headerBank], wildData, headerPointer));
                }
            }
        }

        private void LoadMapTiles() {
            foreach(RedPlusMap map in Maps) {
                RedPlusTileset tileset = map.Tileset;
                byte width = map.Width;
                byte height = map.Height;
                byte[] blocks = Rom.Subarray(new RomAddress(map.Bank, map.DataPointer), width * height);

                int length = blocks.Length - blocks.Length % width;
                byte[] tiles = new byte[length * 4 * 4];
                for(int i = 0; i < length; i++) {
                    byte[] tile2 = Rom.Subarray(new RomAddress(tileset.Bank, tileset.BlockPointer) + blocks[i] * 16, 16);
                    for(int j = 0; j < 4; j++) {
                        Array.Copy(tile2, j * 4, tiles, (i / width) * (width * 4) * 4 + j * (width * 4) + (i % width) * 4, 4);
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

                        RedPlusTile tile = new RedPlusTile(map, xTile, yTile, tl, tr, bl, br);

                        if(Array.IndexOf(tileset.CollisionData, bl) == -1) {
                            tile.Solid = true;
                        }

                        if(tileset.Id == 0 && tile.Solid) {
                            if (br == 0x37) tile.DownHop = true;
                            else if (tl == 0x27 && bl == 0x24) tile.LeftHop = true;
                            else if (tr == 0x24 && br == 0x24) tile.RightHop = true;
                        }

                        map.Tiles[xTile, yTile] = tile;
                    }
                }

                for(byte y = 0; y < mapHeight; y++) {
                    for(byte x = 0; x < mapWidth; x++) {
                        RedPlusTile down = y == mapHeight - 1 ? new RedPlusTile() : map.Tiles[x, y + 1];
                        RedPlusTile up = y == 0 ? new RedPlusTile() : map.Tiles[x, y - 1];
                        RedPlusTile left = x == 0 ? new RedPlusTile() : map.Tiles[x - 1, y];
                        RedPlusTile right = x == mapWidth - 1 ? new RedPlusTile() : map.Tiles[x + 1, y];
                        RedPlusTile tile = map.Tiles[x,  y];

                        tile.CanMoveDown = !(tileset.TilePairCollisions.Contains((down.BottomLeft, tile.BottomLeft)) || tileset.TilePairCollisions.Contains((tile.BottomLeft, down.BottomLeft))) && (!down.Solid || down.DownHop);
                        tile.CanMoveUp = !(tileset.TilePairCollisions.Contains((up.BottomLeft, tile.BottomLeft)) || tileset.TilePairCollisions.Contains((tile.BottomLeft, up.BottomLeft))) && (!up.Solid);
                        tile.CanMoveLeft = !(tileset.TilePairCollisions.Contains((left.BottomLeft, tile.BottomLeft)) || tileset.TilePairCollisions.Contains((tile.BottomLeft, left.BottomLeft))) && (!left.Solid || left.LeftHop);
                        tile.CanMoveRight = !(tileset.TilePairCollisions.Contains((right.BottomLeft, tile.BottomLeft)) || tileset.TilePairCollisions.Contains((tile.BottomLeft, right.BottomLeft))) && (!right.Solid || right.RightHop);

                    }
                }
            }
        }

        private void LoadMapObjects() {
            foreach(RedPlusMap map in Maps) {
                int pointer = new RomAddress(map.Bank, map.ObjectPointer);

                map.BorderBlock = Rom[pointer++];

                byte numWarps = Rom[pointer++];
                for(byte index = 0; index < numWarps; index++) {
                    byte y = Rom[pointer++];
                    byte x = Rom[pointer++];
                    byte destinationIndex = Rom[pointer++];
                    byte destinationMap = Rom[pointer++];

                    if(destinationMap == 0xFF || destinationMap == 0xED) {
                        destinationMap = RedPlus.wLastMapDestinations[(map.Name, index)];
                        if(map.Name == "SilphCo11F") {
                            destinationIndex = 0;
                        }
                    }
                    map.Warps[x, y] = new RedPlusWarp(map, x, y, index, Maps[destinationMap], destinationIndex);
                }

                byte numSigns = Rom[pointer++];
                for(byte index = 0; index < numSigns; index++) {
                    byte y = Rom[pointer++];
                    byte x = Rom[pointer++];
                    byte scriptId = Rom[pointer++];

                    ushort scriptAddr = Rom.ReadWord(new RomAddress(map.Bank, (ushort) (map.TextPointer + (scriptId - 1) * 2)));
                    RomAddress addr = new RomAddress(map.Bank, scriptAddr);
                    if(Sym.GetAddressName(addr) == null) addr = new RomAddress(0, scriptAddr);

                    if(Rom[addr] != 0x17) continue; // ?

                    uint textAddress = new RomAddress(Rom[addr + 3], Rom.ReadWord(addr + 1));

                    bool doneReading = false;
                    string text ="";
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

                    map.Signs[x, y] = new RedPlusSign(map, x, y, scriptId, text);
                }

                byte numSprites = Rom[pointer++];
                for(byte index = 0; index < numSprites; index++) {
                    byte spriteId = Rom[pointer++];
                    byte y = (byte) (Rom[pointer++] - 4);
                    byte x = (byte) (Rom[pointer++] - 4);
                    RedPlusSpriteMovement movement = (RedPlusSpriteMovement) Rom[pointer++];
                    byte rangeOrDirection = Rom[pointer++];
                    byte range = 0;
                    Direction direction = Direction.None;
                    byte textId = Rom[pointer++];
                    bool isTrainer = (textId & (1 << 6)) != 0;
                    bool isItemBall = (textId & (1 << 7)) != 0;
                    textId &= 0xFF ^ (1 << 6 | 1 << 7);

                    if(movement == RedPlusSpriteMovement.Walk) {
                        range = rangeOrDirection;
                    } else {
                        direction = DirectionFunctions.RbySpriteDirection(rangeOrDirection);
                        if(direction == Direction.None) {
                            movement = RedPlusSpriteMovement.Turn;
                        }
                    }

                    RedPlusSprite baseSprite = new RedPlusSprite(map, (byte) (index + 1), spriteId, x, y, movement, direction, range);
                    map.Sprites[x, y] = baseSprite;

                    if(isTrainer) {
                        byte trainerClassId = Rom[pointer++];
                        byte trainerId = Rom[pointer++];
                        if(trainerClassId < 200) continue; // stationary encounters

                        RedPlusTrainerClass trainerClass = TrainerClasses[trainerClassId];

                        int textPointer = new RomAddress(map.Bank, map.TextPointer) + (textId - 1) * 2;
                        int scriptPointer = new RomAddress(map.Bank, Rom.ReadWord(textPointer));
                        int headerPointer = new RomAddress(map.Bank, Rom.ReadWord(scriptPointer + 2));

                        byte eventFlagBit = Rom[headerPointer++];
                        byte viewRange = (byte) (Rom[headerPointer++] >> 4);
                        ushort eventFlagAddress = Rom.ReadWord(ref headerPointer);

                        map.Trainers[x, y] = new RedPlusTrainer(baseSprite, trainerClass, trainerId, eventFlagBit, viewRange, eventFlagAddress);
                    } else if(isItemBall) {
                        map.Itemballs[x,  y] = new RedPlusItemball(baseSprite, Items[Rom[pointer++]]);
                    }
                }
            }
        }

        private void LoadHiddenItems() {
            for(int i = 0; i < 85; i++) {
                RedPlusMap map = Maps[Rom[Sym["HiddenObjectMaps"].Rom() + i]];
                int pointer = new RomAddress(Sym["HiddenObjectMaps"].Bank, Rom.ReadWord(Sym["HiddenObjectPointers"].Rom() + i * 2));

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
                    } else if(routine == Sym["HiddenItems"].Rom().Address) {
                        byte quantity = (byte) (id - Items["Coin"].Id);
                        if(quantity == 40) quantity = 20; // ??????
                        itemstack = new ItemStack(Items["Coin"], quantity);
                    }

                    if(itemstack != null && x > 0 && y > 0 && x < map.Width * 2 - 1 && y < map.Height * 2 - 1) {
                        map.HiddenItems[x, y] = new RedPlusHiddenItem(map, x, y, routine, itemstack);
                    }
                }
            }
        }

        public RedPlusMap GetCurMap() {
            return Maps[CpuRead(Sym["wCurMap"])];
        }

        public RedPlusTile GetCurTile() {
            RedPlusMap map = GetCurMap();
            return map == null ? null : map.Tiles[CpuRead(Sym["wXCoord"]), CpuRead(Sym["wYCoord"])];
        }

        public RedPlusPokemon CreateEnemyMon(CpuAddress address) {
            RedPlusSpecies species = Species[CpuRead(address)];
            ushort hp = CpuReadWord(address + 1);
            byte status = CpuRead(address + 4);
            RedPlusMove[] moves = new RedPlusMove[4];
            DVs dvs = new DVs(CpuReadWord(address + 12));
            byte[] pp = new byte[4];
            byte level = CpuRead(address + 14);
            ushort maxHp = CpuReadWord(address + 15);
            ushort attack = CpuReadWord(address + 17);
            ushort defense = CpuReadWord(address + 19);
            ushort speed = CpuReadWord(address + 21);
            ushort special = CpuReadWord(address + 23);
            ReadMovesAndPP(address + 8, address + 25, ref moves, ref pp);
            return new RedPlusPokemon(species, hp, status, moves, dvs, pp, level, maxHp, attack, defense, speed, special);
        }

        private void ReadMovesAndPP(CpuAddress moveAddress, CpuAddress ppAddress, ref RedPlusMove[] moves, ref byte[] pp) {
            for(byte i = 0; i < 4; i++) {
                moves[i] = Moves[CpuRead(moveAddress + i)];
                pp[i] = CpuRead(ppAddress + i);
            }
        }

        public RedPlusPokemon GetEnemyMon() {
            return CreateEnemyMon(Sym["wEnemyMon"]);
        }

        // to-do add getenemymon code to check dvs and other stuff
        // private RedPlusPokemon CreatePartyStruct(CpuAddress address) {
        //     RedPlusSpecies species = Species[CpuRead(address)];
        // }

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

        public override CpuAddress Execute(Action[] actions, params (byte, byte, byte)[] itemPickups) {
            bool run = true;
            CpuAddress ret = new CpuAddress(0);

            foreach(Action action in actions) {
                // Thread.Sleep(100);
                if(action.IsDpad()) {
                    Joypad joypad = (Joypad) ((byte) action << 4);
                    if (run) joypad = joypad | Joypad.B;
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

        public override void Inject(Joypad joypad) {
            CpuWrite(0xFFF8, (byte) joypad);
        }

        public bool Yoloball(bool slot2 = false) {
            AdvanceWithJoypadToAddress(Joypad.B, Sym["ManualTextScroll"]);

            int waitFrames = 0;
            for(int i = 0; i < waitFrames; i++) {
                AdvanceToAddress(Sym["_Joypad"]);
                AdvanceFrame();
            }

            Inject(Joypad.A);
            AdvanceFrame(Joypad.A);
            AdvanceToAddress(Sym["PlayCry"]);
            if (slot2) {
                Press(Joypad.Down | Joypad.A, Joypad.Down | Joypad.Right | Joypad.A);
            } else {
                Press(Joypad.Down | Joypad.A, Joypad.Up | Joypad.A);
            }
            return AdvanceWithJoypadToAddress(Joypad.A, Sym["BallAnyway.captured"], Sym["BallAnyway.failedToCapture"]) == Sym["BallAnyway.captured"];
        }

        public bool SelectBall(bool slot2 = false) {
            AdvanceWithJoypadToAddress(Joypad.B, Sym["ManualTextScroll"]);
            Inject(Joypad.A);
            AdvanceFrame(Joypad.A);
            AdvanceToAddress(Sym["PlayCry"]);
            if (slot2) {
                Press(Joypad.Down | Joypad.A, Joypad.Down | Joypad.Select, Joypad.A);
            } else {
                Press(Joypad.Down | Joypad.A, Joypad.Select, Joypad.A);
            }
            return AdvanceWithJoypadToAddress(Joypad.A, Sym["BallAnyway.captured"], Sym["BallAnyway.failedToCapture"]) == Sym["BallAnyway.captured"];
        }

        public void gfSkip() {
            Press(Joypad.Start);
        }

        public void UpBSel() {
            Press(Joypad.Up | Joypad.B | Joypad.Select, Joypad.Up | Joypad.B | Joypad.Select);
        }

        public void Hop1() {
            AdvanceToAddress("AnimateIntroNidorino");
            AdvanceToAddress("CheckForUserInterruption");
            Press(Joypad.A);
        }

    }
}
