using System;
using System.Collections.Generic;
using System.IO;

namespace Pokemon {
    public class Cans {
        public static object WriteLock = new object();
        public static void testcans() {
            object frameLock = new object();
            object writeLock = new object();

            int firstCan = 0;
            int secondCan = 0;
            int total = 3600;

            int numSims = 0;
            byte sec;
            byte frame;

            int gbCount = 16;

            // Change based on what game you're doing

            Red[] gbs = new Red[gbCount];
            for (int i = 0; i < gbCount; i++) {
                gbs[i] = new Red(true);
            }

            // gbs[0].palHold();
            gbs[0].UpBSel();
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

                CpuAddress ret = gb.Execute("S_B D L A L L A U R U A U U U U");
                bool first = false;
                bool second = false;
                if (gb.CpuRead("wFirstLockTrashCanIndex") == 8) {
                    gb.Execute("A");
                    first = true;
                    if (gb.CpuRead("wSecondLockTrashCanIndex") == 11) {
                        second = true;
                    }
                }
                lock(writeLock) {
                    if (first) firstCan++;
                    if (second) secondCan++;
                }
            });
            Console.WriteLine("First can success: {0}/{1}, second can success: {2}/{0}", firstCan, total, secondCan);
        }

        public static void testcans2() {
            Red gb = new Red(true);
            // gb.Show();
            gb.UpBSel();
            gb.Press(Joypad.Start);
            gb.AdvanceToAddress("igtInject");
            byte[] igtState = gb.SaveState();
            int firstCan = 0;
            int secondCan = 0;

            for (byte sec = 0; sec < 60; sec++) {
                for (byte frame = 0; frame < 60; frame++) {
                    gb.LoadState(igtState);
                    gb.CpuWrite("wPlayTimeSeconds", sec);
                    gb.CpuWrite("wPlayTimeFrames", frame);
                    gb.Press(Joypad.A, Joypad.A);
                    gb.AdvanceToAddress("JoypadOverworld");
                    gb.Execute("D A L L L A U R U U U U U");
                    string res = String.Format("[{0}] [{1}]: ", sec, frame);
                    if (gb.CpuRead("wFirstLockTrashCanIndex") == 8) {
                        res += "FIRST CAN SUCCESS";
                        firstCan++;
                        gb.Execute("A");
                        if (gb.CpuRead("wSecondLockTrashCanIndex") == 5) {
                            secondCan++;
                            res += ", SECOND CAN SUCCESS";
                        } else {
                            res += ", SECOND CAN FAILED";
                        }
                    } else {
                        res += "FIRST CAN FAILED";
                    }
                    lock(WriteLock) {
                        Console.WriteLine(res);
                    }
                }
            }
            Console.WriteLine("Summary: {0}/{1}, {2}/{1}", firstCan, 3600, secondCan);
        }
    }
}