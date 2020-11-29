using System;
using System.Collections.Generic;

namespace Pokemon {

    public class RedResult : SimulationResult {

        public bool Victory {
            get { return HpLeft > 0; }
        }

        public ushort HpLeft;
        public ushort DamageTaken;
    }

    public class ParaResult : SimulationResult {
        public bool Victory {
            get { return HpLeft > 0; }
        }

        public ushort HpLeft;
        public ushort DamageTaken;
        public bool Paralyzed;
    }

    public class RedSimulation : FightSimulation<Red, RedResult> {

        public ushort StartHP;
        public RedSimulation(ushort startHP) => StartHP = startHP;

        public RedSimulation() {StartHP = 65535;}
        public bool paralyzed = false;

        public bool Lorelei(Red gb, Dictionary<string, object> memory) {
            RbyPokemon enemyMon = gb.GetEnemyMon();
            RbyBag bag = gb.GetBag();

            if (bag["X ACCURACY"].Quantity == 3) {
                gb.UseXAccuracy();
            } else {
                gb.UseSlot1();
            }

            return true;
        }

        public bool agathaCandy(Red gb, Dictionary<string, object> memory) {
            RbyPokemon enemyMon = gb.GetEnemyMon();
            string name = enemyMon.Species.Name;
            RbyPokemon battleMon = gb.GetBattleMon();
            RbyBag bag = gb.GetBag();

            if (battleMon.Asleep) {
                gb.UsePokeFlute();
            }
            if (battleMon.Paralyzed && !bag.Contains("PARLYZ HEAL") && !bag.Contains("FULL RESTORE")) {
                return false;
            }

            if (name == "GENGAR" && bag["X SPECIAL"].Quantity == 5) {
                gb.UseXSpecial();
            } else if (name == "GOLBAT") {
                if (enemyMon.HP != enemyMon.MaxHP || gb.CpuRead("wPlayerMonSpecialMod") == 7 || battleMon.PP[3] == 1) {
                    gb.UseSlot3();
                } else {
                    gb.UseSlot4();
                }
            } else if (name == "ARBOK") {
                if (battleMon.Paralyzed) {
                    if (bag.Contains("FULL RESTORE")) {
                        gb.UseFullRestore();
                    } else if (bag.Contains("PARLYZ HEAL")) {
                        gb.UseHealingItem("PARLYZ HEAL");
                    } else {
                        return false;
                    }
                } else if (gb.CpuRead("wPlayerMonSpecialMod") == 7 && battleMon.HP >= 24 && battleMon.HP <= 60) {
                    gb.UseXSpecial();
                } else {
                    gb.UseSlot2();
                }
            } else {
                gb.UseSlot2();
            }

            return true;
        }

        public bool champCandy(Red gb, Dictionary<string, object> memory) {
            RbyPokemon enemyMon = gb.GetEnemyMon();
            string name = enemyMon.Species.Name;
            RbyPokemon battleMon = gb.GetBattleMon();
            RbyBag bag = gb.GetBag();

            if (name == "PIDGEOT") {
                if (bag["X SPECIAL"].Quantity == 3) {
                    gb.UseXSpecial();
                } else if (gb.CpuRead("wEnemyBattleStatus1") == 16) {
                    gb.UseSlot4();
                } else if (bag.Contains("X ACCURACY")) {
                    gb.UseXAccuracy();
                } else {
                    gb.UseSlot1();
                }
            } else if (bag.Contains("X ACCURACY")) {
                if (name == "RHYDON") {
                    gb.UseXAccuracy();
                } else if (name == "ALAKAZAM") {
                    gb.UseSlot2();
                }
            } else if (name == "GYARADOS" && battleMon.PP[0] == 2) {
                gb.UseSlot3();
            } else {
                gb.UseSlot1();
            }

            return true;
        }

        public bool lateDrillRival(Red gb, Dictionary<string, object> memory) {
            RbyPokemon enemyMon = gb.GetEnemyMon();
            string enemyName = enemyMon.Species.Name;

            if (enemyName == "PIDGEOTTO") {
                gb.UseSlot3();
            } else if (enemyName == "GYARADOS") {
                gb.UseSlot3();
            } else if (enemyName == "GROWLITHE") {
                if (gb.GetBag()["POTION"].Quantity == 3 && gb.GetBattleMon().HP <= 8) {
                    gb.UseHealingItem("POTION");
                } else {
                    gb.UseSlot4();
                }
            } else {
                gb.UseSlot2();
            }
            return true;
        }

        public bool earlyDrillRival(Red gb, Dictionary<string, object> memory) {
            RbyPokemon enemyMon = gb.GetEnemyMon();
            RbyPokemon battleMon = gb.GetBattleMon();
            RbyBag bag = gb.GetBag();

            if (battleMon.Burned) {
                return false;
            }

            if (enemyMon.Species.Name == "PIDGEOTTO") {
                if (battleMon.HP <= 8 && bag["POTION"].Quantity == 3 && bag["X ACCURACY"].Quantity == 11) {
                    gb.UseHealingItem("POTION");
                // } else if (battleMon.HP == 8 || battleMon.HP == 16) {
                } else if (battleMon.HP == 16) {
                    gb.UseSlot3();
                } else if (bag["X ACCURACY"].Quantity == 11) {
                    gb.UseXAccuracy();
                } else {
                    gb.UseSlot1();
                }
            } else if (enemyMon.Species.Name == "GYARADOS") {
                if (bag["X ACCURACY"].Quantity == 11) {
                    gb.UseSlot3();
                } else {
                    gb.UseSlot1();
                }
            } else if (enemyMon.Species.Name == "GROWLITHE") {
                if (battleMon.HP <= 7 && bag["POTION"].Quantity == 3) {
                    gb.UseHealingItem("POTION");
                } else if (bag["X ACCURACY"].Quantity == 11) {
                    gb.UseXAccuracy();
                } else {
                    gb.UseSlot1();
                }
            } else {
                gb.UseSlot1();
            }
            return true;
        }

        public override void TransformInitialState(Red gb, ref byte[] state) {
            gb.LoadState(state);
            gb.AdvanceFrame();
            if (StartHP != 65535 && StartHP != gb.GetBattleMon().HP) {
                gb.CpuWriteWord("wPartyMon1HP", StartHP);
                gb.CpuWriteWord("wBattleMonHP", StartHP);
            }
            gb.AdvanceToAddress("PlayCry");
            gb.AdvanceToAddress("_Joypad");
            state = gb.SaveState();
        }

        public override bool HasSimulationEnded(Red gb, Dictionary<string, object> memory) {
            return gb.GetBattleMon().HP == 0 || gb.CpuRead("wIsInBattle") == 0;
        }

        public override void SimulationStart(Red gb, Dictionary<string, object> memory) {
            memory["starthp"] = gb.GetBattleMon().HP;
        }

        public override void SimulationEnd(Red gb, Dictionary<string, object> memory, ref RedResult result) {
            result.HpLeft = gb.GetBattleMon().HP;
            result.DamageTaken = (ushort) (((ushort) memory["starthp"]) - gb.GetBattleMon().HP);
        }
    }
}
