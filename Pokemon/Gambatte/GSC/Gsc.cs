using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class GscCharmap : Charmap {

        public GscCharmap() : base(0x80, "A B C D E F G H I J K L M N O P " +
                                         "Q R S T U V W X Y Z ( ) : ; [ ] " +
                                         "a b c d e f g h i j k l m n o p " +
                                         "q r s t u v w x y z _ _ _ _ _ _ " +
                                         "_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ " +
                                         "_ _ _ _ _ _ _ _ _ _ _ _ _ _ _ _ " +
                                         "' _ _ - _ _ ? ! . & _ _ _ _ _ M " +
                                         "_ * . / , F 0 1 2 3 4 5 6 7 8 9 ") { }
    }

    public enum GscType : byte {

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
        Curse = 19,
        Fire,
        Water,
        Grass,
        Electric,
        Psychic,
        Ice,
        Dragon,
        Dark,
    }

    public enum GscGenderRatio : byte {

        F0 = 0x00,
        F12_5 = 0x1F,
        F25 = 0x3F,
        F50 = 0x7F,
        F75 = 0xBF,
        F100 = 0xFE,
        Unknown = 0xFF,
    }

    public class GscSpecies {

        public Gsc Game;
        public string Name;
        public byte IndexNumber;
        public byte PokedexNumber;
        public byte BaseHP;
        public byte BaseAttack;
        public byte BaseDefense;
        public byte BaseSpeed;
        public byte BaseSpecialAttack;
        public byte BaseSpecialDefense;
        public GscType Type1;
        public GscType Type2;
        public byte CatchRate;
        public byte BaseExpYield;
        public GscGenderRatio GenderRatio;

        public GscSpecies(Gsc game, string name, byte pokedexNumber, byte[] data) {
            (Game, Name, PokedexNumber) = (game, name, pokedexNumber);
            IndexNumber = data[0];
            BaseHP = data[1];
            BaseAttack = data[2];
            BaseDefense = data[3];
            BaseSpeed = data[4];
            BaseSpecialAttack = data[5];
            BaseSpecialDefense = data[6];
            Type1 = (GscType) data[7];
            Type2 = (GscType) data[8];
            CatchRate = data[9];
            BaseExpYield = data[10];
            GenderRatio = (GscGenderRatio) data[13];
        }

        public static implicit operator byte(GscSpecies species) { return species.IndexNumber; }
    }

    public enum GscMoveEffect : byte {

        NormalHit,
        Sleep,
        PoisonHit,
        LeechHit,
        BurnHit,
        FreezeHit,
        ParalyzeHit,
        Selfdestruct,
        DreamEater,
        MirrorMove,
        AttackUp,
        DefenseUp,
        SpeedUp,
        SpAtkUp,
        SpDefUp,
        AccuracyUp,
        EvasionUp,
        AlwaysHit,
        AttackDown,
        DefenseDown,
        SpeedDown,
        SpAtkDown,
        SpDefDown,
        AccuracyDown,
        EvasionDown,
        ResetStats,
        Bide,
        Rampage,
        ForceSwitch,
        MultiHit,
        Conversion,
        FlinchHit,
        Heal,
        Toxic,
        PayDay,
        LightScreen,
        TriAttack,
        Unused25,
        Ohko,
        RazorWind,
        SuperFang,
        StaticDamage,
        TrapTarget,
        Unused2b,
        DoubleHit,
        JumpKick,
        Mist,
        FocusEnergy,
        RecoilHit,
        Confuse,
        AttackUp2,
        DefenseUp2,
        SpeedUp2,
        SpAtkUp2,
        SpDefUp2,
        AccuracyUp2,
        EvasionUp2,
        Transform,
        AttackDown2,
        DefenseDown2,
        SpeedDown2,
        SpAtkDown2,
        SpDefDown2,
        AccuracyDown2,
        EvasionDown2,
        Reflect,
        Poison,
        Paralyze,
        AttackDownHit,
        DefenseDownHit,
        SpeedDownHit,
        SpAtkDownHit,
        SpDefDownHit,
        AccuracyDownHit,
        EvasionDownHit,
        SkyAttack,
        ConfuseHit,
        PoisonMultiHit,
        Unused4e,
        Substitute,
        HyperBeam,
        Rage,
        Mimic,
        Metronome,
        LeechSeed,
        Splash,
        Disable,
        LevelDamage,
        Psywave,
        Counter,
        Encore,
        PainSplit,
        Snore,
        Conversion2,
        LockOn,
        Sketch,
        DefrostOpponent,
        SleepTalk,
        DestinyBond,
        Reversal,
        Spite,
        FalseSwipe,
        HealBell,
        PriorityHit,
        TripleKick,
        Thief,
        MeanLook,
        Nightmare,
        FlameWheel,
        Curse,
        Unused6e,
        Protect,
        Spikes,
        Foresight,
        PerishSong,
        Sandstorm,
        Endure,
        Rollout,
        Swagger,
        FuryCutter,
        Attract,
        Return,
        Present,
        Frustration,
        Safeguard,
        SacredFire,
        Magnitude,
        BatonPass,
        Pursuit,
        RapidSpin,
        Unused82,
        Unused83,
        MorningSun,
        Synthesis,
        Moonlight,
        HiddenPower,
        RainDance,
        SunnyDay,
        DefenseUpHit,
        AttackUpHit,
        AllUpHit,
        FakeOut,
        BellyDrum,
        PsychUp,
        MirrorCoat,
        SkullBash,
        Twister,
        Earthquake,
        FutureSight,
        Gust,
        Stomp,
        Solarbeam,
        Thunder,
        Teleport,
        BeatUp,
        Fly,
        DefenseCurl,
    }

    public enum GscGender {

        Male,
        Female,
        Genderless,
    }

    public class GscMove {

        public Gsc Game;
        public string Name;
        public byte Id;
        public byte Animation;
        public GscMoveEffect Effect;
        public byte Power;
        public GscType Type;
        public byte Accuracy;
        public byte PP;
        public byte EffectChance;

        public GscMove(Gsc game, string name, byte[] data) {
            Game = game;
            Name = name;
            Id = data[0];
            Animation = data[0];
            Effect = (GscMoveEffect) data[1];
            Power = data[2];
            Type = (GscType) data[3];
            Accuracy = data[4];
            PP = data[5];
            EffectChance = data[6];
        }

        public static implicit operator byte(GscMove move) { return move.Id; }
    }

    public class GscTileset {

        public Gsc Game;
        public byte Id;
        public RomAddress GFX;
        public RomAddress Meta;
        public RomAddress Coll;
        public ushort Anim;
        public ushort PalMap;

        public byte Bank {
            get { return GFX.Bank; }
        }

        public GscTileset(Gsc game, byte id, byte[] data) {
            Game = game;
            Id = id;
            GFX = new RomAddress(data[0], data.ReadWord(1));
            Meta = new RomAddress(data[3], data.ReadWord(4));
            Coll = new RomAddress(data[6], data.ReadWord(7));
            Anim = data.ReadWord(9);
            PalMap = data.ReadWord(13);
        }

        public static implicit operator byte(GscTileset tileset) { return tileset.Id; }
    }

    public class GscPokemon {

        public GscSpecies Species;
        public byte Level;
        public DVs DVs;

        public ushort HP {
            get { return CalcStat(DVs.HP, Species.BaseHP, 0, (byte) (Level + 10)); }
        }

        public ushort Attack {
            get { return CalcStat(DVs.Attack, Species.BaseAttack, 0, 5); }
        }

        public ushort Defense {
            get { return CalcStat(DVs.Defense, Species.BaseDefense, 0, 5); }
        }

        public ushort Speed {
            get { return CalcStat(DVs.Speed, Species.BaseSpeed, 0, 5); }
        }

        public ushort SpecialAttack {
            get { return CalcStat(DVs.Special, Species.BaseSpecialAttack, 0, 5); }
        }

        public ushort SpecialDefense {
            get { return CalcStat(DVs.Special, Species.BaseSpecialDefense, 0, 5); }
        }

        public GscGender Gender {
            get {
                switch(Species.GenderRatio) {
                    case GscGenderRatio.Unknown: return GscGender.Genderless;
                    case GscGenderRatio.F0: return GscGender.Male;
                    case GscGenderRatio.F100: return GscGender.Female;
                    default: return (byte) Species.GenderRatio < (byte) ((DVs.Upper & 0xF0) | (DVs.Lower >> 4)) ? GscGender.Male : GscGender.Female;
                }
            }
        }

        public GscType HPType {
            get {
                GscType type = (GscType) ((DVs.Defense & 3) + ((DVs.Attack & 3) << 2));
                if(type == GscType.Normal) type++;
                if(type == GscType.Bird) type++;
                if(type > GscType.Steel) type += 0xC;
                return type;
            }
        }

        public byte HPPower {
            get { return (byte) ((((DVs.Attack & 8) | ((DVs.Defense & 8) >> 1) | ((DVs.Speed & 8) >> 2) | ((DVs.Special & 8) >> 3)) * 5 + (DVs.Special & 3)) / 2 + 31); }
        }

        public GscPokemon(GscSpecies species, byte level) : this(species, level, 0x9888) { }
        public GscPokemon(GscSpecies species, byte level, DVs dvs) => (Species, Level, DVs) = (species, level, dvs);

        private ushort CalcStat(byte dv, byte baseStat, uint statXp, byte constant) {
            return (ushort) ((2 * (dv + baseStat) + Math.Min((int) Math.Ceiling(Math.Sqrt(statXp)), 255) / 4) * Level / 100 + constant);
        }

        public override string ToString() {
            return string.Format("L{0} {1} DVs {2:X4}", Level, Species.Name, DVs.Value);
        }

        public static implicit operator GscSpecies(GscPokemon pokemon) { return pokemon.Species; }
    }

    public enum GscEnvironment : byte {

        Town,
        Route,
        Indoor,
        Cave,
        Enviroment5,
        Gate,
        Dungeon,
    }

    public enum GscLandmark : byte {

        SpecialMap,
        NewBarkTown,
        Route29,
        CherrygroveCity,
        Route30,
        Route31,
        VioletCity,
        SproutTower,
        Route32,
        RuinsOfAlph,
        UnionCave,
        Route33,
        AzaleaTown,
        SlowpokeWell,
        IlexForest,
        Route34,
        GoldenrodCity,
        RadioTower,
        Route35,
        NationalPark,
        Route36,
        Route37,
        EcruteakCity,
        TinTower,
        BurnedTower,
        Route38,
        Route39,
        OlivineCity,
        Lighthouse,
        BattleTower,
        Route40,
        WhirlIslands,
        Route41,
        CianwoodCity,
        Route42,
        MtMortar,
        MahoganyTown,
        Route43,
        LakeOfRage,
        Route44,
        IcePath,
        BlackthornCity,
        DragonsDen,
        Route45,
        DarkCave,
        Route46,
        SilverCave,
        PalletTown,
        Route1,
        ViridianCity,
        Route2,
        PewterCity,
        Route3,
        MtMoon,
        Route4,
        CeruleanCity,
        Route24,
        Route25,
        Route5,
        UndergroundPath,
        Route6,
        VermilionCity,
        DiglettsCave,
        Route7,
        Route8,
        Route9,
        RockTunnel,
        Route10,
        PowerPlant,
        LavenderTown,
        LavRadioTower,
        CeladonCity,
        SaffronCity,
        Route11,
        Route12,
        Route13,
        Route14,
        Route15,
        Route16,
        Route17,
        Route18,
        FuchsiaCity,
        Route19,
        Route20,
        SeafoamIslands,
        CinnabarIsland,
        Route21,
        Route22,
        VictoryRoad,
        Route23,
        IndigoPlateau,
        Route26,
        Route27,
        TohjoFalls,
        Route28,
        FastShip,
        GiftLocation = 0x7E,
        EventLocation = 0x7F,
    }

    public enum GscMusic : byte {

        None,
        Title,
        Route1,
        Route3,
        Route12,
        MagnetTrain,
        KantoGymLeaderBattle,
        KantoTrainerBattle,
        KantoWildBattle,
        PokemonCenter,
        HikerEncounter,
        LassEncounter,
        OfficerEncounter,
        Heal,
        LavenderTown,
        Route2,
        MtMoon,
        ShowMeAround,
        GameCorner,
        Bicycle,
        HallOfFame,
        ViridianCity,
        CeladonCity,
        TrainerVictory,
        WildVictory,
        GymVictory,
        MtMoonSquare,
        Gym,
        PalletTown,
        PokemonTalk,
        ProfOak,
        RivalEncounter,
        RivalAfter,
        Surf,
        Evolution,
        NationalPark,
        Credits,
        AzaleaTown,
        CherrygroveCity,
        KimonoEncounter,
        UnionCave,
        JohtoWildBattle,
        JohtoTrainerBattle,
        Route30,
        EcruteakCity,
        VioletCity,
        JohtoGymLeaderBattle,
        ChampionBattle,
        RivalBattle,
        RocketBattle,
        ProfElm,
        DarkCave,
        Route29,
        Route36,
        SsAqua,
        YoungsterEncounter,
        BeautyEncounter,
        RocketEncounter,
        PokemaniacEncounter,
        SageEncounter,
        NewBarkTown,
        GoldenrodCity,
        VermilionCity,
        PokemonChannel,
        PokeFluteChannel,
        TinTower,
        SproutTower,
        BurnedTower,
        Lighthouse,
        LakeOfRage,
        IndigoPlateau,
        Route37,
        RocketHideout,
        DragonsDen,
        JohtoWildBattleNight,
        RuinsOfAlphRadio,
        Capture,
        Route26,
        Mom,
        VictoryRoad,
        PokemonLullaby,
        PokemonMarch,
        GsOpening,
        GsOpening2,
        MainMenu,
        RuinsOfAlphInterior,
        RocketOverture,
        DancingHall,
        BugCatchingContestRanking,
        BugCatchingContest,
        LakeOfRageRocketRadio,
        Printer,
        PostCredits,
        Clair,
        MobileAdapterMenu,
        MobileAdapter,
        BuenasPassword,
        MysticalmanEncounter,
        CrystalOpening,
        BattleTowerTheme,
        SuicuneBattle,
        BattleTowerLobby,
        MobileCenter,
    }

    public enum GscMapPalette : byte {

        Auto,
        Day,
        Nite,
        Morn,
        Dark,
    }

    public enum GscFishGroup : byte {

        None,
        Shore,
        Ocean,
        Lake,
        Pond,
        Dratini,
        QwilfishSwarm,
        RemoraidSwarm,
        Gyarados,
        Dratini2,
        WhirlIslands,
        Qwilfish,
        Remoraid,
        QwilfishNoSwarm,
    }

    public class GscConnection {

        public GscMap Map;
        public byte DestMapGroup;
        public byte DestMapNumber;
        public ushort Source;
        public ushort Destination;
        public byte Length;
        public byte MapWidth;
        public byte YAlignment;
        public byte XAlignment;
        public ushort Window;

        public GscMap DestinationMap {
            get { return Map.Game.Maps[(DestMapGroup << 8) | DestMapNumber]; }
        }

        public GscConnection(GscMap map, byte[] data) {
            Map = map;
            DestMapGroup = data[0];
            DestMapNumber = data[1];
            Source = data.ReadWord(2);
            Destination = data.ReadWord(4);
            Length = data[6];
            MapWidth = data[7];
            YAlignment = data[8];
            XAlignment = data[9];
            Window = data.ReadWord(10);
        }
    }

    public class GscWarp {

        public GscMap Map;
        public byte X;
        public byte Y;
        public byte MapGroup;
        public byte MapNumber;
        public byte Index;
        public byte DestinationIndex;

        public bool Allowed;

        public ushort MapId {
            get { return (ushort) ((MapGroup << 8) | MapNumber); }
        }

        public GscMap DestinationMap {
            get { return Map.Game.Maps[MapId]; }
        }

        public GscWarp DestinationWarp {
            get { return Map.Warps[DestinationIndex]; }
        }

        public GscWarp(GscMap map, byte index, byte[] data) {
            Map = map;
            Y = data[0];
            X = data[1];
            Index = index;
            DestinationIndex = data[2];
            MapGroup = data[3];
            MapNumber = data[4];
            Allowed = false;
        }
    }

    public class GscCoordEvent {

        public GscMap Map;
        public byte X;
        public byte Y;
        public byte SceneId;
        public ushort ScriptPointer;

        public GscCoordEvent(GscMap map, byte[] data) {
            SceneId = data[0];
            Y = data[1];
            X = data[2];
            ScriptPointer = data.ReadWord(4);
        }
    }

    public enum GscBGEventType : byte {

        Read,
        Up,
        Down,
        Right,
        Left,
        IfSet,
        IfNotSet,
        Item,
        Copy,
    }

    public class GscBGEvent {

        public GscMap Map;
        public byte X;
        public byte Y;
        public GscBGEventType Function;
        public ushort ScriptPointer;

        public GscBGEvent(GscMap map, byte[] data) {
            Y = data[0];
            X = data[1];
            Function = (GscBGEventType) data[2];
            ScriptPointer = data.ReadWord(3);
        }
    }

    public enum GscSpriteMoveData : byte {

        SpriteMoveData_00,
        Still,
        Wander,
        SpinrandomSlow,
        WalkUpDown,
        WalkLeftRight,
        StandingDown,
        StandingUp,
        StandingLeft,
        StandingRight,
        SpinrandomFast,
        Player,
        SpriteMoveData_0C,
        SpriteMoveData_0D,
        SpriteMoveData_0E,
        SpriteMoveData_0F,
        SpriteMoveData_10,
        SpriteMoveData_11,
        SpriteMoveData_12,
        Following,
        Scripted,
        Bigdollsym,
        Pokemon,
        Sudowoodo,
        SmashableRock,
        StrengthBoulder,
        Follownotexact,
        Shadow,
        Emote,
        Screenshake,
        Spincounterclockwise,
        Spinclockwise,
        Bigdollasym,
        Bigdoll,
        Boulderdust,
        Grass,
        SwimWander,
    }

    public enum GscPalNpc : byte {

        Red = 8,
        Blue,
        Green,
        Brown,
        Pink,
        Silver,
        Tree,
        Rock,
    }

    public enum GscSpriteType : byte {

        Script,
        Itemball,
        Trainer,
        SpriteType_3,
        SpriteType_4,
        SpriteType_5,
        SpriteType_6,
    }

    public enum GscSpriteId : byte {

        None,
        Chris,
        ChrisBike,
        GameboyKid,
        Silver,
        Oak,
        Red,
        Blue,
        Bill,
        Elder,
        Janine,
        Kurt,
        Mom,
        Blaine,
        RedsMom,
        Daisy,
        Elm,
        Will,
        Falkner,
        Whitney,
        Bugsy,
        Morty,
        Chuck,
        Jasmine,
        Pryce,
        Clair,
        Brock,
        Karen,
        Bruno,
        Misty,
        Lance,
        Surge,
        Erika,
        Koga,
        Sabrina,
        CooltrainerM,
        CooltrainerF,
        BugCatcher,
        Twin,
        Youngster,
        Lass,
        Teacher,
        Buena,
        SuperNerd,
        Rocker,
        PokefanM,
        PokefanF,
        Gramps,
        Granny,
        SwimmerGuy,
        SwimmerGirl,
        BigSnorlax,
        SurfingPikachu,
        Rocket,
        RocketGirl,
        Nurse,
        LinkReceptionist,
        Clerk,
        Fisher,
        FishingGuru,
        Scientist,
        KimonoGirl,
        Sage,
        UnusedGuy,
        Gentleman,
        BlackBelt,
        Receptionist,
        Officer,
        Cal,
        Slowpoke,
        Captain,
        BigLapras,
        GymGuy,
        Sailor,
        Biker,
        Pharmacist,
        Monster,
        Fairy,
        Bird,
        Dragon,
        BigOnix,
        N64,
        Sudowoodo,
        Surf,
        PokeBall,
        Pokedex,
        Paper,
        VirtualBoy,
        OldLinkReceptionist,
        Rock,
        Boulder,
        Snes,
        Famicom,
        FruitTree,
        GoldTrophy,
        SilverTrophy,
        Kris,
        KrisBike,
        KurtOutside,
        Suicune,
        Entei,
        Raikou,
        StandingYoungster,
    }

    public class GscSprite {

        public GscMap Map;
        public byte Id;
        public byte X;
        public byte Y;
        public GscSpriteId Sprite;
        public GscSpriteMoveData MovementFunction;
        public byte MovementRadius;
        public byte H1;
        public byte H2;
        public byte ColorAndFunction;
        public byte SightRange;
        public ushort ScriptPointer;
        public ushort EventFlag;

        public byte MovementRadiusY {
            get { return (byte) (MovementRadius >> 4); }
        }

        public byte MovementRadiusX {
            get { return (byte) (MovementRadius & 0xF); }
        }

        public GscPalNpc Color {
            get { return (GscPalNpc) (ColorAndFunction >> 4); }
        }

        public GscSpriteType Function {
            get { return (GscSpriteType) (ColorAndFunction & 0xF); }
        }

        public GscSprite(GscMap map, byte id, byte[] data) {
            Map = map;
            Id = id;
            Sprite = (GscSpriteId) data[0];
            Y = (byte) (data[1] - 4);
            X = (byte) (data[2] - 4);
            MovementFunction = (GscSpriteMoveData) data[3];
            MovementRadius = data[4];
            H1 = data[5];
            H2 = data[6];
            ColorAndFunction = data[7];
            SightRange = data[8];
            ScriptPointer = data.ReadWord(9);
            ScriptPointer = data.ReadWord(11);
        }
    }

    public class GscTile {

        public GscMap Map;
        public byte X;
        public byte Y;
        public bool Solid;
        public bool DownHop;
        public bool LeftHop;
        public bool RightHop;
        public bool CanMoveDown;
        public bool CanMoveUp;
        public bool CanMoveLeft;
        public bool CanMoveRight;
        public Dictionary<int, List<GscEdge>> Edges;

        public bool IsUnallowedWarp {
            get {
                GscWarp warp = Map.Warps[X, Y];
                return warp == null ? false : !warp.Allowed;
            }
        }

        public GscTile Up {
            get { return Y == 0 ? null : Map.Tiles[X, Y - 1]; }
        }

        public GscTile Down {
            get { return Y == Map.Height * 2 - 1 ? null : Map.Tiles[X, Y + 1]; }
        }

        public GscTile Left {
            get { return X == 0 ? null : Map.Tiles[X - 1, Y]; }
        }

        public GscTile Right {
            get { return X == Map.Width * 2 - 1 ? null : Map.Tiles[X + 1, Y]; }
        }

        public GscTile(GscMap map, byte x, byte y, bool solid, bool downHop, bool leftHop, bool rightHop) => (Map, X, Y, Solid, DownHop, LeftHop, RightHop, Edges) = (map, x, y, solid, downHop, leftHop, rightHop, new Dictionary<int, List<GscEdge>>());

        public void AddEdge(int edgeSet, GscEdge edge) {
            if(!Edges.ContainsKey(edgeSet)) {
                Edges[edgeSet] = new List<GscEdge>();
            }
            Edges[edgeSet].Add(edge);
            //Edges[edgeSet].Sort();
        }

        public GscEdge GetEdge(int edgeSet, Action action) {
            if(!Edges.ContainsKey(edgeSet)) {
                return null;
            }

            foreach(GscEdge edge in Edges[edgeSet]) {
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
            GscEdge edge = GetEdge(edgeSet, action);
            if(edge == null) {
                return;
            }

            Edges[edgeSet].Remove(edge);
            //Edges[edgeSet].Sort();
        }

        public override string ToString() {
            return string.Format("{0}#{1},{2}", Map.Id, X, Y);
        }
    }

    public class GscEdge : IComparable<GscEdge> {

        public Action Action;
        public GscTile NextTile;
        public int NextEdgeset;
        public int Cost;

        public GscEdge(Action action, GscTile nextTile, int nextEdgeset, int cost) => (Action, NextTile, NextEdgeset, Cost) = (action, nextTile, nextEdgeset, cost);

        public int CompareTo(GscEdge other) {
            return Cost - other.Cost;
        }

        public override string ToString() {
            return Enum.GetName(typeof(Action), Action) + " cost: " + Cost;
        }
    }

    public class GscMap {

        public Gsc Game;
        public string Name;
        public ushort Id;
        public RomAddress Attribute;
        public GscTileset Tileset;
        public GscEnvironment Environment;
        public GscLandmark Location;
        public GscMusic Music;
        public bool PhoneService;
        public GscMapPalette TimeOfDay;
        public GscFishGroup FishingGroup;
        public byte BorderBlock;
        public byte Height;
        public byte Width;
        public RomAddress Blocks;
        public RomAddress Scripts;
        public RomAddress Events;
        public byte ConnectionFlags;
        public GscConnection[] Connections;
        public DataArray<GscWarp> Warps;
        public DataArray<GscCoordEvent> CoordEvents;
        public DataArray<GscBGEvent> BGEvents;
        public DataArray<GscSprite> Sprites;
        public DataArray<GscTile> Tiles;

        public GscConnection EastConnection {
            get { return Connections[0]; }
        }

        public GscConnection WestConnection {
            get { return Connections[1]; }
        }

        public GscConnection SouthConnection {
            get { return Connections[2]; }
        }

        public GscConnection NorthConnection {
            get { return Connections[3]; }
        }

        public GscMap(Gsc game, ushort id, byte[] data) {
            Game = game;
            Id = id;
            Attribute = new RomAddress(data[0], data.ReadWord(3));
            Tileset = game.Tilesets[data[1]];
            Environment = (GscEnvironment) data[2];
            Location = (GscLandmark) data[5];
            Music = (GscMusic) data[6];
            PhoneService = (data[7] & 0xF0) != 0;
            TimeOfDay = (GscMapPalette) (data[7] & 0xF);
            FishingGroup = (GscFishGroup) data[8];

            Name = game.Sym.GetAddressName(Attribute);
            Name = Name.Substring(0, Name.LastIndexOf('_'));

            BorderBlock = game.Rom[Attribute + 0];
            Height = game.Rom[Attribute + 1];
            Width = game.Rom[Attribute + 2];
            Blocks = new RomAddress(game.Rom[Attribute + 3], game.Rom.ReadWord(Attribute + 4));
            Scripts = new RomAddress(game.Rom[Attribute + 6], game.Rom.ReadWord(Attribute + 7));
            Events = new RomAddress(game.Rom[Attribute + 6], game.Rom.ReadWord(Attribute + 9));
            ConnectionFlags = game.Rom[Attribute + 11];
            Connections = new GscConnection[4];
            const byte connectionSize = 12;
            for(int i = 3; i >= 0; i--) {
                if(((ConnectionFlags >> i) & 1) == 1) {
                    Connections[i] = new GscConnection(this, game.Rom.Subarray(Attribute + 12 + (3 - i) * connectionSize, connectionSize));
                }
            }

            RomAddress eventPointer = Events;
            eventPointer += 2; // filler

            Warps = new DataArray<GscWarp>(Width * 2, Height * 2);
            byte numWarps = game.Rom[eventPointer];
            eventPointer += 1;
            const byte warpSize = 5;
            for(byte index = 0; index < numWarps; index++) {
                GscWarp warp = new GscWarp(this, index, game.Rom.Subarray(eventPointer, warpSize));
                eventPointer += warpSize;
                Warps[warp.X, warp.Y] = warp;
            }

            CoordEvents = new DataArray<GscCoordEvent>(Width * 2, Height * 2);
            byte numCoordEvents = game.Rom[eventPointer];
            eventPointer += 1;
            const byte coordEventSize = 8;
            for(byte i = 0; i < numCoordEvents; i++) {
                GscCoordEvent coordEvent = new GscCoordEvent(this, game.Rom.Subarray(eventPointer, coordEventSize));
                eventPointer += coordEventSize;
                CoordEvents[coordEvent.X, coordEvent.Y] = coordEvent;
            }

            BGEvents = new DataArray<GscBGEvent>(Width * 2, Height * 2);
            byte numBGEvents = game.Rom[eventPointer];
            eventPointer += 1;
            const byte bgEventSize = 5;
            for(byte i = 0; i < numBGEvents; i++) {
                GscBGEvent bgEvent = new GscBGEvent(this, game.Rom.Subarray(eventPointer, bgEventSize));
                eventPointer += bgEventSize;
                BGEvents[bgEvent.X, bgEvent.Y] = bgEvent;
            }

            Sprites = new DataArray<GscSprite>(Width * 2, Height * 2);
            byte numSprites = game.Rom[eventPointer];
            eventPointer += 1;
            const byte spriteSize = 13;
            for(byte i = 0; i < numSprites; i++) {
                GscSprite sprite = new GscSprite(this, i, game.Rom.Subarray(eventPointer, spriteSize));
                eventPointer += spriteSize;
                if(sprite.X >= Width * 2 || sprite.Y >= Height * 2) continue; // Goldenrod center npc is offscreen
                Sprites[sprite.X, sprite.Y] = sprite;
            }

            Tiles = new DataArray<GscTile>(Width * 2, Height * 2);

            for(byte y = 0; y < Height * 2; y++) {
                for(byte x = 0; x < Width * 2; x++) {
                    byte yBlock = (byte) (y / 2);
                    byte xBlock = (byte) (x / 2);
                    byte offset = (byte) ((x & 1) + (y & 1) * 2);
                    byte block = game.Rom[Blocks + xBlock + yBlock * Width];
                    byte tile = game.Rom[Tileset.Coll + block * 4 + offset];
                    byte collision = game.Rom[game.Sym["TileCollisionTable"].Rom() + tile];
                    Tiles[x, y] = new GscTile(this, x, y, collision != 0, tile == 0xA3, tile == 0xA1, tile == 0xA0);
                }
            }

            for(byte y = 0; y < Height * 2; y++) {
                for(byte x = 0; x < Width * 2; x++) {
                    GscTile tile = Tiles[x,y];
                    if(tile.Up != null) tile.CanMoveUp = !tile.Up.Solid;
                    if(tile.Left != null) tile.CanMoveLeft = !tile.Left.Solid;
                    if(tile.Right != null) tile.CanMoveRight = !tile.Right.Solid;
                    if(tile.Down != null) tile.CanMoveDown = !tile.Down.Solid;
                }
            }
        }

        public GscTile this[byte x, byte y] {
            get { return Tiles[x, y]; }
            set { Tiles[x, y] = value; }
        }

        public GscTile this[int x, int y] {
            get { return Tiles[x, y]; }
            set { Tiles[x, y] = value; }
        }
    }

    internal class GscData {

        public GscCharmap Charmap;
        public DataMap<GscSpecies> Species;
        public DataMap<Item> Items;
        public DataMap<GscMove> Moves;
        public DataMap<GscTileset> Tilesets;
        public DataMap<GscMap> Maps;

        public GscData() {
            Charmap = new GscCharmap();
            Species = new DataMap<GscSpecies>();
            Items = new DataMap<Item>();
            Moves = new DataMap<GscMove>();
            Tilesets = new DataMap<GscTileset>();
            Maps = new DataMap<GscMap>();

            Species.Callbacks = new Dictionary<Type, Func<GscSpecies, object, bool>>() {
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

            Moves.Callbacks = new Dictionary<Type, Func<GscMove, object, bool>>() {
                { typeof(string), (move, name) => { return move.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (move, id) => { return move.Id == (int) id; } },
                { typeof(uint), (move, id) => { return move.Id == (uint) id; } },
                { typeof(byte), (move, id) => { return move.Id == (byte) id; } },
            };

            Tilesets.Callbacks = new Dictionary<Type, Func<GscTileset, object, bool>>() {
                { typeof(int), (tileset, id) => { return tileset.Id == (int) id; } },
                { typeof(uint), (tileset, id) => { return tileset.Id == (uint) id; } },
                { typeof(byte), (tileset, id) => { return tileset.Id == (byte) id; } },
            };

            Maps.Callbacks = new Dictionary<Type, Func<GscMap, object, bool>>() {
                { typeof(string), (map, name) => { return map.Name.ToLower() == ((string) name).ToLower(); } },
                { typeof(int), (map, id) => { return map.Id == (int) id; } },
                { typeof(uint), (map, id) => { return map.Id == (uint) id; } },
                { typeof(ushort), (map, id) => { return map.Id == (ushort) id; } },
            };
        }
    }

    public class Gsc : GameBoy {

        private static GscData Data;

        public GscCharmap Charmap {
            get { return Data.Charmap; }
        }

        public DataMap<GscSpecies> Species {
            get { return Data.Species; }
        }

        public DataMap<Item> Items {
            get { return Data.Items; }
        }

        public DataMap<GscMove> Moves {
            get { return Data.Moves; }
        }

        public DataMap<GscTileset> Tilesets {
            get { return Data.Tilesets; }
        }

        public DataMap<GscMap> Maps {
            get { return Data.Maps; }
        }


        public Gsc(string rom, string saveName, SpeedupFlags speedupFlags = SpeedupFlags.None) : base(rom, saveName, "roms/gbc_bios.bin", speedupFlags) {
            if(Data == null) {
                Data = new GscData();
                LoadSpecies();
                LoadItems();
                LoadMoves();
                LoadTilesets();
                LoadMaps();
            }
        }

        public override void Press(params Joypad[] joypads) {
            foreach(Joypad joypad in joypads) {
                AdvanceToAddress(Sym["GetJoypad"]);
                InjectMenu(joypad);
                AdvanceFrame();
            }
        }

        public override CpuAddress Execute(Action[] actions, params (byte, byte, byte)[] itemPickups) {
            CpuAddress ret = new CpuAddress(0);

            foreach(Action action in actions) {
                if(action.IsDpad()) {
                    Joypad joypad;
                    if(action == Action.LeftA || action == Action.RightA || action == Action.UpA || action == Action.DownA) {
                        joypad = (Joypad) (1 | ((uint) action >> 4));
                    } else {
                        joypad = (Joypad) ((uint) action << 4);
                    }
                    Inject(joypad);
                    ret = AdvanceWithJoypadToAddress(joypad, "CountStep", "RandomEncounter.ok", "PrintLetterDelay.checkjoypad", "DoPlayerMovement.BumpSound");

                    if(ret == Sym["CountStep"]) {
                        ret = AdvanceWithJoypadToAddress(joypad, "PrintLetterDelay.checkjoypad", "OWPlayerInput", "RandomEncounter.ok");
                    }

                    if(ret != Sym["OWPlayerInput"]) return ret;
                } else if(action == Action.StartB) {
                    Inject(Joypad.Start);
                    AdvanceFrame(Joypad.Start);
                    AdvanceToAddress("GetJoypad");
                    InjectMenu(Joypad.B);
                    ret = AdvanceToAddress("OWPlayerInput", "PrintLetterDelay.checkjoypad");
                } else if(action == Action.Select) {
                    Inject(Joypad.Select);
                    AdvanceFrame(Joypad.Select);
                    ret = AdvanceToAddress("OWPlayerInput", "PrintLetterDelay.checkjoypad");
                }
            }

            return ret;
        }

        public override byte ReadHRA() {
            return CpuRead(0xFFE1);
        }

        public override byte ReadHRS() {
            return CpuRead(0xFFE2);
        }

        public GscTile GetCurTile() {
            return Maps[(CpuRead("wMapGroup") << 8) | CpuRead("wMapNumber")][CpuRead("wXCoord"), CpuRead("wYCoord")];
        }

        public GscPokemon GetEnemyMon() {
            return new GscPokemon(Species[CpuRead("wEnemyMon")], CpuRead("wEnemyMonLevel"), CpuReadWord("wEnemyMonDVs"));
        }

        private void LoadSpecies() {
            int nameLength = 10;

            RomAddress namesStart = Sym["PokemonNames"];
            RomAddress baseStatsStart = Sym["BaseData"];
            int baseStatsSize = Sym["wCurBaseDataEnd"].Rom() - Sym["wCurBaseData"].Rom();
            int numBaseStats = (namesStart - baseStatsStart) / baseStatsSize;

            byte[] pokedex = new byte[numBaseStats];
            for(int i = 0; i < numBaseStats; i++) {
                pokedex[i] = Rom[Sym["NewPokedexOrder"].Rom() + i];
            }

            for(int i = 0; i < numBaseStats; i++) {
                byte[] header = Rom.Subarray(baseStatsStart + i * baseStatsSize, baseStatsSize);
                string name = Charmap.Decode(Rom.Subarray(namesStart + i * nameLength, nameLength));
                Species.Add(new GscSpecies(this, name, (byte) (Array.IndexOf(pokedex, header[0]) + 1), header));
            }
        }

        private void LoadItems() {
            int namePointer = Sym["ItemNames"].Rom() - 1;
            for(int i = 0; i < 256; i++) {
                Items.Add(new Item((byte) (i + 1), Charmap.Decode(Rom.ReadUntil(GscCharmap.Terminator, ref namePointer))));
            }
        }

        private void LoadMoves() {
            RomAddress movesStart = Sym["Moves"];
            int moveSize = 7;
            int numMoves = (Sym["EvolvePokemon"] - movesStart) / moveSize;

            int namePointer = Sym["MoveNames"].Rom();
            for(int i = 0; i < numMoves; i++) {
                string name = Charmap.Decode(Rom.ReadUntil(GscCharmap.Terminator, ref namePointer));
                Moves.Add(new GscMove(this, name, Rom.Subarray(movesStart + i * moveSize, moveSize)));
            }
        }

        private void LoadTilesets() {
            RomAddress tilesetsStart = Sym["Tilesets"];
            int tilesetSize = Sym["wTilesetEnd"].Rom() - Sym["wTileset"].Rom();
            int numTilesets = (Sym["SmallFarFlagAction"].Rom() - tilesetsStart) / tilesetSize;

            for(byte i = 0; i < numTilesets; i++) {
                Tilesets.Add(new GscTileset(this, i, Rom.Subarray(tilesetsStart + i * tilesetSize, tilesetSize)));
            }
        }

        private void LoadMaps() {
            int numMapGroups = (Sym["MapGroup_Olivine"].Rom() - Sym["MapGroupPointers"].Rom()) / 2;
            byte bank = Sym["MapGroupPointers"].Bank;
            byte mapSize = 9;

            for(byte mapGroup = 0; mapGroup < numMapGroups; mapGroup++) {
                RomAddress mapGroupPointer = new RomAddress(bank, Rom.ReadWord(Sym["MapGroupPointers"].Rom() + mapGroup * 2));
                RomAddress nextMapGroupPointer = mapGroup == (numMapGroups - 1) ? Sym["NewBarkTown_MapAttributes"].Rom() : new RomAddress(bank, Rom.ReadWord(Sym["MapGroupPointers"].Rom() + (mapGroup + 1) * 2));
                int numMaps = (nextMapGroupPointer - mapGroupPointer) / mapSize;

                for(byte mapNumber = 0; mapNumber < numMaps; mapNumber++) {
                    Maps.Add(new GscMap(this, (ushort) (((mapGroup + 1) << 8) | mapNumber + 1), Rom.Subarray(mapGroupPointer + mapNumber * mapSize, mapSize)));
                }
            }
        }
    }
}
