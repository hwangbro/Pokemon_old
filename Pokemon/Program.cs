using System;
using System.Diagnostics;


namespace Pokemon {

    public unsafe class Program {

        private static int gbCount = 16;

        public static void Main(string[] args) {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        public static void runSimulation(string name, string saveStateLoc) {
            // ideal count for my system
            // save state occurs after first pokemon cry
            // todo generic init for gb array for any game type

            Red[] gbs = new Red[gbCount];

            for (int i = 0; i < gbCount; i++) {
                gbs[i] = new Red(true);
            }
            // gbs[0].Record("test");

            RedSimulation a = new RedSimulation(79);
            a.Simulate($"Simulation/{name}", gbs, 10000, $"Simulation/saves/{saveStateLoc}", a.agathaCandy);
        }

        public static void runHPSimulations(ushort lowerHP, ushort upperHP) {
            Red[] gbs = new Red[gbCount];

            for (int i = 0; i < gbCount; i++) {
                gbs[i] = new Red(true);
            }
            RedSimulation a;

            for (ushort i = lowerHP; i <= upperHP; i++) {
                a = new RedSimulation(i);
                a.Simulate($"Simulation/RedResults/EarlyDrill/TowerRival/EarlyDrillPotion_{i}", gbs, 2000, "Simulation/saves/red/tower_rival.gqs", a.earlyDrillRival);
                // a.Simulate($"Simulation/RedResults/EarlyDrill/TowerRival/LateDrillPotion_{i}", gbs, 2000, "Simulation/saves/red/tower_rival.gqs", a.lateDrillRival);
            }
        }
    }
}