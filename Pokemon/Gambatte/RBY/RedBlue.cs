using System;
using System.Collections.Generic;
using System.Text;

namespace Pokemon {

    public class RedBlue : Rby {

        public static readonly Dictionary<(string, byte), byte> wLastMapDestinations = new Dictionary<(string, byte), byte>() {
            { ("DiglettsCaveRoute2", 1), 13 },
            { ("Route11Gate1F", 2), 22 },
            { ("DiglettsCaveRoute2", 0), 13 },
            { ("Route11Gate1F", 3), 22 },
            { ("Route11Gate1F", 0), 22 },
            { ("Route11Gate1F", 1), 22 },
            { ("VermilionTradeHouse", 1), 5 },
            { ("VermilionTradeHouse", 0), 5 },
            { ("Museum1F", 1), 2 },
            { ("Museum1F", 0), 2 },
            { ("DiglettsCaveRoute11", 0), 22 },
            { ("Museum1F", 3), 2 },
            { ("DiglettsCaveRoute11", 1), 22 },
            { ("Museum1F", 2), 2 },
            { ("PewterNidoranHouse", 1), 2 },
            { ("PewterNidoranHouse", 0), 2 },
            { ("MtMoonB1F", 7), 15 },
            { ("CeruleanTrashedHouse", 0), 3 },
            { ("CeruleanTrashedHouse", 1), 3 },
            { ("CeruleanTrashedHouse", 2), 3 },
            { ("ViridianGym", 1), 1 },
            { ("ViridianGym", 0), 1 },
            { ("CeruleanPokecenter", 0), 3 },
            { ("FuchsiaMart", 0), 7 },
            { ("CeruleanPokecenter", 1), 3 },
            { ("FuchsiaMart", 1), 7 },
            { ("PokemonTower1F", 0), 4 },
            { ("PokemonTower1F", 1), 4 },
            { ("LavenderCuboneHouse", 1), 4 },
            { ("LavenderCuboneHouse", 0), 4 },
            { ("UndergroundPathRoute7Copy", 0), 18 },
            { ("UndergroundPathRoute7Copy", 1), 18 },
            { ("PewterSpeechHouse", 1), 2 },
            { ("PewterSpeechHouse", 0), 2 },
            { ("CinnabarPokecenter", 1), 8 },
            { ("SilphCo1F", 1), 10 },
            { ("CinnabarPokecenter", 0), 8 },
            { ("SilphCo1F", 0), 10 },
            { ("WardensHouse", 0), 7 },
            { ("WardensHouse", 1), 7 },
            { ("MrFujisHouse", 1), 4 },
            { ("MrFujisHouse", 0), 4 },
            { ("BikeShop", 1), 3 },
            { ("BikeShop", 0), 3 },
            { ("Route12SuperRodHouse", 1), 23 },
            { ("Route12SuperRodHouse", 0), 23 },
            { ("Route8Gate", 1), 19 },
            { ("CeladonMansion1F", 2), 6 },
            { ("Route8Gate", 0), 19 },
            { ("CeladonMansion1F", 1), 6 },
            { ("MrPsychicsHouse", 1), 10 },
            { ("Route8Gate", 3), 19 },
            { ("CeladonMansion1F", 0), 6 },
            { ("MrPsychicsHouse", 0), 10 },
            { ("Route8Gate", 2), 19 },
            { ("CinnabarMart", 1), 8 },
            { ("CinnabarMart", 0), 8 },
            { ("FuchsiaBillsGrandpasHouse", 1), 7 },
            { ("FuchsiaBillsGrandpasHouse", 0), 7 },
            { ("Route12Gate1F", 3), 23 },
            { ("Route12Gate1F", 2), 23 },
            { ("Route12Gate1F", 1), 23 },
            { ("Route12Gate1F", 0), 23 },
            { ("VermilionDock", 0), 5 },
            { ("PokemonMansion1F", 7), 8 },
            { ("VictoryRoad2F", 2), 34 },
            { ("PokemonMansion1F", 6), 8 },
            { ("VictoryRoad2F", 1), 34 },
            { ("PokemonMansion1F", 3), 8 },
            { ("PokemonMansion1F", 2), 8 },
            { ("PokemonMansion1F", 1), 8 },
            { ("PokemonMansion1F", 0), 8 },
            { ("CopycatsHouse1F", 1), 10 },
            { ("CopycatsHouse1F", 0), 10 },
            { ("PewterGym", 1), 2 },
            { ("PewterGym", 0), 2 },
            { ("CeruleanGym", 1), 3 },
            { ("CeruleanGym", 0), 3 },
            { ("FuchsiaGym", 0), 7 },
            { ("FuchsiaGym", 1), 7 },
            { ("CinnabarLab", 1), 8 },
            { ("CinnabarLab", 0), 8 },
            { ("PewterPokecenter", 0), 2 },
            { ("PewterPokecenter", 1), 2 },
            { ("Route5Gate", 3), 16 },
            { ("Route5Gate", 2), 16 },
            { ("Route5Gate", 1), 16 },
            { ("Route5Gate", 0), 16 },
            { ("PowerPlant", 1), 21 },
            { ("PowerPlant", 0), 21 },
            { ("PowerPlant", 2), 21 },
            { ("CeladonGym", 0), 6 },
            { ("CeladonGym", 1), 6 },
            { ("MtMoonPokecenter", 0), 15 },
            { ("MtMoonPokecenter", 1), 15 },
            { ("ViridianForestSouthGate", 3), 13 },
            { ("ViridianForestSouthGate", 2), 13 },
            { ("BillsHouse", 0), 36 },
            { ("VermilionPidgeyHouse", 1), 5 },
            { ("BillsHouse", 1), 36 },
            { ("VermilionPidgeyHouse", 0), 5 },
            { ("CeladonChiefHouse", 0), 6 },
            { ("CeladonHotel", 1), 6 },
            { ("CeladonChiefHouse", 1), 6 },
            { ("CeladonHotel", 0), 6 },
            { ("CinnabarGym", 1), 8 },
            { ("CinnabarGym", 0), 8 },
            { ("RockTunnelPokecenter", 1), 21 },
            { ("RockTunnelPokecenter", 0), 21 },
            { ("LavenderMart", 0), 4 },
            { ("LavenderMart", 1), 4 },
            { ("CeruleanCave1F", 1), 3 },
            { ("CeruleanCave1F", 0), 3 },
            { ("OaksLab", 0), 0 },
            { ("OaksLab", 1), 0 },
            { ("Daycare", 0), 16 },
            { ("Daycare", 1), 16 },
            { ("VermilionMart", 1), 5 },
            { ("VermilionMart", 0), 5 },
            { ("SaffronPidgeyHouse", 0), 10 },
            { ("SaffronPidgeyHouse", 1), 10 },
            { ("SeafoamIslands1F", 3), 31 },
            { ("SeafoamIslands1F", 2), 31 },
            { ("SeafoamIslands1F", 1), 31 },
            { ("SeafoamIslands1F", 0), 31 },
            { ("CeruleanTradeHouse", 1), 3 },
            { ("CeruleanTradeHouse", 0), 3 },
            { ("RedsHouse1F", 1), 0 },
            { ("Route2Gate", 2), 13 },
            { ("RedsHouse1F", 0), 0 },
            { ("Route2Gate", 3), 13 },
            { ("Route2Gate", 0), 13 },
            { ("Route2Gate", 1), 13 },
            { ("SaffronGym", 0), 10 },
            { ("SaffronGym", 1), 10 },
            { ("LavenderPokecenter", 0), 4 },
            { ("LavenderPokecenter", 1), 4 },
            { ("SilphCo11F", 2), 10 },
            { ("GameCornerPrizeRoom", 0), 6 },
            { ("MtMoon1F", 1), 15 },
            { ("GameCornerPrizeRoom", 1), 6 },
            { ("MtMoon1F", 0), 15 },
            { ("RockTunnel1F", 2), 21 },
            { ("RockTunnel1F", 3), 21 },
            { ("RockTunnel1F", 0), 21 },
            { ("RockTunnel1F", 1), 21 },
            { ("SafariZoneGate", 1), 7 },
            { ("SafariZoneGate", 0), 7 },
            { ("VictoryRoad1F", 1), 34 },
            { ("VictoryRoad1F", 0), 34 },
            { ("NameRatersHouse", 0), 4 },
            { ("NameRatersHouse", 1), 4 },
            { ("Route6Gate", 2), 17 },
            { ("VermilionGym", 0), 5 },
            { ("Route6Gate", 3), 17 },
            { ("VermilionGym", 1), 5 },
            { ("Route6Gate", 0), 17 },
            { ("Route6Gate", 1), 17 },
            { ("Route16FlyHouse", 1), 27 },
            { ("Route16FlyHouse", 0), 27 },
            { ("Route18Gate1F", 3), 29 },
            { ("Route18Gate1F", 2), 29 },
            { ("Route18Gate1F", 1), 29 },
            { ("Route18Gate1F", 0), 29 },
            { ("VermilionOldRodHouse", 0), 5 },
            { ("VermilionOldRodHouse", 1), 5 },
            { ("BluesHouse", 0), 0 },
            { ("FightingDojo", 1), 10 },
            { ("BluesHouse", 1), 0 },
            { ("FightingDojo", 0), 10 },
            { ("Route2TradeHouse", 0), 13 },
            { ("Route2TradeHouse", 1), 13 },
            { ("ViridianForestNorthGate", 0), 13 },
            { ("ViridianForestNorthGate", 1), 13 },
            { ("GameCorner", 1), 10 },
            { ("GameCorner", 0), 10 },
            { ("IndigoPlateauLobby", 0), 9 },
            { ("IndigoPlateauLobby", 1), 9 },
            { ("ViridianNicknameHouse", 0), 1 },
            { ("ViridianNicknameHouse", 1), 1 },
            { ("Route16Gate1F", 4), 27 },
            { ("SaffronMart", 1), 10 },
            { ("Route16Gate1F", 5), 27 },
            { ("SaffronMart", 0), 10 },
            { ("PokemonFanClub", 0), 1 },
            { ("Route16Gate1F", 6), 27 },
            { ("PokemonFanClub", 1), 1 },
            { ("Route16Gate1F", 7), 27 },
            { ("Route16Gate1F", 0), 27 },
            { ("Route16Gate1F", 1), 27 },
            { ("Route16Gate1F", 2), 27 },
            { ("Route16Gate1F", 3), 27 },
            { ("ViridianSchoolHouse", 1), 1 },
            { ("ViridianSchoolHouse", 0), 1 },
            { ("ViridianPokecenter", 1), 1 },
            { ("ViridianPokecenter", 0), 1 },
            { ("UndergroundPathRoute5", 0), 16 },
            { ("UndergroundPathRoute6", 1), 17 },
            { ("UndergroundPathRoute5", 1), 16 },
            { ("UndergroundPathRoute6", 0), 17 },
            { ("UndergroundPathRoute7", 0), 18 },
            { ("UndergroundPathRoute8", 1), 19 },
            { ("UndergroundPathRoute7", 1), 18 },
            { ("UndergroundPathRoute8", 0), 19 },
            { ("CeladonPokecenter", 1), 6 },
            { ("CeruleanMart", 1), 3 },
            { ("VermilionPokecenter", 0), 5 },
            { ("CeladonPokecenter", 0), 6 },
            { ("CeruleanMart", 0), 3 },
            { ("VermilionPokecenter", 1), 5 },
            { ("PewterMart", 1), 2 },
            { ("PewterMart", 0), 2 },
            { ("FuchsiaGoodRodHouse", 0), 7 },
            { ("Route22Gate", 2), 34 },
            { ("FuchsiaGoodRodHouse", 1), 7 },
            { ("Route22Gate", 3), 34 },
            { ("FuchsiaGoodRodHouse", 2), 7 },
            { ("Route22Gate", 0), 33 },
            { ("Route22Gate", 1), 33 },
            { ("Route15Gate1F", 1), 26 },
            { ("Route7Gate", 1), 18 },
            { ("Route15Gate1F", 0), 26 },
            { ("Route7Gate", 0), 18 },
            { ("Route15Gate1F", 3), 26 },
            { ("Route7Gate", 3), 18 },
            { ("Route15Gate1F", 2), 26 },
            { ("Route7Gate", 2), 18 },
            { ("ViridianMart", 1), 1 },
            { ("ViridianMart", 0), 1 },
            { ("FuchsiaPokecenter", 1), 7 },
            { ("FuchsiaPokecenter", 0), 7 },
            { ("CeruleanBadgeHouse", 2), 3 },
            { ("CeruleanBadgeHouse", 1), 3 },
            { ("SilphCoElevator", 1), 181 },
            { ("CeruleanBadgeHouse", 0), 3 },
            { ("SilphCoElevator", 0), 181 },
            { ("CeladonMart1F", 2), 6 },
            { ("CeladonMart1F", 3), 6 },
            { ("CeladonDiner", 0), 6 },
            { ("CeladonMart1F", 0), 6 },
            { ("CeladonDiner", 1), 6 },
            { ("CeladonMart1F", 1), 6 },
            { ("FuchsiaMeetingRoom", 1), 7 },
            { ("SaffronPokecenter", 1), 10 },
            { ("FuchsiaMeetingRoom", 0), 7 },
            { ("SaffronPokecenter", 0), 10 },
        };

        public static readonly Dictionary<YoloballTypes, Joypad[]> Yoloballs = new Dictionary<YoloballTypes, Joypad[]>() {
            {YoloballTypes.Normal, new Joypad[] {Joypad.Down | Joypad.A, Joypad.Up | Joypad.A}},
            {YoloballTypes.NormalSlot2, new Joypad[] {Joypad.Down | Joypad.A, Joypad.Down | Joypad.Right | Joypad.A}},
            {YoloballTypes.Select, new Joypad[] {Joypad.Down | Joypad.A, Joypad.Select, Joypad.A}},
            {YoloballTypes.SelectSlot2, new Joypad[] {Joypad.Down | Joypad.A, Joypad.Down | Joypad.Select, Joypad.A}},
        };

        public RedBlue(string rom, string saveName, bool speedup = false) : base(rom, saveName, speedup ? SpeedupFlags.All : SpeedupFlags.None) { }

        public override void Inject(Joypad joypad) {
            CpuWrite(0xFFF8, (byte) joypad);
        }

        public bool Yoloball(YoloballTypes type=YoloballTypes.Normal) {
            AdvanceWithJoypadToAddress(Joypad.B, Sym["ManualTextScroll"]);
            Inject(Joypad.A);
            AdvanceFrame(Joypad.A);
            AdvanceToAddress(Sym["PlayCry"]);
            Press(Yoloballs[type]);
            return AdvanceWithJoypadToAddress(Joypad.A, Sym["ItemUseBall.captured"], Sym["ItemUseBall.failedToCapture"]) == Sym["ItemUseBall.captured"];
        }

        public bool SafariBall() {
            AdvanceWithJoypadToAddress(Joypad.A, Sym["ManualTextScroll"]);
            Inject(Joypad.B);
            AdvanceFrame(Joypad.B);
            return AdvanceWithJoypadToAddress(Joypad.A, Sym["ItemUseBall.captured"], Sym["ItemUseBall.failedToCapture"]) == Sym["ItemUseBall.captured"];
        }

        public void nopal() {
            base.AdvanceToAddress(0x100);
        }

        public void pal() {
            base.AdvanceToAddress("biosReadKeypad");
            base.AdvanceFrame(Joypad.Left);
            base.AdvanceToAddress(0x100);
        }

        public void palHold() {
            base.AdvanceToAddress("biosReadKeypad");
            base.AdvanceWithJoypadToAddress(Joypad.Left, 0x100);
        }

        public void nopalAB() {
            base.AdvanceToAddress("biosReadKeypad");
            base.AdvanceWithJoypadToAddress(Joypad.A, 0100);
        }

        public void palAB() {
            base.AdvanceToAddress("biosReadKeypad");
            for (int i = 0; i < 70; i++) {
                base.AdvanceFrame(Joypad.Left);
            }
            base.AdvanceToAddress("biosReadKeypad");
            base.AdvanceWithJoypadToAddress(Joypad.Left | Joypad.A, 0x100);
        }

        public void palABRel() {
            base.AdvanceToAddress("biosReadKeypad");
            for(int i = 0; i < 70; i++) {
                base.AdvanceFrame(Joypad.Left);
            }
            base.AdvanceToAddress("biosReadKeypad");
            base.AdvanceFrame(Joypad.Left | Joypad.A);
            base.AdvanceToAddress(0x100);
        }

        public void gfSkip() {
            base.Press(Joypad.Start);
        }

        public void gfWait() {
            base.AdvanceToAddress("PlayShootingStar.endDelay");
        }

        public void UpBSel() {
            base.Press(Joypad.Up | Joypad.B | Joypad.Select, Joypad.Up | Joypad.B | Joypad.Select);
        }

        public void Hop1() {
            base.AdvanceToAddress("AnimateIntroNidorino");
            base.AdvanceToAddress("CheckForUserInterruption");
            base.Press(Joypad.A);
        }
    }
}
