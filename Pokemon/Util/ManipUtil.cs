using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pokemon {
    public class ManipUtil {
        public static object WriteLock = new object();
        public static string checkMultiplePathsIGT(string saveName, string file, string outFile, string gameName, string targetPoke, YoloballTypes yoloballType) {
            StreamReader manips = new StreamReader(file);
            int bestIGT = 0;
            string bestPath = "";
            int path = 0;
            while (manips.Peek() >= 0) {
                Console.WriteLine("Manip #{0}", path++);
                string manip = manips.ReadLine();
                int current = checkIGT0GenericEncounter(saveName, manip, targetPoke, yoloballType, false, false);
                if (current > bestIGT) {
                    bestIGT = current;
                    bestPath = manip;
                } else if (current == bestIGT) {
                    bestPath += "\n" + manip;
                }
            }

            StreamWriter manipOutput = new StreamWriter(outFile);
            manipOutput.WriteLine($"{bestPath}\nIGT: {bestIGT}/60");

            return bestPath + "\nIGT: " + bestIGT.ToString() + "/60";
        }

        public static int checkIGT0GenericEncounter(string saveName, string path, string targetPoke, YoloballTypes yoloballType, bool check3600, bool verbose, params (byte, byte, byte)[] itemPickups) {
            object frameLock = new object();
            object writeLock = new object();

            int successes = 0;
            int total = 60;
            if (check3600) {
                total = 3600;
            }

            int numSims = 0;
            byte sec;
            byte frame;
            Dictionary<string, int> dvSpread = new Dictionary<string, int>();

            int gbCount = 16;

            // Change based on what game you're doing

            Red[] gbs = new Red[gbCount];
            for (int i = 0; i < gbCount; i++) {
                gbs[i] = new Red(true, saveName);
            }
            // gbs[0].Record("test");

            // gbs[0].nopalAB();
            gbs[0].UpBSel();
            // gbs[0].gfSkip();
            // gbs[0].intro0();
            gbs[0].Press(Joypad.Start);
            gbs[0].AdvanceToAddress("igtInject");
            byte[] igtState = gbs[0].SaveState();


            MultiThread.For(total, gbs, (gb, iterator) => {
                lock(frameLock) {
                    gb.LoadState(igtState);
                    sec = (byte)(numSims / 60);
                    frame = (byte)(numSims % 60);
                    gb.CpuWrite("wPlayTimeSeconds", sec);
                    gb.CpuWrite("wPlayTimeFrames", frame);
                    numSims++;
                }

                gb.Press(Joypad.A, Joypad.A);
                gb.AdvanceToAddress("JoypadOverworld");

                CpuAddress ret = gb.Execute(path, itemPickups);
                if (ret == gb.Sym["CalcStats"]) {
                    RbyPokemon enemyMon = gb.GetEnemyMon();
                    string pokeName = enemyMon.Species.Name;
                    if (pokeName == targetPoke) {
                        string DVs = enemyMon.DVs.Value.ToString("X4");
                        lock(WriteLock) {
                            if (!dvSpread.ContainsKey(DVs)){
                                dvSpread.Add(DVs, 1);
                            } else {
                                dvSpread[DVs]++;
                            }
                        }

                        bool yoloball = gb.Yoloball(yoloballType);
                        if (yoloball) {
                            lock(WriteLock) {
                                successes++;
                            }
                        }
                    } else {
                        lock(WriteLock) {
                            if (!dvSpread.ContainsKey(pokeName)) {
                                dvSpread.Add(pokeName, 1);
                            } else {
                                dvSpread[pokeName]++;
                            }
                        }
                    }
                } else {
                    lock(WriteLock) {
                        if (!dvSpread.ContainsKey("NO ENCOUNTER")) {
                            dvSpread.Add("NO ENCOUNTER", 1);
                        } else {
                            dvSpread["NO ENCOUNTER"]++;
                        }
                    }
                }
            });

            string summary = String.Format("Yoloball Success: {0}/{1}\n\n\n", successes, total);
            foreach(KeyValuePair<string, int> kvp in dvSpread) {
                summary += String.Format("{0}: {1}/{2}\n", kvp.Key, kvp.Value, total);
            }

            if (verbose) {
                Console.WriteLine(summary);
            }
            return successes;
        }

        public static int checkIGT0GenericEncounterSingleThread(string saveName, string path, string targetPoke, YoloballTypes yoloballType, bool check3600, bool verbose, params (byte, byte, byte)[] itemPickups) {
            object frameLock = new object();
            object writeLock = new object();

            int successes = 0;
            int total = 60;
            if (check3600) {
                total = 3600;
            }
            Dictionary<string, int> dvSpread = new Dictionary<string, int>();


            // Change based on what game you're doing
            Red gb = new Red(true, saveName);
            // gb.Record("test");
            // gb.Show();
            gb.nopalAB();
            // gb.Record("wg_paras");
            // gb.palHold();
            gb.UpBSel();
            gb.Press(Joypad.Start);
            gb.AdvanceToAddress("igtInject");
            byte[] igtState = gb.SaveState();

            for (byte f = 0; f < 60; f++) {
                gb.LoadState(igtState);
                gb.CpuWrite("wPlayTimeSeconds", (byte) 0);
                gb.CpuWrite("wPlayTimeFrames", f);

                gb.Press(Joypad.A);

                int backouts = 0;
                for (int j = 0; j < backouts; ++j){
                    gb.Press(Joypad.B, Joypad.A);
                }

                gb.Press(Joypad.A);
                gb.AdvanceToAddress("JoypadOverworld");

                CpuAddress ret = gb.Execute(path, itemPickups);
                if (ret == gb.Sym["CalcStats"]) {

                    RbyPokemon enemyMon = gb.GetEnemyMon();
                    string pokeName = enemyMon.Species.Name;
                    Console.WriteLine("Tile: {0}, Mon: {1}", gb.GetCurTile(), enemyMon);
                    if (pokeName == targetPoke) {
                        string DVs = enemyMon.DVs.Value.ToString("X4");
                        lock(WriteLock) {
                            if (!dvSpread.ContainsKey(DVs)){
                                dvSpread.Add(DVs, 1);
                            } else {
                                dvSpread[DVs]++;
                            }
                        }

                        bool yoloball = gb.Yoloball(yoloballType);
                        if (yoloball) {
                            lock(WriteLock) {
                                successes++;
                            }
                        }
                    } else {
                        lock(WriteLock) {
                            if (!dvSpread.ContainsKey(pokeName + gb.GetEnemyMon().Level)) {
                                dvSpread.Add(pokeName + gb.GetEnemyMon().Level, 1);
                            } else {
                                dvSpread[pokeName + gb.GetEnemyMon().Level]++;
                            }
                        }
                    }
                } else {
                    lock(WriteLock) {
                        if (!dvSpread.ContainsKey("NO ENCOUNTER")) {
                            dvSpread.Add("NO ENCOUNTER", 1);
                        } else {
                            dvSpread["NO ENCOUNTER"]++;
                        }
                    }
                }
            }
            gb.Dispose();

            string summary = String.Format("Yoloball Success: {0}/{1}\n\n\n", successes, total);
            foreach(KeyValuePair<string, int> kvp in dvSpread) {
                summary += String.Format("{0}: {1}/{2}\n", kvp.Key, kvp.Value, total);
            }

            if (verbose) {
                Console.WriteLine(summary);
            }
            if (dvSpread.ContainsKey("NO ENCOUNTER"))
                return dvSpread["NO ENCOUNTER"];
            else
                return 0;
        }

        public static void changeNickname(Rby gb, int len) {
            // change party1mon nickname, len is in characters from 1 to 10
            byte[] nickname;
            nickname = gb.CpuRead("wPartyMonNicks", 11);
            for (int k = 0; k < 11; k++) {
                if (k < len) {
                    nickname[k] = 0x80;
                } else {
                    nickname[k] = 0x50;
                }
            }
            gb.CpuWrite("wPartyMonNicks", nickname);
        }

        public static string spacePath(string path) {
            string output = "";

            string[] validActions = new string[] { "A", "U", "D", "L", "R", "S_B" };
            while(path.Length > 0) {
                if (validActions.Any(path.StartsWith)) {
                    if (path.StartsWith("S")) {
                        output += "S_B";
                        path = path.Remove(0, 3);
                    } else {
                        output += path[0];
                        path = path.Remove(0, 1);
                    }

                    output += " ";
                } else {
                    throw new Exception(String.Format("Invalid Path Action Recieved: {0}", path));
                }
            }

            return output.Trim();
        }
    }
}