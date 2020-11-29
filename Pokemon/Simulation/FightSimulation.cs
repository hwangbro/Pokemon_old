using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Newtonsoft.Json;

namespace Pokemon {

    public class SimulationResult {

        [JsonIgnore]
        public double Frames {
            get { return Cycles / GameBoy.NumSamplesPerFrame; }
        }

        [JsonIgnore]
        public double Seconds {
            get { return Cycles / Math.Pow(2, 21); }
        }

        public ulong Cycles;
    }

    public abstract class FightSimulation<Gb, Result> where Gb : GameBoy
                                                      where Result : SimulationResult, new() {
        public delegate bool ActionCallback(Gb gb, Dictionary<string, object> memory);

        public List<Result> Results;

        public void Simulate(string name, Gb[] gbs, int numSimulations, string initialState, ActionCallback actionCallback, bool write = true) {
            Results = new List<Result>();

            object frameLock = new object();
            object writeLock = new object();

            byte[] state = File.ReadAllBytes(initialState);
            TransformInitialState(gbs[0], ref state);

            int seed = (int) DateTime.Now.Ticks & 0x0000FFFF;
            Random random = new Random(seed);
            Console.WriteLine("Random Seed: " + seed);
            uint numSims = 0;

            MultiThread.For(numSimulations, gbs, (gb, iterator) => {
                Dictionary<string, object> memory;
                ulong start;
                lock(frameLock) {
                    gb.LoadState(state);
                    gb.RandomizeRNG(random);
                    memory = new Dictionary<string, object>();
                    start = gb.GetCycleCount();
                    SimulationStart(gb, memory);
                    Console.WriteLine(numSims++ + " " + System.Threading.Thread.CurrentThread.ManagedThreadId);
                }

                actionCallback(gb, memory);

                CpuAddress ret;
                Dictionary<int, Joypad> alt = new Dictionary<int, Joypad> { {1, Joypad.B}, {-1, Joypad.A}};
                int idx = 1;
                Joypad joypad = Joypad.B;
                do {
                    ret = gb.AdvanceToAddress(gb.Sym["ManualTextScroll"], gb.Sym["HandleMenuInput"], gb.Sym["PrintStatsBox.PrintStats"], gb.Sym["TextCommand_PAUSE"]);
                    if(ret == gb.Sym["ManualTextScroll"] || ret == gb.Sym["PrintStatsBox.PrintStats"]) {
                        joypad = alt[idx];
                        idx *= -1;
                        gb.Inject(joypad);
                        gb.AdvanceFrame(joypad);
                    }
                    else if (ret == gb.Sym["TextCommand_PAUSE"]) {
                            gb.Inject(alt[idx]);
                            gb.AdvanceWithJoypadToAddress(alt[idx], gb.Sym["_Joypad"]);
                            idx *= -1;
                    } else {
                        gb.AdvanceToAddress(gb.Sym["_Joypad"]);
                        if(!actionCallback(gb, memory)) {
                            gb.CpuWriteWord("wBattleMonHP", 0);
                            break;
                        }
                        idx = 1;
                    }
                } while(!HasSimulationEnded(gb, memory));

                lock(writeLock) {
                    Result result = new Result() {
                        Cycles = gb.GetCycleCount() - start,
                    };
                    SimulationEnd(gb, memory, ref result);
                    Results.Add(result);
                }
            });

            if(write) {
                File.WriteAllText(name + ".json", JsonConvert.SerializeObject(Results));
            }
        }

        public abstract void TransformInitialState(Gb gb, ref byte[] state);
        public abstract void SimulationStart(Gb gb, Dictionary<string, object> memory);
        public abstract bool HasSimulationEnded(Gb gb, Dictionary<string, object> memory);
        public abstract void SimulationEnd(Gb gb, Dictionary<string, object> memory, ref Result result);
    }
}
