using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Pokemon {

    public class Paras {

        public static StreamWriter searchOutput = new StreamWriter("logs/search.txt");
        public static HashSet<int> seenStates = new HashSet<int>();
        public static byte igtSec = 1;
        public static byte gbCount = 12;
        public static Red[] Gbs = new Red[gbCount];

        public static object WriteLock = new object();

        public static void main() {
            StartSearch();
        }

        public static void InitTiles(Rby gb) {
            RbyMap map = gb.Maps["MtMoonB2F"];

            AStar.EdgeTrimmingFn trim = AStar.None;
            map[5,6].AddEdge(0, new RbyEdge(Action.Down, gb.Maps["MtMoonB1F"][23, 3], 0, 0));
            map[4,7].AddEdge(0, new RbyEdge(Action.Right, gb.Maps["MtMoonB1F"][23, 3], 0, 0));

            map[5,6].CanMoveDown = true;
            map[4,7].CanMoveRight = true;

            for (int y = 7; y < 14; y++) {
                map[y, 2].CanMoveDown = false;
            }

            AStar.GenerateEdges(map, 0, map[5, 7], Action.Right | Action.Left | Action.Down | Action.Up | Action.A, 17, trim);

            map = gb.Maps["MtMoonB1F"];
            map[26,3].CanMoveDown = true;
            map[26,3].AddEdge(0, new RbyEdge(Action.Down, map[26,3], 0, 4));
            AStar.GenerateEdges(map, 0, map[26, 3], Action.Right | Action.Left | Action.Down | Action.Up | Action.A, 17, trim);
        }

        public static void StartSearch() {
            for (int i = 0; i < 12; i++) {
                Gbs[i] = new Red(true, "basesaves/red/paras.sav");
            }

            Red gb = Gbs[0];

            InitTiles(gb);
            Console.WriteLine("InitTiles done");
            //gb.Show();

            gb.palHold();
            gb.UpBSel();

            gb.Press(Joypad.Start);
            gb.AdvanceToAddress("igtInject"); //1C:79D6 igtInject

            byte[] igtState = gb.SaveState();

            EncounterRNGState[] initialStates = new EncounterRNGState[60 * igtSec];
            RbyTile curTile = gb.GetCurTile();


            searchOutput.WriteLine("Tile: {0}", curTile);
            Console.WriteLine("Tile: {0}", curTile);

            for (byte sec = 0; sec < igtSec; sec++) {
                for(byte frame = 0; frame < 60; frame++) {
                    gb.LoadState(igtState);
                    gb.setIGT(0, 0, sec, frame);
                    gb.Press(Joypad.A);

                    int backouts = 0;
                    for (int j = 0; j < backouts; ++j){
                        gb.Press(Joypad.B, Joypad.A);
                    }

                    gb.Press(Joypad.A);
                    gb.AdvanceToAddress("JoypadOverworld");
                    initialStates[frame + sec * 60] = new EncounterRNGState(gb);
                }
            }


            Search(new EncounterState() {
                Log = "",
                Tile = curTile,
                APressCounter = 0,
                WastedFrames = 0,
                EdgeSet = 0,
                NextEdgeSet = 0,
                IGTs = initialStates,
            });
        }

        public static void Search(EncounterState state) {
            if (!seenStates.Add(state.GetHashCode())) {
                return;
            }

            int maxSteps = 0;
            int maxAPress = 3;
            int maxStartFlash = 0;
            double maxCost = maxSteps*17 + maxAPress*2 + maxStartFlash*52;

            foreach(RbyEdge edge in state.Tile.Edges[state.EdgeSet]) {
                // two checks to prevent impossible movement
                if (state.APressCounter == 2 && edge.Action == Action.StartB) {
                    continue;
                }
                if (state.APressCounter > 0 && edge.Action == Action.A) {
                    continue;
                }

                int newCost = state.WastedFrames + edge.Cost;
                if (newCost > maxCost) {
                    return;
                }

                int successes = 60 * igtSec;
                byte parasCatches = 0;


                EncounterState newState = new EncounterState() {
                    Log = state.Log + edge.Action.LogString() + " ",
                    Tile = edge.NextTile,
                    EdgeSet = edge.NextEdgeset,
                    APressCounter = 0,
                    WastedFrames = newCost,
                    IGTs = new EncounterRNGState[60 * igtSec],
                };

                if (edge.Action == Action.A) {
                    newState.APressCounter = 2;
                } else if (edge.Action.IsDpad()) {
                    newState.APressCounter = Math.Max(0, state.APressCounter - 1);
                }

                MultiThread.For(60 * igtSec, Gbs, (gb, igt) => {
                    if (successes < 57) {
                        successes--;
                        return;
                    }

                    // null represents igt frame got an encounter earlier
                    if (state.IGTs[igt] == null) {
                        successes--;
                        return;
                    }

                    gb.LoadState(state.IGTs[igt].SaveState);
                    CpuAddress result = gb.Execute(edge.Action);
                    newState.IGTs[igt] = new EncounterRNGState(gb);

                    if (edge.Action.IsDpad()) {
                        if (result == gb.Sym["CalcStats"]) {
                            RbyPokemon enemyMon = gb.GetEnemyMon();
                            RbyTile nextTile = edge.NextTile;
                            if (enemyMon.Species.Name == "PARAS" && nextTile.Y == 3) {
                                byte[] hpstate = gb.SaveState();

                                bool yoloball = gb.Yoloball();
                                if (yoloball) {
                                    gb.LoadState(hpstate);
                                    gb.CpuWriteWord("wPartyMon2HP", 1);
                                    gb.CpuWriteWord("wBattleMonHP", 1);
                                    bool yoloball2 = gb.Yoloball();
                                    if (yoloball2) {
                                        parasCatches++;
                                    }
                                }
                            }
                        }

                        if (result != gb.Sym["JoypadOverworld"]) {
                            successes--;
                            newState.IGTs[igt] = null;
                        }
                    } else if (edge.Action == Action.A) {
                        if (result == gb.Sym["PrintLetterDelay"]) {
                            successes--;
                        }
                    }

                });

                if (successes < 57 * igtSec) {
                    continue;
                }
                if (parasCatches >= 57 * igtSec) {
                    Console.WriteLine("FOUND MANIP :: {0}/{1} :: Wasted Frames: {2}", parasCatches, 60 * igtSec, newState.WastedFrames);
                    searchOutput.WriteLine("{0} :: Wasted Frames: {1} :: IGT: {2}/{3}", newState.Log, newState.WastedFrames, parasCatches, 60 * igtSec);
                    searchOutput.Flush();
                    return;
                } else if (parasCatches > 2 * igtSec && parasCatches < 57 * igtSec) {
                    continue;
                }

                Search(newState);
            }
        }
    }
}